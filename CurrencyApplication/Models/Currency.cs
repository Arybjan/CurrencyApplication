namespace CurrencyApp.Models;

public class Currency
{
    public string Id { get; set; } = string.Empty;
    public string CharCode { get; set; } = string.Empty;
    public int Nominal { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Previous { get; set; }
    public bool IsUserAdded { get; set; }
}