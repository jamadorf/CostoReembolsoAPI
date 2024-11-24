using Microsoft.AspNetCore.Mvc;

namespace CostoReembolsoAPI.Dtos
{
    public class MatrizCoberturaDto
    {
        public string Tipo { get; set; } = string.Empty;
        public decimal Minimo { get; set; }
        public decimal Maximo { get; set; }
        public decimal Average { get; set; }
    }

    public class SometerCoberturaResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string ReembolsoProveedorFueraRed { get; set; } = string.Empty;
        public string CostoProveedorFueraRed { get; set; } = string.Empty;
        public List<MatrizCoberturaDto> MatrizCobertura { get; set; } = new List<MatrizCoberturaDto>();
    }
}