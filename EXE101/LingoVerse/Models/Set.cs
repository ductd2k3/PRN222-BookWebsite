using System;
using System.Collections.Generic;

namespace LingoVerse.Models;

public partial class Set
{
    public int SetId { get; set; }

    public int UserId { get; set; }

    public string SetName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? TotalWords { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Word> Words { get; set; } = new List<Word>();
}
