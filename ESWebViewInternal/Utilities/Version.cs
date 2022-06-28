using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace ESWebViewInternal.Utilities
{
    public enum AutoUpdaterResult
    {
        NONE,
        REQUIRE_UPDATE
    }
    public class AutoUpdateResult
    {
        public string Version { get; set; }
        public DateTime Date { get; set; }
    }

    public class AutoUpdater
    {

        public string UpdateUrl { get; set; }
        public AutoUpdater()
        {

        }
        public async Task<AutoUpdaterResult> CheckForUpdates()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(UpdateUrl);
                if (result.IsSuccessStatusCode)
                {
                    try
                    {
                        var obj = JsonConvert.DeserializeObject<AutoUpdateResult>(await result.Content.ReadAsStringAsync());
                    }
                    catch(Exception ex)
                    {
                        return AutoUpdaterResult.NONE;
                    }
                }
                else
                {
                    return AutoUpdaterResult.NONE;
                }
            }
            return AutoUpdaterResult.NONE;
        }
    }
}
