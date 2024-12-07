using CostoReembolsoAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace CostoReembolsoAPI.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class ApiController(IConfiguration configuration, ILogger<ApiController> logger) : ControllerBase
    {
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
                using var command = new OracleCommand("DBAPER.PKG_WEBSERVICES_PAG_AUX_MUTUO.P_OBTENER_CATEGORIA_SERVICIO", (OracleConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("OUT_CATEGORIA_SERVICIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_ESTATUS", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_MENSAJE", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                response.Estatus = int.Parse(command.Parameters["OUT_ESTATUS"].Value.ToString()!);
                response.Mensaje = command.Parameters["OUT_MENSAJE"].Value.ToString()!;

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
                logger.LogError($"Error al ejecutar el procedimiento almacenado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.CategoriasServicio = [];
                return Content(response.Mensaje);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error inesperado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.CategoriasServicio = [];
                return Content(response.Mensaje);
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
                using var command = new OracleCommand("DBAPER.PKG_WEBSERVICES_PAG_AUX_MUTUO.P_OBTENER_TIPO_SERVICIO", (OracleConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("IN_SERVICIO", OracleDbType.Int32).Value = servicio;

                command.Parameters.Add("OUT_TIPO_SERVICIO", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_ESTATUS", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_MENSAJE", OracleDbType.Varchar2, 200).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                response.Estatus = int.Parse(command.Parameters["OUT_ESTATUS"].Value.ToString()!);
                response.Mensaje = command.Parameters["OUT_MENSAJE"].Value.ToString()!;

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
                logger.LogError($"Error al ejecutar el procedimiento almacenado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.TiposServicios = [];
                return Content(response.Mensaje);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error inesperado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.TiposServicios = [];
                return Content(response.Mensaje);
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

                using var command = new OracleCommand("DBAPER.PKG_WEBSERVICES_PAG_AUX_MUTUO.P_OBTENER_COBERTURA_SALUD", (OracleConnection)dbConnection);
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
                    }
                }
            }
            catch (OracleException ex)
            {
                logger.LogError($"Error al ejecutar el procedimiento almacenado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.ServiciosTiposCobertura = [];
                return Content(response.Mensaje);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error inesperado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.ServiciosTiposCobertura = [];
                return Content(response.Mensaje);
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

                using var command = new OracleCommand("DBAPER.PKG_WEBSERVICES_PAG_AUX_MUTUO.P_SOMETER_COBERTURA", (OracleConnection)dbConnection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("IN_SERVICIO", OracleDbType.Int32).Value = servicio;
                command.Parameters.Add("IN_TIPO_COBERTURA", OracleDbType.Int32).Value = tipoCobertura;
                command.Parameters.Add("IN_COBERTURA", OracleDbType.Varchar2).Value = cobertura;
                command.Parameters.Add("IN_VALOR_PROVEEDOR_FUERA_RED", OracleDbType.Decimal).Value = valorProveedorFueraRed;

                command.Parameters.Add("OUT_MATRIZ_COBERTURA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_REEMBOLSO_PROV_FUERA_RED", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_COSTO_PROVEEDOR_FUERA_RED", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_ESTATUS", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("OUT_MENSAJE", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                response.Estatus = int.Parse(command.Parameters["OUT_ESTATUS"].Value.ToString()!);
                response.Mensaje = command.Parameters["OUT_MENSAJE"].Value.ToString()!;

                if (response.Estatus == 0)
                {
                    response.ReembolsoProveedorFueraRed = Convert.ToDecimal(command.Parameters["OUT_REEMBOLSO_PROV_FUERA_RED"].Value);
                    response.CostoProveedorFueraRed = Convert.ToDecimal(command.Parameters["OUT_COSTO_PROVEEDOR_FUERA_RED"].Value);

                    var matrizCobertura = new List<MatrizCoberturaDto>();

                    using (var reader = ((OracleRefCursor)command.Parameters["OUT_MATRIZ_COBERTURA"].Value).GetDataReader())
                    {
                        while (reader.Read())
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
                }
                else
                {
                    response.ReembolsoProveedorFueraRed = 0;
                    response.CostoProveedorFueraRed = 0;
                    response.MatrizCobertura = [];
                }
            }
            catch (OracleException ex)
            {
                logger.LogError($"Error al ejecutar el procedimiento almacenado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.MatrizCobertura = [];
                response.ReembolsoProveedorFueraRed = 0;
                response.CostoProveedorFueraRed = 0;
                return Content(response.Mensaje);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error inesperado: {ex.Message}");
                response.Estatus = 1;
                response.Mensaje = "HA OCURRIDO UN ERROR, FAVOR REVISAR LOS LOGS.";
                response.MatrizCobertura = [];
                response.ReembolsoProveedorFueraRed = 0;
                response.CostoProveedorFueraRed = 0;
                return Content(response.Mensaje);
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
                logger.LogError($"Error al conectar con Oracle: {ex.Message}");
                return StatusCode(500, $"Error al conectar con Oracle: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error inesperado: {ex.Message}");
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }
    }
}