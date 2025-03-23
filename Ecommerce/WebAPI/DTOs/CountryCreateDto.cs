using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebAPI.Models;

namespace WebAPI.DTOs
{
    public class CountryCreateDto
    {
        public int IdCountry { get; set; }

        [Required(ErrorMessage = "You must provide a Name")]
        public string? Name { get; set; }

        public CountryCreateDto(Country? country)
        {
            if (country != null)
            {
                this.Name = country.Name;
                this.IdCountry = country.IdCountry;
            }
        }

        [JsonConstructor]
        public CountryCreateDto(int idCountry, string? name)
        {
            IdCountry = idCountry;
            Name = name;
        }
    }
}
