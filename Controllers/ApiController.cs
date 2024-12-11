using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CostoReembolsoAPI.Dtos;
using Oracle.ManagedDataAccess.Types;
using CostoReembolsoAPI.Common;
using CostoReembolsoAPI.Services;

namespace CostoReembolsoAPI.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class ApiController(IConfiguration configuration, LogService logService) : ControllerBase
    {
        private string? DatosObjetoConsumido;
        public long transaccionId;

        private IDbConnection DbConnection
        {
            get
            {
                string connectionString = configuration.GetConnectionString("OracleDbConnection") ?? string.Empty;
                return new OracleConnection(connectionString);
            }
        }

        [HttpGet("categorias-servicios")]
        public async Task<IActionResult> ObtenerCategoriasServicio()
        {
            var response = new CategoriaServicioResponseDto();

            try
            {
                using IDbConnection dbConnection = DbConnection;
                dbConnection.Open();
                using var command = new OracleCommand(Constants.ObtenerCategoriaServicio, (OracleConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("OUT_CATEGORIA_SERVICIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_ESTATUS", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_MENSAJE", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                response.Estatus = int.Parse(command.Parameters["OUT_ESTATUS"].Value.ToString()!);
                response.Mensaje = command.Parameters["OUT_MENSAJE"].Value.ToString()!;

                DatosObjetoConsumido = $@"
                        BEGIN
                            {Constants.ObtenerCategoriaServicio} (
                                OUT_CATEGORIA_SERVICIO => :OUT_CATEGORIA_SERVICIO,
                                OUT_ESTATUS => {response.Estatus},
                                OUT_MENSAJE => '{response.Mensaje}'
                            );
                        END;";

                if (response.Estatus == 0)
                {
                    using var reader = ((OracleRefCursor)command.Parameters["OUT_CATEGORIA_SERVICIO"].Value).GetDataReader();
                    while (reader.Read())
                    {
                        response.CategoriasServicio.Add(new CategoriaServicioDto
                        {
                            TipoServicio = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
                        });
                    }
                }
            }
            catch (OracleException ex)
            {
                logService.RegistrarLog(Constants.ObtenerCategoriaServicio, "I", $"Error al ejecutar el procedimiento almacenado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorLlamandoPlsql;
                response.CategoriasServicio = [];
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                logService.RegistrarLog(Constants.ObtenerCategoriaServicio, "I", $"Error inesperado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorInesperado;
                response.CategoriasServicio = [];
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet("tipos-servicios")]
        public async Task<IActionResult> ObtenerTiposServicio([FromQuery] int servicio)
        {
            var response = new TipoServicioResponseDto();

            try
            {
                using IDbConnection dbConnection = DbConnection;
                dbConnection.Open();
                using var command = new OracleCommand(Constants.ObtenerTipoServicio, (OracleConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("IN_SERVICIO", OracleDbType.Int32).Value = servicio;

                command.Parameters.Add("OUT_TIPO_SERVICIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_ESTATUS", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_MENSAJE", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                response.Estatus = int.Parse(command.Parameters["OUT_ESTATUS"].Value.ToString()!);
                response.Mensaje = command.Parameters["OUT_MENSAJE"].Value.ToString()!;

                DatosObjetoConsumido = $@"
                        BEGIN
                            {Constants.ObtenerTipoServicio} (
                                IN_SERVICIO => {servicio},
                                OUT_TIPO_SERVICIO => :OUT_TIPO_SERVICIO,
                                OUT_ESTATUS => {response.Estatus},
                                OUT_MENSAJE => '{response.Mensaje}'
                            );
                        END;";

                if (response.Estatus == 0)
                {
                    using var reader = ((OracleRefCursor)command.Parameters["OUT_TIPO_SERVICIO"].Value).GetDataReader();
                    while (reader.Read())
                    {
                        response.TiposServicios.Add(new TipoServicioDto
                        {
                            TipoCobertura = reader.GetInt32(0),
                            Descripcion = reader.GetString(1)
                        });
                    }
                }
            }
            catch (OracleException ex)
            {
                logService.RegistrarLog(Constants.ObtenerTipoServicio, "I", $"Error al ejecutar el procedimiento almacenado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorLlamandoPlsql;
                response.TiposServicios = [];
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                logService.RegistrarLog(Constants.ObtenerTipoServicio, "I", $"Error inesperado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorInesperado;
                response.TiposServicios = [];
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet("validar-cobertura")]
        public async Task<IActionResult> ValidarCobertura([FromQuery] int servicio, [FromQuery] int tipoCobertura, [FromQuery] string cobertura)
        {
            var response = new ValidarCoberturaResponseDto();

            try
            {
                using IDbConnection dbConnection = DbConnection;
                dbConnection.Open();

                using var command = new OracleCommand(Constants.ObtenerCoberturaSalud, (OracleConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("IN_SERVICIO", OracleDbType.Int32).Value = servicio;
                command.Parameters.Add("IN_TIPO_COBERTURA", OracleDbType.Int32).Value = tipoCobertura;
                command.Parameters.Add("IN_COBERTURA", OracleDbType.Varchar2).Value = cobertura;

                command.Parameters.Add("OUT_DESCRIPCION_CPT", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_ESTATUS", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_MENSAJE", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_SERVICIO_TIPO_COBERTURA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                response.Estatus = int.Parse(command.Parameters["OUT_ESTATUS"].Value.ToString()!);
                response.Mensaje = command.Parameters["OUT_MENSAJE"].Value?.ToString() ?? string.Empty;
                response.DescripcionCPT = command.Parameters["OUT_DESCRIPCION_CPT"].Value?.ToString() ?? string.Empty;

                DatosObjetoConsumido = $@"
                        BEGIN
                            {Constants.ObtenerCoberturaSalud} (
                                IN_SERVICIO => {servicio},
                                IN_TIPO_COBERTURA => {tipoCobertura},
                                IN_COBERTURA => '{cobertura}',
                                OUT_DESCRIPCION_CPT => '{response.DescripcionCPT}',
                                OUT_ESTATUS => {response.Estatus},
                                OUT_MENSAJE => '{response.Mensaje}',
                                OUT_SERVICIO_TIPO_COBERTURA => :OUT_SERVICIO_TIPO_COBERTURA
                            );
                        END;";

                if (response.Estatus == 0)
                {
                    if (response.DescripcionCPT.Equals("null"))
                    {
                        response.ServiciosTiposCobertura = [];

                        using (var reader = ((OracleRefCursor)command.Parameters["OUT_SERVICIO_TIPO_COBERTURA"].Value).GetDataReader())
                        {
                            while (reader.Read())
                            {
                                response.ServiciosTiposCobertura.Add(new ServicioTipoCoberturaDto
                                {
                                    Servicio = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0)),
                                    DescripcionServicio = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    TipoCobertura = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                                    DescripcionTipoCobertura = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                                });
                            }
                        }

                        response.Mensaje = "CPT NO EXISTE EN LA CATEGORIA Y TIPO DE SERVICIO SELECCIONADO, PERO SI EN ESTAS DEL LISTADO.";
                    }
                    else
                    {
                        response.ServiciosTiposCobertura = [];
                        response.Mensaje = "OK";
                    }
                }
            }
            catch (OracleException ex)
            {
                logService.RegistrarLog(Constants.ObtenerCoberturaSalud, "I", $"Error al ejecutar el procedimiento almacenado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorLlamandoPlsql;
                response.ServiciosTiposCobertura = [];
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                logService.RegistrarLog(Constants.ObtenerCoberturaSalud, "I", $"Error inesperado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorInesperado;
                response.ServiciosTiposCobertura = [];
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet("someter-cobertura")]
        public async Task<IActionResult> CoberturaSometer([FromQuery] int servicio, [FromQuery] int tipoCobertura, [FromQuery] string cobertura, [FromQuery] decimal valorProveedorFueraRed)
        {
            var response = new SometerCoberturaResponseDto();
            try
            {
                using IDbConnection dbConnection = DbConnection;
                dbConnection.Open();

                using var command = new OracleCommand(Constants.SometerCobertura, (OracleConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("IN_SERVICIO", OracleDbType.Int32).Value = servicio;
                command.Parameters.Add("IN_TIPO_COBERTURA", OracleDbType.Int32).Value = tipoCobertura;
                command.Parameters.Add("IN_COBERTURA", OracleDbType.Varchar2).Value = cobertura;
                command.Parameters.Add("IN_VALOR_PROVEEDOR_FUERA_RED", OracleDbType.Decimal).Value = valorProveedorFueraRed;

                command.Parameters.Add("OUT_MATRIZ_COBERTURA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_REEMBOLSO_PROV_FUERA_RED", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_COSTO_PROVEEDOR_FUERA_RED", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_ESTATUS", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_MENSAJE", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                response.Estatus = int.Parse(command.Parameters["OUT_ESTATUS"].Value.ToString()!);
                response.Mensaje = command.Parameters["OUT_MENSAJE"].Value.ToString()!;

                if (response.Estatus == 0)
                {
                    var matrizCobertura = new List<MatrizCoberturaDto>();

                    if (command.Parameters["OUT_MATRIZ_COBERTURA"].Value is OracleRefCursor refCursor)
                    {
                        using var reader = refCursor.GetDataReader();
                        while (await reader.ReadAsync())
                        {
                            matrizCobertura.Add(new MatrizCoberturaDto
                            {
                                Tipo = reader.GetString(0),
                                Minimo = reader.GetDecimal(1),
                                Maximo = reader.GetDecimal(2),
                                Average = reader.GetDecimal(3)
                            });
                        }
                    }

                    response.MatrizCobertura = matrizCobertura;

                    response.ReembolsoProveedorFueraRed = command.Parameters["OUT_REEMBOLSO_PROV_FUERA_RED"].Value?.ToString()!;
                    response.CostoProveedorFueraRed = command.Parameters["OUT_COSTO_PROVEEDOR_FUERA_RED"].Value?.ToString()!;
                }
                else
                {
                    response.ReembolsoProveedorFueraRed = string.Empty;
                    response.CostoProveedorFueraRed = string.Empty;
                    response.MatrizCobertura = [];
                }

                DatosObjetoConsumido = $@"
                        BEGIN
                            {Constants.SometerCobertura} (
                                IN_SERVICIO => {servicio},
                                IN_TIPO_COBERTURA => {tipoCobertura},
                                IN_COBERTURA => '{cobertura}',
                                IN_VALOR_PROVEEDOR_FUERA_RED => {valorProveedorFueraRed},
                                OUT_MATRIZ_COBERTURA => :OUT_MATRIZ_COBERTURA,
                                OUT_REEMBOLSO_PROV_FUERA_RED => '{response.ReembolsoProveedorFueraRed}',
                                OUT_COSTO_PROVEEDOR_FUERA_RED => '{response.CostoProveedorFueraRed}',
                                OUT_ESTATUS => {response.Estatus},
                                OUT_MENSAJE => '{response.Mensaje}'
                            );
                        END;";
            }
            catch (OracleException ex)
            {
                logService.RegistrarLog(Constants.SometerCobertura, "I", $"Error al ejecutar el procedimiento almacenado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorLlamandoPlsql;
                response.MatrizCobertura = [];
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                logService.RegistrarLog(Constants.SometerCobertura, "I", $"Error inesperado: {ex.Message} {DatosObjetoConsumido}", out transaccionId);
                response.Estatus = 1;
                response.Mensaje = Constants.ErrorInesperado;
                response.MatrizCobertura = [];
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet("probar-conexion")]
        public IActionResult ProbarConexion()
        {
            try
            {
                using IDbConnection dbConnection = DbConnection;
                dbConnection.Open();
                return Ok("Conexión a la base de datos exitosa.");
            }
            catch (OracleException ex)
            {
                return StatusCode(500, $"Error al conectar con Oracle: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }
    }
}