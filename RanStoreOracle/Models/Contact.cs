using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Contact
{
    public decimal Id { get; set; }

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }
}
