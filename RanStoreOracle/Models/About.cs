using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RanStoreOracle.Models;

public partial class About
{
    public decimal Id { get; set; }

    public string? Paragraph { get; set; }

    public string? Images { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }
}
