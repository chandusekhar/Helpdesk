using Helpdesk.Data;

namespace Helpdesk.Infrastructure
{
    /// <summary>
    /// Provides current default options for seeding the ConfigurationOption table
    /// </summary>
    public static class ConfigOptConsts
    {
        public static ConfigOptDefault System_Version =
            new ConfigOptDefault("System", "Version", "1.0", 9999, ReferenceTypes.Hidden);
        public static ConfigOptDefault System_UploadPath =
            new ConfigOptDefault("System", "Uploads dedicated folder", @"c:\HelpdeskUploads", 0, ReferenceTypes.String);
        public static ConfigOptDefault System_SaveUploadsToDatabase =
            new ConfigOptDefault("System", "Save Uploads In Database", "false", 1, ReferenceTypes.Boolean);
        public static ConfigOptDefault System_UploadFileSizeLimit =
            new ConfigOptDefault("System", "Upload File Size Limit Bytes", "4194304", 1, ReferenceTypes.Integer);
        public static ConfigOptDefault Accounts_AllowSelfRegistration =
            new ConfigOptDefault("Accounts", "Allow Self-Registration", "true", 0, ReferenceTypes.Boolean);
        public static ConfigOptDefault Accounts_MfaQrCodeSitename =
            new ConfigOptDefault("Accounts", "MFA QR Code Site Name", "Helpdesk", 1, ReferenceTypes.String);
        public static ConfigOptDefault Accounts_ShowMfaBanner =
            new ConfigOptDefault("Accounts", "Show MFA Banner", "true", 2, ReferenceTypes.Boolean); 
        public static ConfigOptDefault Accounts_DefaultNavTemplate =
            new ConfigOptDefault("Accounts", "Default Site Nav Template", "Everything Visible", 2, ReferenceTypes.Table_SiteNavTemplate);
        public static ConfigOptDefault Branding_OrganizationName =
            new ConfigOptDefault("Branding", "Organization Name", "Our Organization", 0, ReferenceTypes.String);
        public static ConfigOptDefault Branding_SiteName =
            new ConfigOptDefault("Branding", "Site Name", "Helpdesk", 1, ReferenceTypes.String);
        public static ConfigOptDefault Branding_SiteURL =
            new ConfigOptDefault("Branding", "Site URL", "helpdesk.localhost", 2, ReferenceTypes.String);

    }
}
