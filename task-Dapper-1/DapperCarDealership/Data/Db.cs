using Microsoft.Data.SqlClient;

namespace DapperCarDealership.Data;

public static class Db
{
    public static SqlConnection OpenConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string is missing.");

        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
