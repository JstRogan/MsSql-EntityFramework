GO
CREATE TRIGGER trg_Students_LimitGroupSize
ON Students
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxGroupSize INT = 30;

    IF EXISTS (
        SELECT 1
        FROM Students s
        WHERE s.GroupId IN (SELECT DISTINCT i.GroupId FROM inserted i)
        GROUP BY s.GroupId
        HAVING COUNT(*) > @MaxGroupSize
    )
    BEGIN
        THROW 50001, 'Cannot add student: group already has 30 students.', 1;
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

GO
CREATE TRIGGER trg_Students_UpdateGroupCount
ON Students
AFTER INSERT, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedGroups AS (
        SELECT GroupId FROM inserted
        UNION
        SELECT GroupId FROM deleted
    ),
    GroupCounts AS (
        SELECT s.GroupId, COUNT(*) AS StudentCount
        FROM Students s
        JOIN AffectedGroups ag ON ag.GroupId = s.GroupId
        GROUP BY s.GroupId
    )
    UPDATE g
    SET g.StudentsCount = ISNULL(gc.StudentCount, 0)
    FROM [Groups] g
    JOIN AffectedGroups ag ON ag.GroupId = g.GroupId
    LEFT JOIN GroupCounts gc ON gc.GroupId = g.GroupId;
END;
GO

GO
CREATE TRIGGER trg_Students_AutoEnrollIntroCourse
ON Students
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IntroCourseId INT;

    SELECT TOP (1) @IntroCourseId = c.CourseId
    FROM Courses c
    WHERE c.Name = N'Введение в программирование';

    IF @IntroCourseId IS NULL
        RETURN;

    INSERT INTO StudentCourses (StudentId, CourseId)
    SELECT i.StudentId, @IntroCourseId
    FROM inserted i
    WHERE NOT EXISTS (
        SELECT 1
        FROM StudentCourses sc
        WHERE sc.StudentId = i.StudentId
          AND sc.CourseId = @IntroCourseId
    );
END;
GO

GO
CREATE TRIGGER trg_Grades_LowGradeWarning
ON Grades
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LowGradeThreshold INT = 3;

    INSERT INTO Warnings (StudentId, Reason, [Date])
    SELECT i.StudentId,
           N'Low grade: ' + CAST(i.Grade AS NVARCHAR(10)),
           GETDATE()
    FROM inserted i
    WHERE i.Grade < @LowGradeThreshold;
END;
GO

GO
CREATE TRIGGER trg_Teachers_BlockDeleteWithActiveCourses
ON Teachers
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN TeacherCourses tc ON tc.TeacherId = d.TeacherId
        WHERE tc.IsActive = 1
    )
    BEGIN
        THROW 50002, 'Cannot delete teacher: active courses assigned.', 1;
        RETURN;
    END

    DELETE t
    FROM Teachers t
    JOIN deleted d ON d.TeacherId = t.TeacherId;
END;
GO

GO
CREATE TRIGGER trg_Grades_History
ON Grades
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO GradeHistory (GradeId, StudentId, OldGrade, ChangedAt)
    SELECT d.GradeId, d.StudentId, d.Grade, GETDATE()
    FROM deleted d
    JOIN inserted i ON i.GradeId = d.GradeId
    WHERE ISNULL(i.Grade, -1) <> ISNULL(d.Grade, -1);
END;
GO

GO
CREATE TRIGGER trg_Attendance_ConsecutiveAbsences
ON Attendance
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ConsecutiveAbsenceLimit INT = 6;
    DECLARE @Reason NVARCHAR(200) = N'Missed more than 5 consecutive lessons';

    ;WITH Recent AS (
        SELECT a.StudentId,
               a.IsPresent,
               ROW_NUMBER() OVER (PARTITION BY a.StudentId ORDER BY a.LessonDate DESC) AS rn
        FROM Attendance a
        WHERE a.StudentId IN (
            SELECT DISTINCT i.StudentId
            FROM inserted i
            WHERE i.IsPresent = 0
        )
    ),
    Windowed AS (
        SELECT r.StudentId
        FROM Recent r
        WHERE r.rn <= @ConsecutiveAbsenceLimit
        GROUP BY r.StudentId
        HAVING COUNT(*) = @ConsecutiveAbsenceLimit
           AND SUM(CASE WHEN r.IsPresent = 0 THEN 1 ELSE 0 END) = @ConsecutiveAbsenceLimit
    )
    INSERT INTO RetakeList (StudentId, Reason, CreatedAt)
    SELECT w.StudentId,
           @Reason,
           GETDATE()
    FROM Windowed w
    WHERE NOT EXISTS (
        SELECT 1
        FROM RetakeList r
        WHERE r.StudentId = w.StudentId
          AND r.Reason = @Reason
    );
END;
GO

GO
CREATE TRIGGER trg_Students_BlockDeleteWithDebtsOrFails
ON Students
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FailingGradeThreshold INT = 3;

    IF EXISTS (
        SELECT 1
        FROM deleted d
        JOIN Payments p ON p.StudentId = d.StudentId
        WHERE p.IsPaid = 0 OR p.Balance > 0
    )
    OR EXISTS (
        SELECT 1
        FROM deleted d
        JOIN Grades g ON g.StudentId = d.StudentId
        WHERE g.Grade < @FailingGradeThreshold
    )
    BEGIN
        THROW 50003, 'Cannot delete student: debts or failing grades exist.', 1;
        RETURN;
    END

    DELETE s
    FROM Students s
    JOIN deleted d ON d.StudentId = s.StudentId;
END;
GO

GO
CREATE TRIGGER trg_Grades_UpdateStudentAverage
ON Grades
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedStudents AS (
        SELECT StudentId FROM inserted
        UNION
        SELECT StudentId FROM deleted
    )
    UPDATE s
    SET s.AverageGrade = g.AvgGrade
    FROM Students s
    JOIN (
        SELECT g.StudentId, AVG(CAST(g.Grade AS DECIMAL(10, 2))) AS AvgGrade
        FROM Grades g
        JOIN AffectedStudents a ON a.StudentId = g.StudentId
        GROUP BY g.StudentId
    ) g ON g.StudentId = s.StudentId;
END;
GO
