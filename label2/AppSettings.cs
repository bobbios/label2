using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace label2
{
    public class AppSettings
    {
        public string ZebraPrinterPath { get; set; }
        public string SnbcPrinterPath { get; set; }
        public string CreditTerminalIp { get; set; }
    }

    public static class AppSettingsStore
    {
        private static string SettingsFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PKS_LOCAL_SETTING");

        private static string SettingsFile =>
            Path.Combine(SettingsFolder, "label2-settings.json");

        public static AppSettings LoadOrCreate()
        {
            if (!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);

            if (!File.Exists(SettingsFile))
            {
                var defaults = new AppSettings
                {
                    ZebraPrinterPath = "",
                    SnbcPrinterPath = @"\\DESKTOP-HPLQJ4E\SNBC",
                    CreditTerminalIp = "192.168.1.216"
                };

                Save(defaults);
                return defaults;
            }

            try
            {
                string json = File.ReadAllText(SettingsFile);
                return JsonConvert.DeserializeObject<AppSettings>(json)
                       ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            if (!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);

            string json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(SettingsFile, json);
        }
    }
}
