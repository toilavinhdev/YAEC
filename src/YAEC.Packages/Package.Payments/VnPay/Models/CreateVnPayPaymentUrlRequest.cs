namespace Package.Payments.VnPay.Models;

public class CreateVnPayPaymentUrlRequest
{
    public string OrderCode { get; set; } = null!;
    
    public double Amount { get; set; }

    public string IpAddress { get; set; } = null!;
}