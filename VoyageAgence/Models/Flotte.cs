using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoyageAgence.Models
{
    [Table("Flottes")]
    public class Flotte
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        [StringLength(20)]
        public string Matricule { get; set; }

        public bool EstDisponible { get; set; } = true;

        public virtual ICollection<Voyage> Voyages { get; set; } = new List<Voyage>();
    }
}