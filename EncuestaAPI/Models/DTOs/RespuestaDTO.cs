namespace EncuestaAPI.Models.DTOs
{
    public class RespuestaDTO
    {
        public int encuestaId { get; set; }
        public int aplicacionId { get; set; }
        public string? nombreRespondedor { get; set; }
        public string? numeroControl { get; set; }
        public List<RespuestaPreguntaDTO>? respuestas { get; set; }
    }
    public class RespuestaPreguntaDTO
    {
        public int preguntaId { get; set; }
        public int valor { get; set; }
    }
}
