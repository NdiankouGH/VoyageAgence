using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoyageAgence.Models
{
    [Table("Chauffeurs")]
    public class Chauffeur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        public bool EstDisponible { get; set; } = true;

        public virtual ICollection<Voyage> Voyages { get; set; } = new List<Voyage>();
    }
}