using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class HelpdeskUser
    {
        public HelpdeskUser()
        {

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }
    }
}
