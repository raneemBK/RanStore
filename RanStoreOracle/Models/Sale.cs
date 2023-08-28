using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Sale
{
    public decimal Id { get; set; }

    public DateTime? Dateofsale { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Totalprice { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Itemid { get; set; }

    public virtual Item? Item { get; set; }

    public virtual User? User { get; set; }
}
