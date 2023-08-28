using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Cart
{
    public decimal Id { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Totalprice { get; set; }

    public decimal? UserId { get; set; }

    public decimal? ItemId { get; set; }

    public string? State { get; set; }

    public virtual Item? Item { get; set; }

    public virtual User? User { get; set; }
}
