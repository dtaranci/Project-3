using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Country
{
    public int IdCountry { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<CountryProduct> CountryProducts { get; set; } = new List<CountryProduct>();
}
