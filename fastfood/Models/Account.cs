using System;
using System.Collections.Generic;

namespace fastfood.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool? Active { get; set; }

    public int? RoleId { get; set; }

    public int? CusId { get; set; }

    public virtual Customer? Cus { get; set; }

    public virtual Role? Role { get; set; }
}
