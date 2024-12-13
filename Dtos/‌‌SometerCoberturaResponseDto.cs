namespace CostoReembolsoAPI.Dtos
{
    public class MatrizCoberturaDto
    {
        public string Tipo { get; set; } = string.Empty;
        public decimal Minimo { get; set; }
        public decimal Maximo { get; set; }
        public string Average { get; set; } = string.Empty;
    }

    public class SometerCoberturaResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int ReembolsoProveedorFueraRed { get; set; }
        public int CostoProveedorFueraRed { get; set; }
        public List<MatrizCoberturaDto> MatrizCobertura { get; set; } = [];
    }
}