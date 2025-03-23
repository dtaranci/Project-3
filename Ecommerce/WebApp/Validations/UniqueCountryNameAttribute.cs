using WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Validation
{
    public class UniqueCountryNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (EcommerceDwaContext)validationContext
                         .GetService(typeof(EcommerceDwaContext));

            string countryName = (string)value;
            int countryId = (int)validationContext.ObjectType.GetProperty("Id").GetValue(validationContext.ObjectInstance);

            if (_context.Countries.Any(x => x.Name == countryName && x.IdCountry != countryId))
            {
                return new ValidationResult("Country name already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
