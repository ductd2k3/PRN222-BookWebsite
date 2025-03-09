using System;
using System.Collections.Generic;

namespace LingoVerse.Models;

public partial class Word
{
    public int WordId { get; set; }

    public int SetId { get; set; }

    public string Word1 { get; set; } = null!;

    public string Definition { get; set; } = null!;

    public string? Example { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Set Set { get; set; } = null!;
}
