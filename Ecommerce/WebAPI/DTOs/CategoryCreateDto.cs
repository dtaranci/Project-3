using global::WebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.DTOs;

public partial class CategoryCreateDto
{
    public int IdCategory { get; set; }

    [Required(ErrorMessage = "You must provide a Name")]
    public string? Name { get; set; }

    public CategoryCreateDto(Category? category)
    {
        if (category != null)
        {
            this.Name = category.Name;
            this.IdCategory = category.IdCategory;
        }
    }

    [JsonConstructor]
    public CategoryCreateDto(int idCategory, string? name)
    {
        IdCategory = idCategory;
        Name = name;
    }
}

