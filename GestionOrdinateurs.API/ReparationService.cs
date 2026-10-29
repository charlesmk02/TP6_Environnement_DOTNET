using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public record HistoriqueReparationRecord(
    int OrdinateurId,
    string NomOrdinateur,
    string Technicien,
    string TypeReparation,
    DateTime DateIntervention
);

public class ReparationService : IReparationService
{
    private readonly ParcInformatiqueContext _context;
    // On utilise un sémaphore pour synchroniser l'accès au DbContext
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IJournalService _journalService;
    
    public ReparationService(ParcInformatiqueContext context, IJournalService journalService)
    {
        _context = context;
        _journalService = journalService;
    }

    /*
    TP 3 : Implémentation des méthodes de l'interface IReparationService
    */

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = new Reparation
        {
            UnOrdinateur = ordinateur,
            TypeReparation = "Maintenance standard",
            Technicien = "Jean Dupont",
            DateDebut = DateTime.Now,
            Statut = "En cours"
        };

        // Accès synchronisé au DbContext
        await _semaphore.WaitAsync();
        try
        {
            _context.Reparations.Add(reparation);
            await _context.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }

        // délai
        await Task.Delay(2000);

        await _semaphore.WaitAsync();
        try
        {
            reparation.Statut = "Terminée";
            // TP4.etape21 : on écrit dans les logs
            string message = $"{DateTime.Now:dd/MM/yyyy} - {ordinateur.Nom} réparé";
            _journalService.Ecrire(message);
            Console.WriteLine($"{ordinateur.Nom} réparé");
            await _context.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }

        Console.WriteLine($"{ordinateur.Nom} réparé");
    }

    public async Task<IReadOnlyList<Reparation>> ListerReparationsAsync()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .ToListAsync();
    }

    // TP4.etape 9 : créer une méthode qui renvoi l'historique des réparations dans le record précédent.
    public async Task<IReadOnlyList<HistoriqueReparationRecord>> ObtenirHistoriqueAsync()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .Select(r => new HistoriqueReparationRecord(
                r.UnOrdinateur.Id,
                r.UnOrdinateur.Nom,
                r.Technicien,
                r.TypeReparation,
                r.DateDebut
            ))
            .ToListAsync();
    }   
}