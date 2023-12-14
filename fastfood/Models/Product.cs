using System;
using System.Collections.Generic;

namespace fastfood.Models;

public partial class Product
{
    public int ProId { get; set; }

    public string? ProName { get; set; }

    public double? Price { get; set; }

    public int? Discount { get; set; }

    public string? ShortDesc { get; set; }

    public bool? Hot { get; set; }

    public string? Thumb { get; set; }

    public bool? Active { get; set; }

    public string? Title { get; set; }

    public int? CateId { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Category? Cate { get; set; }
}

