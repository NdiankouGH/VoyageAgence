using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoyageAgence.Models
{
    [Table("Offres")]
    public class Offre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Prix { get; set; }

        public bool Disponible { get; set; } = true;

        public int AgenceId { get; set; }

        [ForeignKey("AgenceId")]
        public virtual Agence Agence { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}