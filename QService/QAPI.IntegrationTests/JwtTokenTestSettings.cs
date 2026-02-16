using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace QAPI.IntegrationTests;

public class JwtTokenTestSettings
{
    public const string SecretKey = "MySuperSecureAndRandomKeyThatLooksJustAwesomeAndNeedsToBeVeryVeryLong!!!111oneeleven";
    public const string Audience = "QueueSystemAudience";
    public const string Issuer = "QueueSystem";
    public const int ExpireTimeInSeconds = 3600;
       

    private static readonly SymmetricSecurityKey secretKey = new(
        Encoding.UTF8.GetBytes(SecretKey));
}