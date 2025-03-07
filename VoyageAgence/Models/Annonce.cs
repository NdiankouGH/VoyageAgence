using System.ComponentModel.DataAnnotations;

namespace VoyageAgence.Models
{
    public class Annonce
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La description est obligatoire")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire")]
        [StringLength(50)]
        public string Statut { get; set; }


        [Required(ErrorMessage = "Le prix est obligatoire")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif")]
        public decimal Prix { get; set; }



        [Required]
        public DateTime DateDepart { get; set; }

        [Required]
        public DateTime DateArrivee { get; set; }



        [Required]
        [StringLength(100)]
        public string LocaliteDepart { get; set; }

        public int? FlotteId { get; set; }
        public virtual Flotte Flotte { get; set; }
    }
}