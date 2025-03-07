using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoyageAgence.Models
{

    [Table("Reservations")]
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        public string Statut { get; set; }

        public string ClientId { get; set; } // Changement de int à string

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public int VoyageId { get; set; }

        [ForeignKey("VoyageId")]
        public virtual Voyage Voyage { get; set; }
    }
}