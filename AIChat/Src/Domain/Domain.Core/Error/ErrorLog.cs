﻿using System.ComponentModel.DataAnnotations;

namespace Domain.Core.Error
{
    public class ErrorLog
    {
        [Key]
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Page { get; set; }
        public string? Block { get; set; }
        public string? Description { get; set; }
        public string? Frame { get; set; }
        public int Line { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
