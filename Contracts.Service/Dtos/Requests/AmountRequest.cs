
namespace Cmas.Services.Contracts.Dtos.Requests
{
    /// <summary>
    /// Стоимость договора
    /// </summary>
    public class AmountRequest
    {
        // Валюта
        public string CurrencySysName;

        // значение
        public double Value = 0;
    }
}
