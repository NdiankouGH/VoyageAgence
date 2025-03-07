using System.ComponentModel.DataAnnotations;

namespace VoyageAgence.Models
{
    public class Client : Utilisateur
    {
        [Required]
        [StringLength(200)]
        public string Adresse { get; set; }

        [Required]
        [StringLength(20)]
        public string NumPasseport { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }


}

