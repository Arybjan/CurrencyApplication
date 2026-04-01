using System.Collections.Generic;

namespace CurrencyApp.Models
{
    public class CbrResponse
    {
        public Dictionary<string, CbrCurrency> Valute { get; set; }
    }

    public class CbrCurrency
    {
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Previous { get; set; }
    }
}