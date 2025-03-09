using System;
using System.Collections.Generic;

namespace LingoVerse.Models;

public partial class UserPremium
{
    public int UserPremiumId { get; set; }

    public int UserId { get; set; }

    public int PremiumId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Premium Premium { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
