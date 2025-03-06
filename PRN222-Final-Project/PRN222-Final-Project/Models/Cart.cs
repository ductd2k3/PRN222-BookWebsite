using System;
using System.Collections.Generic;

namespace PRN222_Final_Project.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? AddedDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
