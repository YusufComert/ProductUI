namespace Payment.DtoLayer.Dtos.TokenDtos
{
    public class JwtTokenDefaults
    {
        // Token oluştururken kullanılacak standart parametreler
        public const string ValidAudience = "PaymentApi"; // Hedeflenen API (audience)
        public const string ValidIssuer = "https://localhost:7276"; // Token'ı oluşturan server (issuer)
        public const string Key = "PayProject..0102030405Asp.NetCore8.0.0*/+-"; // Güvenlik anahtarı (secret key)
        public const int Expire = 5; // Token geçerlilik süresi (dakika cinsinden)

        // Google kullanıcıları için spesifik token ayarları
        public static string GoogleValidIssuer = "https://accounts.google.com"; // Google'dan gelen token'lar için issuer
        public static int GoogleTokenExpire = 10; // Google login için token geçerlilik süresi (örnek: 10 dakika)
    }
}
