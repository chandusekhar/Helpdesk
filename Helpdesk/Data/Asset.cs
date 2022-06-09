using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class Asset
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name="Creating User")]
        public HelpdeskUser CreatingUser { get; set; }

        [Required]
        [Display(Name="Created Date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Display(Name = "Last Update Date")]
        public DateTime LastUpdateDate { get; set; }

        [Display(Name="Asset Type")]
        public AssetType? AssetType { get; set; }

        [Display(Name = "Status")]
        public AssetStatus? AssetStatus { get; set; }

        [Display(Name = "Location")]
        public AssetLocation? AssetLocation { get; set; }

        public Manufacturer? Manufacturer { get; set; }

        [Display(Name = "Model")]
        public AssetModel? AssetModel { get; set; }

        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }

        public Group? Group { get; set; }

        [Display(Name = "Assigned User")]
        public HelpdeskUser? AssignedUser { get; set; }

        [Display(Name = "Purchasing Vendor")]
        public Vendor? PurchasingVendor { get; set; }

        [Display(Name = "Purchase Price")]
        public decimal? PurchasePrice { get; set; }

        [Display(Name = "Quote Number")]
        public string? QuoteNumber { get; set; }

        [Display(Name = "Purchase Order Number")]
        public string? PONumber { get; set; }

        /// <summary>
        /// User that made the purchase
        /// </summary>
        [Display(Name = "Purchased By")]
        public HelpdeskUser? PurchasingUser { get; set; }

        [Display(Name = "Estimated Delivery")]
        public DateTime? EstimatedDelivery { get; set; }

        /// <summary>
        /// User that requested/approved the purchase
        /// </summary>
        [Display(Name = "Approved By")]
        public HelpdeskUser? ApprovingUser { get; set; }

        [Display(Name = "Warranty Info")]
        public string? WarrantyInfo { get; set; }

        public ICollection<AssetChangeLog> ChangeLog { get; set; }
        public ICollection<AssetLicenseAssignment> Licenses { get; set; }
        public ICollection<FileUpload> Documents { get; set; }
    }
}
