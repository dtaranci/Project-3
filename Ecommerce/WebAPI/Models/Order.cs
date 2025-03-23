using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public int CustomerId { get; set; }

    public int PaymentMethodId { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal? Total { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}
