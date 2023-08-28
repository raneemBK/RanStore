using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RanStoreOracle.Models;

public partial class Item
{
    public decimal Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public DateTime? Dateofupload { get; set; }

    public string? Imagepath { get; set; }

    public decimal? UserId { get; set; }

    public decimal? CategoryId { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual User? User { get; set; }
}
