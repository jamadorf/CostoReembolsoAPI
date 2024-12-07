namespace CostoReembolsoAPI.Services
{
    using Oracle.ManagedDataAccess.Client;

    public class DatabaseService(string connectionString)
    {
        public void TestConnection()
        {
            using var connection = new OracleConnection(connectionString);
            connection.Open();
            Console.WriteLine("Connected to Oracle Database!");
        }
    }
}