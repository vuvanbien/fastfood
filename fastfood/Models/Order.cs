using System;
using System.Collections.Generic;

namespace fastfood.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Note { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public double? TotalMoney { get; set; }

    public int? CusId { get; set; }

    public virtual Customer? Cus { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
