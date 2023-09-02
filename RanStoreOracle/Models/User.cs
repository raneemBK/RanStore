using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RanStoreOracle.Models;

public partial class User
{
    public decimal Id { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Gender { get; set; }

    public string? Imagepath { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

    public virtual ICollection<Visa> Visas { get; set; } = new List<Visa>();
}
