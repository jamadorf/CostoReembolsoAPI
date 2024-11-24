using CostoReembolsoAPI.Common;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

public class LogService
{
    private readonly string _connectionString;

    public LogService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void RegistrarLog(string proceso, string movimiento, string novedad, out long transaccion)
    {
        transaccion = 0;

        try
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                using (var command = new OracleCommand(Constants.Log, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_proceso", OracleDbType.Varchar2).Value = proceso;
                    command.Parameters.Add("p_movimiento", OracleDbType.Varchar2).Value = movimiento;
                    command.Parameters.Add("p_novedad", OracleDbType.Varchar2).Value = novedad ?? (object)DBNull.Value;

                    var transactionParam = new OracleParameter("p_trans", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.Parameters.Add(transactionParam);

                    command.ExecuteNonQuery();

                    if (transactionParam.Value is OracleDecimal oracleDecimal)
                    {
                        transaccion = oracleDecimal.ToInt64();
                    }
                    else
                    {
                        transaccion = Convert.ToInt64(transactionParam.Value);
                    }
                }
            }
        }
        catch (OracleException ex)
        {
            Console.WriteLine($"Error al registrar el log en Oracle: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inesperado al registrar el log: {ex.Message}");
        }
    }
}