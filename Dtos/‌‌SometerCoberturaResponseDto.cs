namespace CostoReembolsoAPI.Dtos
{
    public class SometerCoberturaResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public decimal LimiteMinimo { get; set; }
        public decimal LimiteMaximo { get; set; }
        public decimal LimitePromedio { get; set; }
        public decimal CopagoMinimo { get; set; }
        public decimal CopagoMaximo { get; set; }
        public decimal CopagoPromedio { get; set; }
        public decimal PorcientoCoaseguroMinimo { get; set; }
        public decimal PorcientoCoaseguroMaximo { get; set; }
        public decimal PorcientoCoaseguroPromedio { get; set; }
        public decimal MontoCoaseguroMinimo { get; set; }
        public decimal MontoCoaseguroMaximo { get; set; }
        public decimal MontoCoaseguroPromedio { get; set; }
        public decimal ReembolsoProveedorFueraRed { get; set; }
        public decimal CostoProveedorFueraRed { get; set; }
    }
}