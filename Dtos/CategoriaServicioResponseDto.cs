namespace CostoReembolsoAPI.Dtos
{
    public class CategoriaServicioDto
    {
        public int TipoServicio { get; set; }
        public string Descripcion { get; set; }
    }

    public class CategoriaServicioResponseDto
    {
        public int Estatus { get; set; }
        public string Mensaje { get; set; }
        public List<CategoriaServicioDto> CategoriasServicio { get; set; } = new List<CategoriaServicioDto>();
    }
}