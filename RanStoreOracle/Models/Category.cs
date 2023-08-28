using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RanStoreOracle.Models;

public partial class Category
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public string? Imagepath { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
