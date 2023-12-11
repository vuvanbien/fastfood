using System;
using System.Collections.Generic;

namespace fastfood.Models;

public partial class OrderDetail
{
    public int OrDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ProId { get; set; }

    public string? ProName { get; set; }

    public double? Price { get; set; }

    public int? Discount { get; set; }

    public int? Quantity { get; set; }

    public double? TotalMoney { get; set; }

    public virtual Order? Order { get; set; }
}
