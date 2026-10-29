namespace GestionOrdinateurs.Shared;

public class InventaireOrdinateurs
{
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    // Constructeurs
    public InventaireOrdinateurs()
    {
    }

    public InventaireOrdinateurs(IEnumerable<Ordinateur> ordinateurs)
    {
        Ordinateurs = ordinateurs.ToList();
    }

    public Ordinateur? this[int id]
    {
        get => Ordinateurs.FirstOrDefault(o => o.Id == id);
    }

    public Ordinateur? this[string reference]
    {
        get => Ordinateurs.FirstOrDefault(o => 
            string.Equals(o.Nom, reference, StringComparison.OrdinalIgnoreCase));
    }
}