namespace CleanOrderAPI.Models
{
    public class DocumentoModel
    {
        public required int idDocumento { get; set; }
        public required string nombre { get; set; }
        public required DateTime fechaSubida { get; set; }
        public required string tipoMime { get; set; }
        public required string archivo { get; set; }          // Base64
        public required long tamanoBytes { get; set; }
        public required string RutCliente { get; set; }        // Consistent with controller
    }

    public class DocumentoSinArchivoModel
    {
        public required int idDocumento { get; set; }
        public required string nombre { get; set; }
        public required DateTime fechaSubida { get; set; }
        public required string tipoMime { get; set; }
        public required long tamanoBytes { get; set; }
        public required string RutCliente { get; set; }
    }
}
