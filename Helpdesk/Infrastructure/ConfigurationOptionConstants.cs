namespace Helpdesk.Infrastructure
{
    /// <summary>
    /// Provides current default options for seeding the ConfigurationOption table
    /// </summary>
    public static class ConfigOptConsts
    {
        public static ConfigOptDefault System_Version =
            new ConfigOptDefault("System", "Version");
        public static ConfigOptDefault Login_MfaQrCodeSitename =
            new ConfigOptDefault("Login", "MFA QR Code Site Name");
        public static ConfigOptDefault Branding_SiteName =
            new ConfigOptDefault("Branding", "Site Name");
        public static ConfigOptDefault Branding_OrganizationName =
            new ConfigOptDefault("Branding", "Organization Name");
    }

    public class ConfigOptDefault
    {
        public ConfigOptDefault(string category, string key)
        {
            Category = category;
            Key = key;
            Value = string.Empty;
            Order = null;
        }

        public ConfigOptDefault(string category, string key, string value)
        {
            Category = category;
            Key = key;
            Value = value;
            Order = null;
        }

        public ConfigOptDefault(string category, string key, string value, int order)
        {
            Category = category;
            Key = key;
            Value = value;
            Order = order;
        }

        public string Category { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int? Order { get; set; }
    }

    /// <summary>
    /// Provides a list of options that need to be applied if the database is currently on a specified version.
    /// </summary>
    public static class ConfigOptRollupCatalog
    {
        public static List<ConfigOptVersion> RollupIndex = new List<ConfigOptVersion>()
        {
            new ConfigOptVersion()
            {
                Version = string.Empty,
                Additions = new List<ConfigOptDefault>()
                {
                    new ConfigOptDefault("System", "Version", "1.0"),
                    new ConfigOptDefault("Login", "MFA QR Code Site Name", "Helpdesk", 1),
                    new ConfigOptDefault("Branding", "Organization Name", "Our Organization", 0),
                    new ConfigOptDefault("Branding", "Site Name", "Helpdesk", 1),
                }
            }
        };
    }

    public class ConfigOptVersion
    {
        public ConfigOptVersion()
        {
            Version = string.Empty;
            Additions = new List<ConfigOptDefault>();
            Deletions = new List<ConfigOptDefault>();
            Changes = new List<ConfigOptDefault>();
        }

        /// <summary>
        /// Version of the database that this update applies to.
        /// </summary>
        public string Version { get; set; }
        public List<ConfigOptDefault> Additions;
        public List<ConfigOptDefault> Deletions;
        public List<ConfigOptDefault> Changes;
    }

}
