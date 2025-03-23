using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class ProductOrder
{
    public int IdPorder { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
