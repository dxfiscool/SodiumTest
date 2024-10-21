using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Roblox.Services.Logging;
using System.Diagnostics.Eventing.Reader;

namespace Roblox.Services
{
    public class AssetController : ControllerBase
    {
        private readonly HttpClient HTTPClient;

        public AssetController(HttpClient httpClient)
        {
            HTTPClient = httpClient;
        }


        public async Task<IActionResult> GetAsset(int assetId)
        {
            // assetName is the name of the asset that should be in the Assets folder.
            // assetId is provided incase the asset doesn't exist. if it doesn't, we'll return the asset from AssetDelivery.

            string assetPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", assetId.ToString());

            if (System.IO.File.Exists(assetPath))
            {
                // assets aren't returned with any extension, so the mime type should be application/octet-stream
                return PhysicalFile(assetPath, "application/octet-stream");
            }
            else
            {
                // we should be migrating assets from roblox if they don't exist, not throwing an exception!
                //throw new Exception("Asset " + assetName + " does not exist");

                // migrate asset from roblox
                //https://assetdelivery.roblox.com/v1/asset/?id=assetID



                try
                {
                    var response = await HTTPClient.GetAsync($"https://assetdelivery.roblox.com/v1/asset/?id={assetId}");
                    if (response.IsSuccessStatusCode)
                    {
                        // roblox gave us the asset
                        var fileStream = await response.Content.ReadAsStreamAsync();
                        return File(fileStream, "application/octet-stream");
                    } else
                    {
                        return BadRequest();
                    }

                } catch (Exception ex)
                {
                    throw new Exception();
                }
            }

        }
    }
}
