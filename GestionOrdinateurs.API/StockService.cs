namespace GestionOrdinateurs.API;

public class StockService : IStockService
{
    public int StockSSD { get; set; } = 10;

    private readonly object _stockLock = new();

    public void ConsommerSSD()
    {

        // TP3.étape17
        // sans verrou
        //StockSSD--;

        // TP3.étape19 : avec verrou
        lock (_stockLock)
        {
            StockSSD--;
        }
    }
}