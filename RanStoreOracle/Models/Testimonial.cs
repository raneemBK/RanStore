using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Testimonial
{
    public decimal Id { get; set; }

    public string? Message { get; set; }

    public string? Status { get; set; }

    public decimal? UserId { get; set; }

    public virtual User? User { get; set; }
}
