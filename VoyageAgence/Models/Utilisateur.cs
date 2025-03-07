using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VoyageAgence.Models
{
    public class Utilisateur : IdentityUser // Hérite de IdentityUser
    {
        [Required, StringLength(50)]
        [PersonalData]
        public string Nom { get; set; }

        [Required, StringLength(50)]
        [PersonalData]
        public string Prenom { get; set; }

        [Required, Phone]
        [PersonalData] // Marque la propriété comme des données personnelles (utile pour RGPD)
        public string NumTel { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}


