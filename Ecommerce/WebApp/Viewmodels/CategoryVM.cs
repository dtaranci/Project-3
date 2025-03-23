using System.ComponentModel.DataAnnotations;
using WebApp.Validation;

namespace WebApp.Viewmodels
{
    public class CategoryVM
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Category name should be at least 2 characters long.")]
        [UniqueCategoryName]
        public string Name { get; set; }
    }
}
