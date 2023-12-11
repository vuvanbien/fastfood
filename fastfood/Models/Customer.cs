using System;
using System.Collections.Generic;

namespace fastfood.Models;

public partial class Customer
{
    public int CusId { get; set; }

    public string? CusName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
