namespace Package.Payments.VnPay;

public class VnPayOptions
{
    public string Version { get; set; } = null!;
    
    public string TmnCode { get; set; } = null!;
    
    public string HashSecret { get; set; } = null!;

    public string CurrCode { get; set; } = null!;
    
    public string PaymentUrl  { get; set; } = null!;
    
    public string ReturnUrl { get; set; } = null!;
}