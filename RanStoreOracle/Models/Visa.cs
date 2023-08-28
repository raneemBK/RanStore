using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Visa
{
    public decimal Id { get; set; }

    public decimal? Ipan { get; set; }

    public decimal? Balance { get; set; }

    public decimal? UserId { get; set; }

    public virtual User? User { get; set; }
}
