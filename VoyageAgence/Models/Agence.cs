using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoyageAgence.Models
{
    [Table("Agences")]
    public class Agence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Adresse { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

        [Required]
        [StringLength(50)]
        public string RCCM { get; set; }

        [Required]
        [StringLength(20)]
        public string Ninea { get; set; }

        [Range(0, 5)]
        public int Notes { get; set; }

        public String GestionnaireId { get; set; }

        [ForeignKey("GestionnaireId")]
        public virtual Gestionnaire Gestionnaire { get; set; }

        public virtual ICollection<Offre> Offres { get; set; } = new List<Offre>();
    }
}