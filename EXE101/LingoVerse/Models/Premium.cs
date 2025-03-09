using System;
using System.Collections.Generic;

namespace LingoVerse.Models;

public partial class Premium
{
    public int PremiumId { get; set; }

    public string PremiumName { get; set; } = null!;

    public string? Description { get; set; }

    public int DurationDays { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<UserPremium> UserPremia { get; set; } = new List<UserPremium>();
}
