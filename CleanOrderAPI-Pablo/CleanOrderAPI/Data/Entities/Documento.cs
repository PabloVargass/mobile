using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanOrderAPI.Data.Entities;

[Table("documento")]
public partial class Documento
{
    [Key]
    [Column("ID_DOCUMENTO")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdDocumento { get; set; }

    [Column("NOMBRE")]
    [MaxLength(50)]
    [Required]
    public string Nombre { get; set; } = null!;

    [Column("FECHA_SUBIDA")]
    [Required]
    public DateTime FechaSubida { get; set; }

    [Column("TIPO_MIME")]
    [MaxLength(50)]
    [Required]
    public string TipoMime { get; set; } = null!;

    [Column("ARCHIVO", TypeName = "LONGBLOB")]
    [Required]
    public byte[] Archivo { get; set; } = null!;

    [Column("TAMANO_BYTES")]
    [Required]
    public int TamanoBytes { get; set; }

    [Column("FK_RUT_CLIENTE")]
    [MaxLength(10)]
    [Required]
    public string FkRutCliente { get; set; } = null!;

    public virtual Cliente FkRutClienteNavigation { get; set; } = null!;
}
