using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class CountryProduct
{
    public int IdCproduct { get; set; }

    public int CountryId { get; set; }

    public int ProductId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
