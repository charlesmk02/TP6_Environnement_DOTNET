using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.Tests;
//TP4.étape23 :  TEST pour le service REPARATION
public class ReparationServiceTest : IDisposable
{
    private readonly ParcInformatiqueContext _context;
    private readonly ReparationService _reparationService;

    public ReparationServiceTest()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ParcInformatiqueContext(options);
        _context.Database.EnsureCreated();


        _reparationService = new ReparationService(_context, new JournalService());
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SimulerReparation_CreeEtPersisteUneReparation()
    {
        // Arrange
        var ordinateur = new Ordinateur
        {
            Nom = "Dell Latitude 5520",
            Etat = EtatOrdinateur.EnPanne,
            Prix = 1200m,
            Stock = 2,
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable)
        };
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();

        // Act
        await _reparationService.SimulerReparationAsync(ordinateur);

        // Assert
        var reparations = await _reparationService.ListerReparationsAsync();

        Assert.Single(reparations);
        Assert.Equal("Terminée", reparations[0].Statut);
        Assert.Equal("Jean Dupont", reparations[0].Technicien);
        Assert.Equal("Dell Latitude 5520", reparations[0].UnOrdinateur.Nom);
    }
}