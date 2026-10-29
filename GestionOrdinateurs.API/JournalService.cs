namespace GestionOrdinateurs.API;

// TP4.étape20
public class JournalService : IJournalService
{
    // j'ai rajouté un paramètre pour le nom du fichier de log
    public void Ecrire(string message, string fichier = "Logs/Reparations/reparations.log")
    {
        // on crée le dossier s'il n'existe pas
        // --> amélioration nécessaire pour que ça s'écrive au bon endroit
        string? dossier = Path.GetDirectoryName(fichier);
        if (!string.IsNullOrEmpty(dossier))
        {
            Directory.CreateDirectory(dossier);
        }
        using var writer = new StreamWriter(fichier, append: true);
        writer.WriteLine(message);
    }
}