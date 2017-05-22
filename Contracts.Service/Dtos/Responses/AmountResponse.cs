namespace Cmas.Services.Contracts.Dtos.Responses
{ 
    /// <summary>
    /// Стоимость договора
    /// </summary>
    public class AmountResponse
    {
        // Валюта
        public string CurrencySysName;

        // значение
        public double Value = 0;
    }
}
