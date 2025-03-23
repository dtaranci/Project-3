using System.ComponentModel.DataAnnotations;
using WebApp.Validation;

namespace WebApp.Viewmodels
{
    public class CountryVM
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Country name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Country name should be at least 2 characters long.")]
        [UniqueCountryName]
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
