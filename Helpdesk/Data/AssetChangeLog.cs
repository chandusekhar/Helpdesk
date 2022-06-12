using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class AssetChangeLog
    {
        [Key]
        public int Id { get; set; }
        public Asset Asset { get; set; }
        public HelpdeskUser EditingUser { get; set; }
        public DateTime WhenEdited { get; set; }
        public string Description { get; set; }
    }
}
