using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("reparations")]
[Authorize]
public class ReparationController : ControllerBase
{
    private readonly IReparationService _reparService;
    private readonly IOrdinateurService _ordinateurService;
    // Service de gestion du stock de SSD (TP3.étape17)
    private readonly IStockService _stockService;
    private readonly ILogger<ReparationController> _logger;

    public ReparationController(
        IReparationService reparService,
        IOrdinateurService ordinateurService,
        IStockService stockService,
        ILogger<ReparationController> logger)
    {
        _reparService = reparService;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
        _logger = logger;
    }

    // POST: /reparations/simuler
    [HttpPost("simuler")]
    [Authorize]
    public async Task<IActionResult> SimulerReparation()
    {
        _logger.LogInformation("Démarrage de la simulation de réparations simultanées.");

        var listOrdinateurs = await _ordinateurService.ListerOrdinateurs();

        if (listOrdinateurs.Count < 5)
        {
            return BadRequest("Il faut au moins 5 ordinateurs enregistrés pour lancer la simulation.");
        }

        // simu
        await Task.WhenAll(
            _reparService.SimulerReparationAsync(listOrdinateurs[0]),
            _reparService.SimulerReparationAsync(listOrdinateurs[1]),
            _reparService.SimulerReparationAsync(listOrdinateurs[2]),
            _reparService.SimulerReparationAsync(listOrdinateurs[3]),
            _reparService.SimulerReparationAsync(listOrdinateurs[4])
        );

        return Ok("Les 5 réparations ont été simulées avec succès en parallèle.");
    }

    // GET: /reparations
    [HttpGet]
    public async Task<IActionResult> ListerReparations()
    {
        _logger.LogInformation("Récupération de la liste des réparations avec les détails de l'ordinateur.");
        var reparations = await _reparService.ListerReparationsAsync();
        return Ok(reparations);
    }

    // GET: /reparations/stock-ssd
    [HttpGet("stock-ssd")]
    public IActionResult GetStockSSD()
    {

        // éxécution de 10 tâches en parallèle
        Parallel.For(
            0,
            10,
            i => {
                Task.Delay(10).Wait();
                _stockService.ConsommerSSD();
            });

        _logger.LogInformation("Stock SSD restant après décrémentation concurrente : {StockSSD}", _stockService.StockSSD);

        /* TP3.étape17 : 
            Quel constat faites vous ?
                La valeur retournée n'est pas égale à 0
            Le résultat est-il cohérent ? et pourquoi ?
                Non, il est incohérent car 10 consommations sur un stock initial de 10 devraient donner un stock final = 0.
                On a affaire à une condition de course.
        */

        /* TP3.étape20 :
            Relancer notre endpoint "GetStockSSD" que constatez vous ? 
                Le résultat est maintenant cohérent, le stock final est bien égal à 0.
        */

        return Ok(_stockService.StockSSD);
    }

    // GET: reparations/historique
    [HttpGet("historique")]
    public async Task<IActionResult> ObtenirHistorique()
    {
        _logger.LogInformation("Récupération de l'historique des réparations au format Record.");

        var historique = await _reparService.ObtenirHistoriqueAsync();

        return Ok(historique);
    }


}