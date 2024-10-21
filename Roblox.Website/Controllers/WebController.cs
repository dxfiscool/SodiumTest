using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using System.Dynamic;
using Microsoft.Extensions.Configuration;

// Roblox
using Roblox.Services;
using System.Xml.Linq;

namespace Roblox.Website.Controllers
{
    [ApiController]
    public class WebController : ControllerBase
    {
        private readonly HttpClient HTTPClient;
        private IConfiguration configuration;

        public WebController(HttpClient httpClient, IConfiguration iConfig)
        {
            HTTPClient = httpClient;
            configuration = iConfig;
        }


        [HttpGet("asset")]
        public async Task<IActionResult> Asset(int id)
        {
            AssetController assetController = new AssetController(HTTPClient);
            return await assetController.GetAsset(id);
        }

        [HttpGet("GetAllowedMD5Hashes")]
        public string GetAllowedMD5Hashes()
        {
            List<string> hashes = new List<string>()
            {
                "8873d81a5fd2585b695ac880bb4f0c9a"
            };

            return JsonConvert.SerializeObject(new { data = hashes });

        }

        [HttpGet("GetAllowedSecurityKeys")]
        [HttpGet("GetAllowedSecurityVersions")]
        public string GetAllowedSecurityVersions()
        {
            List<string> versions = new List<string>()
            {
                "0.179.0pcplayer"
            };

            return JsonConvert.SerializeObject(new { data = versions });
        }

        [HttpPost("Game/MachineConfiguration.ashx")]
        [HttpPost("game/validate-machine")]
        public IActionResult ValidateMachine()
        {
            return Ok();
        }

        [HttpGet("/")]

        [HttpGet("game/PlaceLauncher.ashx")]
        public IActionResult PlaceLauncherAshx()
        {
            string BaseUrl = configuration.GetValue<string>("BaseUrl");
            dynamic PlaceLauncher = new
            {
                jobId = "Test",
                status = 2,
                joinScriptUrl = BaseUrl + "game/Join.ashx",
                authenticationUrl = BaseUrl + "Login/Negotiate.ashx",
                authenticationTicket = "Coolh",
                message = "Success"
            };

            return Ok(PlaceLauncher);
        }

        [HttpGet("Login/Negotiate.ashx")]
        public string NegotiateAshx(string suggest)
        {
            HttpContext.Response.Cookies.Append(".ROBLOSECURITY", suggest, new CookieOptions
            {
                Domain = ".lithiu.xyz",
                Secure = false,
                HttpOnly = false,
                Expires = DateTimeOffset.Now.AddDays(365),
            });

            return "true";
        }

        [HttpGet("Setting/QuietGet/{type}")]
        public dynamic FFLags(string type)
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "FFLags", type + ".json");
            if (System.IO.File.Exists(jsonFilePath))
            {
                string data = System.IO.File.ReadAllText(jsonFilePath);
                dynamic? final = JsonConvert.DeserializeObject<ExpandoObject>(data);

                return final;
            }
            else
            {
                return BadRequest(); // fflags don't exist
            };
        }

        [HttpGet("/Game/KeepAlivePinger.ashx")]
        public IActionResult KeepAlivePinger()
        {
            // TODO: Arbiter stuff 
            return Ok();
        }

        [HttpGet("game/gameserver.ashx")]
        public string GameserverAshx()
        {
            dynamic GameServerPath = Path.Combine(Directory.GetCurrentDirectory(), "Joinscripts", "2013LHost.lua");
            dynamic gameServerScript = System.IO.File.ReadAllText(GameServerPath);
            return gameServerScript;
            //return SignatureController.rbxsig(gameServerScript);
        }

        [HttpGet("game/Join.ashx")]
        public string JoinAshx()
        {
             dynamic joinScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Joinscripts", "Joinscript.lua");
             dynamic joinScript = System.IO.File.ReadAllText(joinScriptPath);
            return SignatureController.rbxsig(joinScript);
            // return Content(signatureController.rbxsigJson(joinScript), "application/json");
            //return JsonConvert.SerializeObject(joinScript);
        }

        [HttpGet("Asset/CharacterFetch.ashx")]
        public string CharacterFetch()
        {
            return "http://www.lithiu.xyz/Asset/BodyColors.ashx/?userID=1;http://www.lithiu.xyz/asset/?id=1098285";
        }

        [HttpGet("/Asset/BodyColors.ashx")]
        public async Task<string> BodyColorsAshx(int serverplaceid)
        {
            var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

            var xmlRoot = new XElement("roblox",
                new XAttribute(XNamespace.Xmlns + "xmime", "http://www.w3.org/2005/05/xmlmime"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(xsi + "noNamespaceSchemaLocation", "http://www.roblox.com/roblox.xsd"),
                new XAttribute("version", 4)
            );
            xmlRoot.Add(new XElement("External", "null"));
            xmlRoot.Add(new XElement("External", "nil"));
            var items = new XElement("Item", new XAttribute("class", "BodyColors"));
            var properties = new XElement("Properties");
            properties.Add(new XElement("int", new XAttribute("name", "HeadColor"), 194));
            properties.Add(new XElement("int", new XAttribute("name", "LeftArmColor"), 194));
            properties.Add(new XElement("int", new XAttribute("name", "LeftLegColor"), 102));
            properties.Add(new XElement("string", new XAttribute("name", "Name"), "Body Colors"));
            properties.Add(new XElement("int", new XAttribute("name", "RightArmColor"), 194));
            properties.Add(new XElement("int", new XAttribute("name", "RightLegColor"), 102));
            properties.Add(new XElement("int", new XAttribute("name", "TorsoColor"), 23));
            properties.Add(new XElement("bool", new XAttribute("name", "archivable"), "true"));
            items.Add(properties);
            xmlRoot.Add(items);
            return new XDocument(xmlRoot).ToString();
        }
    }
}
