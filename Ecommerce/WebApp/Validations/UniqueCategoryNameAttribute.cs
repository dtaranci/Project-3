using WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Validation
{
    public class UniqueCategoryNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (EcommerceDwaContext)validationContext
                         .GetService(typeof(EcommerceDwaContext));

            string categoryName = (string)value;
            int categoryId = (int)validationContext.ObjectType.GetProperty("Id").GetValue(validationContext.ObjectInstance);

            if (_context.Categories.Any(x => x.Name == categoryName && x.IdCategory != categoryId))
            {
                return new ValidationResult("Category name already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
