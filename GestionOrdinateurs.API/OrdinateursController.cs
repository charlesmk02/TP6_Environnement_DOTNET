using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("ordinateurs")]
[Authorize]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _ordinateurService;
    private readonly ILogger<OrdinateursController> _logger;
    private readonly IConfiguration _configuration;
    

    public OrdinateursController(
        // Injection des dépendances pour le service d'ordinateurs, le logger et la configuration
        IOrdinateurService ordinateurService,
        ILogger<OrdinateursController> logger,
        IConfiguration configuration)
    {
        _ordinateurService = ordinateurService;
        _logger = logger;
        _configuration = configuration;

        // Abonnement aux événements du service d'ordinateurs
        _ordinateurService.OrdinateurAjoute += AfficherMessageOrdinateurAjoute;
        _ordinateurService.OrdinateurSupprime += AfficherMessageOrdinateurSupprime;
        _ordinateurService.OrdinateurMisAJour += AfficherMessageOrdinateurMisAJour;
    }

    // Méthodes pour gérer les événements d'ajout, de suppression et de mise à jour d'un ordinateur
    private void AfficherMessageOrdinateurAjoute(object? sender, OrdinateurAjouteEventArgs e)
    {
        _logger.LogInformation("Ordinateur {OrdinateurId} ajouté.", e.Ordinateur.Id);
    }

    private void AfficherMessageOrdinateurSupprime(object? sender, OrdinateurSupprimeEventArgs e)
    {
        _logger.LogInformation("Ordinateur {OrdinateurId} supprimé.", e.Ordinateur.Id);
    }

    private void AfficherMessageOrdinateurMisAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        _logger.LogInformation("Ordinateur {OrdinateurId} mis à jour.", e.Ordinateur.Id);
    }

    // GET: /ordinateurs lister tous les ordinateurs
    [HttpGet]
    public async Task<IActionResult> ListerOrdinateurs()
    {
        _logger.LogInformation("Récupération de la liste des ordinateurs.");
        return Ok(await _ordinateurService.ListerOrdinateurs());
    }
    // GET: Infos sur l'application
    [HttpGet("/api/ordinateurs/info")]
    public IActionResult AfficherInfos()
    {
        _logger.LogInformation("Récupération des infos sur l'app.");
        var informations = new
        {
            Name = _configuration["Application:Name"],
            Version = _configuration["Application:Version"]
        };

        return Ok(informations);
    }
    // GET: /ordinateurs/{id} obtenir un ordinateur par son ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenirOrdinateurParId(int id)
    {
        _logger.LogInformation("Récupération de l'ordinateur par ID.");
        Ordinateur? ordinateur = await _ordinateurService.ObtenirOrdinateurParId(id);
        return ordinateur is null ? NotFound() : Ok(ordinateur);
    }

    // POST: /ordinateurs ajouter un ordinateur
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AjouterOrdinateur(Ordinateur ordinateur)
    {
        _logger.LogInformation("Ajout d'un nouvel ordinateur...");
        Ordinateur ordinateurAjoute = await _ordinateurService.AjouterOrdinateur(ordinateur);
        return CreatedAtAction(nameof(ObtenirOrdinateurParId), new { id = ordinateurAjoute.Id }, ordinateurAjoute);
    }
    // PUT: /ordinateurs/{id} mettre à jour un ordinateur
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> MettreAJourOrdinateur(int id, Ordinateur ordinateur)
    {
        _logger.LogInformation("Mise à jour de l'ordinateur.");
        Ordinateur? ordinateurMisAJour = await _ordinateurService.MettreAJourOrdinateur(id, ordinateur);
        return ordinateurMisAJour is null ? NotFound() : Ok(ordinateurMisAJour);
    }
    // DELETE: /ordinateurs/{id} supprimer un ordinateur
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> SupprimerOrdinateur(int id)
    {
        _logger.LogInformation("Suppression de l'ordinateur.");
        return await _ordinateurService.SupprimerOrdinateur(id) ? NoContent() : NotFound();
    }

    // GET: /ordinateurs/disponibles lister les ordinateurs disponibles
    [HttpGet("disponibles")]
    public async Task<IActionResult> ListerOrdinateursDisponibles()
    {
        _logger.LogInformation("Récupération de la liste des ordinateurs disponibles.");
        return Ok(await _ordinateurService.ListerOrdinateursDisponibles());
    }

    // GET: /ordinateurs/marque/{marque}
    [HttpGet("marque/{marque}")]
    public async Task<IActionResult> RechercherParMarque(string marque)
    {
        _logger.LogInformation("Recherche des ordinateurs de la marque : {Marque}", marque);
        var resultats = await _ordinateurService.RechercherParMarque(marque);
        return Ok(resultats);
    }

    // GET: /ordinateurs/tri-prix
    [HttpGet("tri-prix")]
    public async Task<IActionResult> ListerOrdinateursParPrixDecroissant()
    {
        _logger.LogInformation("Récupération des ordinateurs triés par prix décroissant.");
        var resultats = await _ordinateurService.ListerOrdinateursParPrixDecroissant();
        return Ok(resultats);
    }

    // GET: /ordinateurs/{id}/diagnostic
    [HttpGet("{id:int}/diagnostic")]
    public async Task<IActionResult> ObtenirDiagnostic(int id)
    {
        _logger.LogInformation("Diagnostic demandé pour l'ordinateur {Id}", id);

        var ordinateur = await _ordinateurService.ObtenirOrdinateurParId(id);
        if (ordinateur is null)
        {
            return NotFound($"Aucun ordinateur trouvé avec l'ID {id}.");
        }

        string resultat = _ordinateurService.Diagnostiquer(ordinateur);

        return Ok(resultat);
    }

    // GET: /ordinateurs/statistiques
    [HttpGet("statistiques")]
    public IActionResult ObtenirStatistiques()
    {
        _logger.LogInformation("Calcul et récupération des statistiques du parc...");

        // TP4.étape16 : récupération des statistiques
        var (total, enStock, valeurStock, nbReparations) = _ordinateurService.CalculerStatistiques();

        return Ok(new
        {
            TotalOrdinateurs = total,
            OrdinateursEnStock = enStock,
            ValeurTotaleStock = valeurStock,
            TotalReparations = nbReparations
        });
    }

    // GET: /ordinateurs/inventaire/{nom}
    // TP4.étape18 
    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> ChercherParNom(string nom)
    {
        _logger.LogInformation("Recherche dans l'inventaire via indexeur pour : {Nom}", nom);

        var ordinateurs = await _ordinateurService.ListerOrdinateurs();

        var inventaire = new InventaireOrdinateurs();
        inventaire.Ordinateurs = ordinateurs.ToList();

        var ordinateur = inventaire[nom];

        return ordinateur is null 
            ? NotFound($"Ordinateur '{nom}' non trouvé dans l'inventaire.") 
            : Ok(ordinateur);
    }
}