using Helpdesk.Data;

namespace Helpdesk.Infrastructure
{
    /// <summary>
    /// Provides current default options for seeding the ConfigurationOption table
    /// </summary>
    public static class ConfigOptConsts
    {
        public static ConfigOptDefault System_Version =
            new ConfigOptDefault("System", "Version");
        public static ConfigOptDefault Accounts_AllowSelfRegistration =
            new ConfigOptDefault("Accounts", "Allow Self-Registration");
        public static ConfigOptDefault Accounts_MfaQrCodeSitename =
            new ConfigOptDefault("Accounts", "MFA QR Code Site Name");
        public static ConfigOptDefault Branding_SiteName =
            new ConfigOptDefault("Branding", "Site Name");
        public static ConfigOptDefault Branding_OrganizationName =
            new ConfigOptDefault("Branding", "Organization Name");
        public static ConfigOptDefault Branding_SiteURL =
            new ConfigOptDefault("Branding", "Site URL");
    }
}
