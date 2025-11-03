namespace CleanOrderAPI.Models
{
    public class ComunaModel
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public int RegionId { get; set; }

    }
    public class RegionModel
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
    }

}
