using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    static public class APISecurityKey
    {
        public static SymmetricSecurityKey TokenKey(IConfiguration config)
        {
            

            byte[] keyBytes = new byte[64]; // 64 bytes for a 512-bit key

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
    }
}
