using Microsoft.JSInterop;
using Saml2AuthGateway.Pages;
using Saml2AuthGateway.Services;

namespace Saml2AuthGateway.Data
{
    
    /// <summary>
    /// Action interface for <see cref="Cookie"/>
    /// </summary>
    public interface ICookie
    {
        public Task SetValue(string key, string value, int? days = null);
        public Task<string> GetValue(string key, string def = "");
        public Task PlaySound(string soundValue, string htext, int delay = 10);
    }

    /// <summary>
    /// Cookie represents a asp.net classic cooklie implementing <see cref="ICookie"/>
    /// </summary>
    public class Cookie : ICookie
    {
        readonly IJSRuntime JSRuntime;
        string expires = "";

        public Cookie(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
            ExpireDays = 300;
        }

        public async Task SetValue(string key, string value, int? days = null)
        {
            var curExp = (days != null) ? (days > 0 ? DateToUTC(days.Value) : "") : expires;
            await SetCookie($"{key}={value}; expires={curExp}; path=/");
        }

        public async Task<string> GetValue(string key, string def = "")
        {
            var cValue = await GetCookie();
            if (string.IsNullOrEmpty(cValue)) return def;

            var vals = cValue.Split(';');
            foreach (var val in vals)
                if (!string.IsNullOrEmpty(val) && val.IndexOf('=') > 0)
                    if (val.Substring(0, val.IndexOf('=')).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                        return val.Substring(val.IndexOf('=') + 1);
            return def;
        }

        private async Task SetCookie(string value)
        {
            await JSRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{value}\"");
        }


        public async Task PlaySound(string soundValue, string htext, int delay = 10)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("eval", 
                    "setTimeout(function() { " +
                    "   document.getElementById('h2Run').innerText = '" + htext + "'; " + 
                    "   let sound = new Audio('" + soundValue + "'); " + @"
                        sound.autoplay = true; 
                        sound.loop = false; 
                        try { 
                            sound.play();  
                        } catch (soundPlayEx) {
                           console.log('playSound(" + soundValue + ") throwed exception: ' + soundPlayEx); " + @"
			            }
                        setTimeout(function() {
                            sound.loop = false; 
                            sound.pause(); 
                            sound.autoplay = false; 
                            sound.currentTime = 0;
                            try { 
                                sound.src = """"; 
                                sound = null; 
                            } catch (exSnd) { 
                            }
                            soundDuration = 2500;
                        }, 2500);" +
                    "}, " + delay + "); "
                );
            }
            catch (Exception ex)
            {
                EnablerLog.LogOriginMsgEx("ICookie.cs", "playSound(" + soundValue + ")", ex);
            }                       
        }

        private async Task<string> GetCookie()
        {
            return await JSRuntime.InvokeAsync<string>("eval", $"document.cookie");
        }

        public int ExpireDays
        {
            set => expires = DateToUTC(value);
        }

        private static string DateToUTC(int days) => DateTime.Now.AddDays(days).ToUniversalTime().ToString("R");
    }
}
