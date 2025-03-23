using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebAPI.Models;

namespace WebAPI.DTOs;

public partial class ProductDto
{
    public int IdProduct { get; set; }

    [Required(ErrorMessage = "You must provide a Name")]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "You must provide a Price")]
    public decimal? Price { get; set; }

    public string? ImgUrl { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual CategoryDto? Category { get; set; }

    public virtual IList<CountryDto> Countries { get; set; } = new List<CountryDto>();

    public ProductDto(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        this.IdProduct = product.IdProduct;
        this.Name = product.Name;
        this.Description = product.Description;
        this.Price = product.Price;
        this.ImgUrl = product.ImgUrl;
        this.IsAvailable = product.IsAvailable;
        if (product.CategoryId != null || product.Category != null)
        {
            this.Category = new CategoryDto(product.Category);
        }
    }

    public ProductDto(int idProduct, string? name, string? description, decimal? price, string? imgUrl, bool? isAvailable, Category? category)
    {
        IdProduct = idProduct;
        Name = name;
        Description = description;
        Price = price;
        ImgUrl = imgUrl;
        IsAvailable = isAvailable;
        this.Category = new CategoryDto(category);
    }

    public ProductDto(string? name, string? description, decimal? price, string? imgUrl, bool? isAvailable)
    {
        Name = name;
        Description = description;
        Price = price;
        ImgUrl = imgUrl;
        IsAvailable = isAvailable;
    }

    [JsonConstructor]
    public ProductDto(int idProduct, string? name, string? description, decimal? price, string? imgUrl, bool? isAvailable, CategoryDto? category)
    {
        IdProduct = idProduct == null ? 0 : idProduct;
        Name = name == null ? "test" : name;
        Description = description == null ? "test" : description;
        Price = price == null ? 0 : price;
        ImgUrl = imgUrl == null ? "test" : imgUrl;
        IsAvailable = isAvailable == null ? false : isAvailable;
        Category = category == null ? null : category;
    }

    public ProductDto(Product product, EcommerceDwaContext context) : this(product)
    {
        var contextProduct = context.Products.Include(x => x.CountryProducts).First(x => x.IdProduct == product.IdProduct);

        if (contextProduct != null) 
        {
            foreach (CountryProduct countryProduct in contextProduct.CountryProducts)
            {
                this.Countries.Add(new CountryDto(context.Countries.First(x => x.IdCountry == countryProduct.CountryId)));
            }
        }
    }

    public ProductDto(ProductCreateDto product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        this.Name = product.Name;
        this.Description = product.Description;
        this.Price = product.Price;
        this.ImgUrl = product.ImgUrl;
        this.IsAvailable = product.IsAvailable;
        if (product.Category != null)
        {
            this.Category = product.Category;
        }
    }

    public ProductDto(ProductCreateDto product, EcommerceDwaContext context, int id) : this(product)
    {
        var contextProduct = context.Products.Include(x => x.CountryProducts).First(x => x.IdProduct == id);

        if (contextProduct != null)
        {
            foreach (CountryProduct countryProduct in contextProduct.CountryProducts)
            {
                this.Countries.Add(new CountryDto(context.Countries.First(x => x.IdCountry == countryProduct.CountryId)));
            }
        }
    }


}
