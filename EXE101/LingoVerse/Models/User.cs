using System;
using System.Collections.Generic;

namespace LingoVerse.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Set> Sets { get; set; } = new List<Set>();

    public virtual ICollection<UserPremium> UserPremia { get; set; } = new List<UserPremium>();
}
