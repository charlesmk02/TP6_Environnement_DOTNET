using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.Service;

public class OrdinateurServiceTest : IDisposable
{
    private readonly ParcInformatiqueContext _context;
    private readonly OrdinateurService _ordinateurService;

    private ParcInformatiqueContext CreerContexte() {

        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ParcInformatiqueContext(options);
        context.Ordinateurs.AddRange(
            new Ordinateur { Id = 1, Nom = "Dell XPS", Etat = EtatOrdinateur.EnMarche, Prix = 1000m, Stock = 5, Caracteristiques = new Caracteristiques(8, 256, TypeOrdinateur.Portable) },
            new Ordinateur { Id = 2, Nom = "HP Elite", Etat = EtatOrdinateur.EnPanne, Prix = 1200m, Stock = 3, Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Fixe) },
            new Ordinateur { Id = 3, Nom = "Lenovo ThinkPad", Etat = EtatOrdinateur.EnMaintenance, Prix = 1500m, Stock = 0, Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable) },
            new Ordinateur { Id = 4, Nom = "Asus ROG", Etat = EtatOrdinateur.EnMarche, Prix = 1800m, Stock = 1, Caracteristiques = new Caracteristiques(32, 1024, TypeOrdinateur.Fixe) },
            new Ordinateur { Id = 5, Nom = "Apple MacBook", Etat = EtatOrdinateur.EnMarche, Prix = 2000m, Stock = 2, Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable) }
        );
    
    context.SaveChanges();
    
    return context;
}

    public OrdinateurServiceTest()
    {
        _context = CreerContexte();
        _context.Database.EnsureCreated();
        // T4.étape23 : obligé d'injecter la dépendance IJournalService pour que le test n'échoue pas
        _ordinateurService = new OrdinateurService(_context, new JournalService());  
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // Test pour vérifier l'ajout d'un ordinateur

    [Fact]
    public async Task AjouterOrdinateur_AjouterUnNouvelOrdinateur()
    {
        // Arrange
        var newOrdinateur = new Ordinateur 
        { 
            Nom = "Dell Latitude", 
            Etat = EtatOrdinateur.EnMarche,
            Stock = 10,
            Prix = 1400m,
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable)
        };

        Ordinateur? ordinateurNotifie = null;
        _ordinateurService.OrdinateurAjoute += (_, args) => ordinateurNotifie = args.Ordinateur;
        // Act
        var result = await _ordinateurService.AjouterOrdinateur(newOrdinateur);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dell Latitude", result.Nom);
        Assert.Equal(1400m, result.Prix);
        // Ajout des propriétés Stock et Prix pour le test (TP3.étape 9)
        Assert.Equal(10, result.Stock);
        Assert.Equal(1400, result.Prix);
        Assert.Same(result, ordinateurNotifie);
        Assert.Equal(6, (await _ordinateurService.ListerOrdinateurs()).Count());
    }
    
// Test pour vérifier la suppression d'un ordinateur existant

    [Fact]
    public async Task SupprimerOrdinateur_SupprimerUnOrdinateurExistant()
    {
        // Arrange
        int id = 1;
        Ordinateur? ordinateurASupprimer = await _ordinateurService.ObtenirOrdinateurParId(id)!;
        Ordinateur? ordinateurNotifie = null;
        _ordinateurService.OrdinateurSupprime += (_, args) => ordinateurNotifie = args.Ordinateur;

        // Act
        var result = await _ordinateurService.SupprimerOrdinateur(id);

        // Assert
        Assert.True(result); // SupprimerOrdinateur devrait retourner true pour un ordinateur existant
        Assert.Same(ordinateurASupprimer, ordinateurNotifie);
        Assert.Equal(4, (await _ordinateurService.ListerOrdinateurs()).Count()); // Vérifie que le nombre d'ordinateurs a diminué de 1
        Assert.Null(await _ordinateurService.ObtenirOrdinateurParId(id));
    }

// Test pour vérifier la suppression d'un ordinateur inexistant

    [Fact]
    public async Task SupprimerOrdinateur_SupprimerUnOrdinateurInexistant()
    {
        // Arrange
        int id = 999;

        // Act
        var result = await _ordinateurService.SupprimerOrdinateur(id);

        // Assert
        Assert.False(result); // SupprimerOrdinateur devrait retourner false pour un ordinateur inexistant  
        Assert.Equal(5, (await _ordinateurService.ListerOrdinateurs()).Count());
    }
}