using global::WebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.DTOs;

public partial class CategoryDto
{
    public int IdCategory { get; set; }


    public CategoryDto(Category? category)
    {
        if (category != null)
        {
            this.IdCategory = category.IdCategory;
        }
    }

    [JsonConstructor]
    public CategoryDto(int idCategory)
    {
        IdCategory = idCategory;
    }
}

