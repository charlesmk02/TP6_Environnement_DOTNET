using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

public interface IReparationService
{
    Task SimulerReparationAsync(Ordinateur ordinateur);
    Task<IReadOnlyList<Reparation>> ListerReparationsAsync();
    Task<IReadOnlyList<HistoriqueReparationRecord>> ObtenirHistoriqueAsync();
    
}