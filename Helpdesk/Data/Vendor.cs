﻿using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
