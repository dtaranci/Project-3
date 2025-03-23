using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class PaymentMethod
{
    public int IdPaymentMethod { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
