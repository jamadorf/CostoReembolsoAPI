namespace CostoReembolsoAPI.Dtos
{
    public class TipoServicioDto
    {
        public int TipoCobertura { get; set; }
        public string Descripcion { get; set; }
    }

    public class TipoServicioResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; }
        public List<TipoServicioDto> TiposServicios { get; set; } = new List<TipoServicioDto>();
    }
}