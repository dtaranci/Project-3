using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebAPI.Models;

namespace WebAPI.DTOs
{
    public class CountryDto
    {
        public int IdCountry { get; set; }

        public CountryDto(Country? country)
        {
            if (country != null)
            {
                this.IdCountry = country.IdCountry;
            }
        }

        [JsonConstructor]
        public CountryDto(int idCountry)
        {
            IdCountry = idCountry;
        }
    }
}
