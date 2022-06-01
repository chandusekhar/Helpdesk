using Helpdesk.Data;

namespace Helpdesk.Infrastructure
{
    /// <summary>
    /// Provides current default options for seeding the ConfigurationOption table
    /// </summary>
    public static class ConfigOptConsts
    {
        public static ConfigOptDefault System_Version =
            new ConfigOptDefault("System", "Version", "1.0");
        public static ConfigOptDefault Accounts_AllowSelfRegistration =
            new ConfigOptDefault("Accounts", "Allow Self-Registration", "true", 0);
        public static ConfigOptDefault Accounts_MfaQrCodeSitename =
            new ConfigOptDefault("Accounts", "MFA QR Code Site Name", "Helpdesk", 1);
        public static ConfigOptDefault Accounts_ShowMfaBanner =
            new ConfigOptDefault("Accounts", "Show MFA Banner", "true", 2); 
        public static ConfigOptDefault Accounts_DefaultNavTemplate =
            new ConfigOptDefault("Accounts", "Default Site Nav Template", "Everything Visible", 2);
        public static ConfigOptDefault Branding_OrganizationName =
            new ConfigOptDefault("Branding", "Organization Name", "Our Organization", 0);
        public static ConfigOptDefault Branding_SiteName =
            new ConfigOptDefault("Branding", "Site Name", "Helpdesk", 1);
        public static ConfigOptDefault Branding_SiteURL =
            new ConfigOptDefault("Branding", "Site URL", "helpdesk.localhost", 2);

    }
}
