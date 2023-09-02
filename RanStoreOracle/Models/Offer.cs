using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Offer
{
    public decimal Id { get; set; }

    public decimal? DiscountPer { get; set; }

    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public decimal? ItemId { get; set; }

    public decimal? Userfk { get; set; }

    public virtual Item? Item { get; set; }

    public virtual User? UserfkNavigation { get; set; }
}
