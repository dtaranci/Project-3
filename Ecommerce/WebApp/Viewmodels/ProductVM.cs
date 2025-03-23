using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApp.Validation;

namespace WebApp.Viewmodels
{
    public class ProductVM
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [UniqueProductName]
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Product name should be at least 2 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Product description is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Product description should be at least 2 characters long.")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Product category is required")]
        public int? CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Product price is required.")]
        [Range(-2147483648.00, 2147483647.00, ErrorMessage = "Product price must be between -2,147,483,648 and 2,147,483,647.")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Display(Name = "Image URL")]
        [Required(ErrorMessage = "Product image url is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Product description should be at least 2 characters long.")]
        [Url(ErrorMessage = "Invalid product image url.")]
        public string ImageURL { get; set; }

        [Display(Name = "Availablity")]
        [Required(ErrorMessage = "This field is required.")]
        public bool? IsAvailable { get; set; }

        [Display(Name = "Available Countries")]
        public IList<CountryVM>? AvailableCountries { get; set; } = new List<CountryVM>();

        public void ValidateSelectedCountry(ModelStateDictionary modelState)
        {
            if (AvailableCountries == null || !AvailableCountries.Any(x => x.IsSelected))
            {
                modelState.AddModelError(nameof(AvailableCountries), "At least one country must be selected.");
            }
        }
    }
}
