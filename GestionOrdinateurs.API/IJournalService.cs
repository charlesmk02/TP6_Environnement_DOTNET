namespace GestionOrdinateurs.API;
public interface IJournalService
{
    void Ecrire(string message, string fichier = "Logs/Reparations/reparations.log");
    
}