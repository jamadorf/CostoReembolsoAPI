namespace CostoReembolsoAPI.Dtos
{
    public class MatrizCoberturaDto
    {
        public string Tipo { get; set; }
        public decimal Minimo { get; set; }
        public decimal Maximo { get; set; }
        public decimal Average { get; set; }
    }

    public class SometerCoberturaResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; }
        public decimal ReembolsoProveedorFueraRed { get; set; }
        public decimal CostoProveedorFueraRed { get; set; }
        public List<MatrizCoberturaDto> MatrizCobertura { get; set; }
    }
}