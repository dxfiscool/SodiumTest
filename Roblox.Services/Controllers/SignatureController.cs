using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

using Roblox.Services.Logging;

namespace Roblox.Services
{
    public class SignatureController : ControllerBase
    {
        // import everything, etc.
        private static RSACryptoServiceProvider? rsaCSP;
        private static SHA1? sha1;

        public static void Start()
        {
            try // it may not always be successful, so it's best if we do this.
            {
                byte[] privateKey = Convert.FromBase64String(System.IO.File.ReadAllText("PrivateKey.txt")); // blob Private Key format

                sha1 = SHA1.Create();
                rsaCSP = new RSACryptoServiceProvider();

                rsaCSP.ImportCspBlob(privateKey);
                Log.RobloxLog("Initialized SignatureController");
            }
            catch
            {
                throw new Exception("Error setting up SignatureController");
            }
        }

        public static string rbxsig(dynamic data)
        {
            if (data is string)
            {
                string rbxsigFormat = "%{0}%{1}"; // 0 is the sig, 1 is the script.
                byte[] sig = rsaCSP.SignData(Encoding.Default.GetBytes(data), sha1);

                string rbxsig = string.Format(rbxsigFormat, Convert.ToBase64String(sig), data); // the sig is in bytes, so it should be converted to base64
                return rbxsig;
            } else
            {
                throw new Exception("Expected a string");
            }

        }

        public static string rbxsigJson(dynamic data)
        {
            // can't be bothered to add a check here for json
            string rbxsigFormat = "--rbxsig%{0}%{1}"; // 0 is the sig, 1 is the script.

            string json = JsonConvert.SerializeObject(data);
            string final = Environment.NewLine + json;
            byte[] sig = rsaCSP.SignData(Encoding.Default.GetBytes(final), sha1);

            string rbxsig = string.Format(rbxsigFormat, Convert.ToBase64String(sig), final); // the sig is in bytes, so it should be converted to base64
            return rbxsig;
        }
    }
}
