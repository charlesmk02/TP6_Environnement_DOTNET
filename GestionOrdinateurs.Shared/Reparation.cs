namespace GestionOrdinateurs.Shared;

public class Reparation
{
    public int Id { get; set; }
    public Ordinateur UnOrdinateur { get; set; } = null!;    public string? Description { get; set; }
    public string TypeReparation { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public string Statut { get; set; } = string.Empty;
    // TP4.étape6
    public string Technicien { get; set; } = string.Empty;
}