using System;
using System.Collections.Generic;

namespace RanStoreOracle.Models;

public partial class Login
{
    public decimal Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public decimal? RoleId { get; set; }

    public decimal? UserId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
