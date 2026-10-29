using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public class OrdinateurService : IOrdinateurService
{
    private readonly ParcInformatiqueContext _context;

    // Déclaration des événements pour l'ajout, la suppression et la mise à jour d'un ordinateur
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;
    private readonly IJournalService _journalService;

    public OrdinateurService(ParcInformatiqueContext context, IJournalService journalService)
    {
        _context = context;
        _journalService = journalService;
    }

    /*
    TP 3 : Implémentation des méthodes de l'interface IOrdinateurService pour gérer les ordinateurs dans la base de données.
    Chaque méthode utilise EF Core pour interagir avec la base de données et déclenche les événements correspondants
    Les méthodes sont asynchrones comme demandé.
    */

    // Lister les ordinateurs GET
    public async Task<IReadOnlyList<Ordinateur>> ListerOrdinateurs()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    public async Task<Ordinateur?> ObtenirOrdinateurParId(int id)
    {
        return await _context.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);
    }
    // Ajouter un ordinateur POST
    public async Task<Ordinateur> AjouterOrdinateur(Ordinateur ordinateur)
    {
        ordinateur.Id = 0;
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
        // écriture dans les logs
        string message = $"{DateTime.Now:dd/MM/yyyy} - {ordinateur.Nom} ajouté";
        _journalService.Ecrire(message, "Logs/Ordinateurs/ordinateurs.log");

        return ordinateur;
    }
    // Mettre à jour un ordinateur PUT
    public async Task<Ordinateur?> MettreAJourOrdinateur(int id, Ordinateur ordinateurMisAJour)
    {
        Ordinateur? ordinateurExistant = await ObtenirOrdinateurParId(id);
        if (ordinateurExistant is null)
        {
            return null;
        }
        
        ordinateurExistant.Nom = ordinateurMisAJour.Nom;
        ordinateurExistant.Etat = ordinateurMisAJour.Etat;
        ordinateurExistant.Stock = ordinateurMisAJour.Stock;
        ordinateurExistant.Prix = ordinateurMisAJour.Prix;
        ordinateurExistant.Caracteristiques = ordinateurMisAJour.Caracteristiques;

        await _context.SaveChangesAsync();
        // Déclenchement de l'événement de mise à jour d'un ordinateur
        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(ordinateurExistant));
        // écriture dans les logs
        string message = $"{DateTime.Now:dd/MM/yyyy} - {ordinateurExistant.Nom} modifié";
        _journalService.Ecrire(message, "Logs/Ordinateurs/ordinateurs.log");
        return ordinateurExistant;
    }
    // Supprimer un ordinateur DELETE
    public async Task<bool> SupprimerOrdinateur(int id)
    {
        Ordinateur? ordinateur = await ObtenirOrdinateurParId(id);
        
        if (ordinateur is null) { return false; }

        _context.Ordinateurs.Remove(ordinateur);
        await _context.SaveChangesAsync();
        // Déclenchement de l'événement après la suppression en base
        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(ordinateur));
        // écriture dans les logs
        string message = $"{DateTime.Now:dd/MM/yyyy} - {ordinateur.Nom} supprimé";
        _journalService.Ecrire(message, "Logs/Ordinateurs/ordinateurs.log");
        return true;
    }

    public async Task<IReadOnlyList<Ordinateur>> ListerOrdinateursDisponibles() {
        return await _context.Ordinateurs
        .Where(o => o.Stock > 0)
        .ToListAsync();
    }

    // TP4.étape 3 : Refactoring 
    public async Task<IReadOnlyList<Ordinateur>> RechercherParMarque(string marque)
    {
        return await _context.Ordinateurs
        .Where(o => o.Nom != null && o.Nom.ToLower().Contains(marque.ToLower()))
        .ToListAsync();
    }

    public async Task<IReadOnlyList<Ordinateur>> ListerOrdinateursParPrixDecroissant(EtatOrdinateur etat)
    {
        return await _context.Ordinateurs
            .Where(o => o.Etat == etat)
            .OrderByDescending(o => o.Prix)
            .ToListAsync();
    }
    public async Task<IReadOnlyList<Ordinateur>> ListerOrdinateursParPrixDecroissant()
    {
        return await _context.Ordinateurs
            .OrderByDescending(o => o.Prix)
            .ToListAsync();
    }
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            {
                Etat: EtatOrdinateur.HS
            }
                => "Remplacement",

            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques:
                {
                    RamGo: < 8
                }
            }
                => "Ajouter RAM",

            _
                => "Aucune action"
        };
    }
    // retour des statistiques sur les ordinateurs et les réparations avec un tuple TP4.étape15
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques()
    {
        int totalOrdinateur = _context.Ordinateurs.Count();

        int ordiDisponibles = _context.Ordinateurs.Count(o => o.Stock > 0);

        decimal valeurStock = _context.Ordinateurs.Sum(o => o.Prix * o.Stock);

        int nbReparations = _context.Reparations.Count();

        return (totalOrdinateur, ordiDisponibles, valeurStock, nbReparations);
    }
}