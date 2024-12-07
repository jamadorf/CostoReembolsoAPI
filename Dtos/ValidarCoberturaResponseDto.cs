namespace CostoReembolsoAPI.Dtos
{
    public class ServicioTipoCoberturaDto
    {
        public int Servicio { get; set; }
        public string DescripcionServicio { get; set; } = string.Empty;
        public int TipoCobertura { get; set; }
        public string DescripcionTipoCobertura { get; set; } = string.Empty;
    }

    public class ValidarCoberturaResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string DescripcionCPT { get; set; } = string.Empty;
        public List<ServicioTipoCoberturaDto> ServiciosTiposCobertura { get; set; } = [];
    }
}