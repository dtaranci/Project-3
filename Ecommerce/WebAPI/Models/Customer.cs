using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Customer
{
    public int IdCustomer { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }

    public virtual ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
