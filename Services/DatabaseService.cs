namespace CostoReembolsoAPI.Services
{
    using Oracle.ManagedDataAccess.Client;

    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void TestConnection()
        {
            using var connection = new OracleConnection(_connectionString);
            connection.Open();
            Console.WriteLine("Connected to Oracle Database!");
        }
    }
}