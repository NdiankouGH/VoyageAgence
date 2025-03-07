using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoyageAgence.Models
{
    [Table("Voyages")]
    public class Voyage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        // ...existing code...
        [Required]
        public DateTime DateDepart { get; set; }

        [Required]
        public DateTime DateRetour { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Prix { get; set; }

        public int ChauffeurId { get; set; }

        [ForeignKey("ChauffeurId")]
        public virtual Chauffeur Chauffeur { get; set; }

        public int FlotteId { get; set; }

        [ForeignKey("FlotteId")]
        public virtual Flotte Flotte { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
