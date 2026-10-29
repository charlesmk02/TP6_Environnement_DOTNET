namespace GestionOrdinateurs.API;

public interface IStockService
{
    int StockSSD { get; set; }
    void ConsommerSSD();
}