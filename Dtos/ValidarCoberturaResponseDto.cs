namespace CostoReembolsoAPI.Dtos
{
    public class ServicioTipoCoberturaDto
    {
        public int Servicio { get; set; }
        public string DescripcionServicio { get; set; }
        public int TipoCobertura { get; set; }
        public string DescripcionTipoCobertura { get; set; }
    }

    public class ValidarCoberturaResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; }
        public string DescripcionCPT { get; set; }
        public List<ServicioTipoCoberturaDto> ServiciosTiposCobertura { get; set; } = new List<ServicioTipoCoberturaDto>();
    }
}