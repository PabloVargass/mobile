namespace CleanOrderAPI.Models
{
    public class DisponibilidadRequest
    {
        public required DateTime FechaAgendada { get; set; }
        public required int HorasTrabajo { get; set; }
    }
}
