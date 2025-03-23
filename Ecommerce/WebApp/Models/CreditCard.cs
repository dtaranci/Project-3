using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class CreditCard
{
    public int IdCreditCard { get; set; }

    public int CustomerId { get; set; }

    public string Provider { get; set; } = null!;

    public string Number { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
