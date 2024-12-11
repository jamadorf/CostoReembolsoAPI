namespace CostoReembolsoAPI.Services
{
    using Oracle.ManagedDataAccess.Client;

    public class DatabaseService(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        public void TestConnection()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();
            Console.WriteLine("Connected to Oracle Database!");
        }
    }
}