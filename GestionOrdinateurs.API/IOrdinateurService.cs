using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

public interface IOrdinateurService {
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();
    string Diagnostiquer(Ordinateur ordinateur);
    event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    // Signature des méthodes d'OrdninateurService
    Task<IReadOnlyList<Ordinateur>> ListerOrdinateurs();
    Task<Ordinateur?> ObtenirOrdinateurParId(int id);
    Task<Ordinateur> AjouterOrdinateur(Ordinateur ordinateur);
    Task<Ordinateur?> MettreAJourOrdinateur(int id, Ordinateur ordinateurMisAJour);
    Task<bool> SupprimerOrdinateur(int id);
    Task<IReadOnlyList<Ordinateur>> ListerOrdinateursDisponibles();
    Task <IReadOnlyList<Ordinateur>> RechercherParMarque(string marque);
    Task <IReadOnlyList<Ordinateur>> ListerOrdinateursParPrixDecroissant();
    
}