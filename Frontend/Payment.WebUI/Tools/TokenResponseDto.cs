namespace Payment.WebUI.Tools
{
    public class TokenResponseDto
    {
        public TokenResponseDto(string token, DateTime expireDate, string provider)
        {
            Token = token;
            ExpireDate = expireDate;
            Provider = provider;
        }

        public string Token { get; set; }           // JWT Token
        public DateTime ExpireDate { get; set; }    // Token'ın son kullanma tarihi
        //public string RefreshToken { get; set; }    // Refresh Token (JWT Token süresi dolduğunda yeni token almak için)
        //public string TokenType { get; set; }       // Token türü (örneğin: "Bearer")
        public string Provider { get; set; }        // Giriş sağlayıcısı (Google)
    }
}
