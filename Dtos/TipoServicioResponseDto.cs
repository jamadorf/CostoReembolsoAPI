namespace CostoReembolsoAPI.Dtos
{
    public class TipoServicioDto
    {
        public int TipoCobertura { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class TipoServicioResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<TipoServicioDto> TiposServicios { get; set; } = new List<TipoServicioDto>();
    }
}