using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;
public enum EtatOrdinateur
{
    EnService,
    EnMarche = EnService,
    EnPanne,
    EnMaintenance,
    // TP4.étape12 : mise à jour de l'énum
    HorsService,
    HS = HorsService,   
    Lent,
    Reparable
}


[Table("Ordinateur")]
public class Ordinateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public EtatOrdinateur Etat { get; set; }

    [DefaultValue(0)]
    public int Stock { get; set; } = 0;
    [DefaultValue(0)]
    public decimal Prix { get; set; } = 0;
    
    [DefaultValue(false)]
    public bool Vendu { get; set; } = false;

    public Caracteristiques Caracteristiques { get; set; } = null!;
    public List<Reparation> Reparations { get; set; } = new();

}
