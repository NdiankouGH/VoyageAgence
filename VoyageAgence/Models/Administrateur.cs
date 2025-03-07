namespace VoyageAgence.Models
{
    public class Administrateur : Utilisateur
    {
        public virtual ICollection<Utilisateur> UtilisateursGeres { get; set; } = new List<Utilisateur>();
    }
}