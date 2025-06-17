namespace EncuestaAPI.Models.DTOs
{
    public class ListaEncuestaDTO
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public required List<string> Preguntas { get; set; }
    }
}
