﻿using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty!;
        [Required]
        public string Description { get; set; } = string.Empty!;

    }
}
