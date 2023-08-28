using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Role
{
    public decimal Id { get; set; }

    public string? Rolename { get; set; }

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}
