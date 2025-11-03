using System.Text.Json.Serialization;

namespace CleanOrderAPI.Models
{
    public sealed class TipoCargaModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombreCarga")]
        public string? NombreCarga { get; set; }
    }
}
