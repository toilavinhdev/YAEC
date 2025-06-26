using System.Net;
using System.Security.Cryptography;
using System.Text;
using Package.Payments.VnPay;
using Package.Payments.VnPay.Models;

namespace Package.Payment.VnPay;

public interface IVnPayService
{
    string CreatePaymentUrl(CreateVnPayPaymentUrlRequest request);

    bool ValidateSignature(Dictionary<string, string> parameters);
}

public class VnPayService : IVnPayService
{
    private readonly VnPayOptions _options;
    
    public VnPayService(VnPayOptions options)
    {
        _options = options;
    }
    
    public string CreatePaymentUrl(CreateVnPayPaymentUrlRequest request)
    {
        var parameters = new SortedList<string, string?>(new VnPayParameterComparer())
        {
            { "vnp_Version", _options.Version },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", _options.TmnCode },
            { "vnp_Amount", $"{request.Amount * 100}" },
            { "vnp_BankCode", null },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", _options.CurrCode },
            { "vnp_IpAddr", request.IpAddress },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", $"Thanh toan hoa don ${request.OrderCode}. So tien {request.Amount} VND" },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", _options.ReturnUrl },
            { "vnp_ExpireDate", DateTime.Now.AddMinutes(5).ToString("yyyyMMddHHmmss") },
            { "vnp_TxnRef", DateTime.Now.Ticks.ToString() }
        };
        var queryString = BuildQueryString(parameters);
        var url = $"{_options.PaymentUrl}?{queryString}";
        var signature = ComputeHmacSha256(_options.HashSecret, queryString);
        url += $"&vnp_SecureHash={signature}";
        return url;
    }

    public bool ValidateSignature(Dictionary<string, string> parameters)
    {
        var parametersNotSecureHash = parameters
            .Where(x => x.Key != "vnp_SecureHash")
            .ToDictionary();
        var queryString = BuildQueryString(parametersNotSecureHash!);
        var checkSum = ComputeHmacSha256(_options.HashSecret, queryString);
        var secureHash = parameters.GetValueOrDefault("vnp_SecureHash");
        return checkSum.Equals(secureHash);
    }

    private static string BuildQueryString(IDictionary<string, string?> dictionary)
    {
        var queries = dictionary
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}");
        return string.Join("&", queries);
    }
    
    private static string ComputeHmacSha256(string key, string input)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var buffer = Encoding.UTF8.GetBytes(input);
        var hash = hmac.ComputeHash(buffer);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}