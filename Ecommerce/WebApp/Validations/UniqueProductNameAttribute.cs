using WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Validation
{
    public class UniqueProductNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (EcommerceDwaContext)validationContext
                         .GetService(typeof(EcommerceDwaContext));

            string productName = (string)value;
            int productId = (int)validationContext.ObjectType.GetProperty("Id").GetValue(validationContext.ObjectInstance);

            if (_context.Products.Any(x => x.Name == productName && x.IdProduct != productId))
            {
                return new ValidationResult("Product name already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
