using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models
{
  public class CountryDTO : CreateCountryDTO
  {
    public int Id { get; set; }
    public IList<HotelDTO> Hotels { get; set; }
  }

  public class UpdateCountryDTO : CreateCountryDTO
  {
    
  }

  public class CreateCountryDTO
  {
    [Required]
    [StringLength(maximumLength: 100, ErrorMessage = "Country name is too long, max 100 characters.")]
    public string Name { get; set; }
    [Required]
    [StringLength(maximumLength: 2, ErrorMessage = "Short country name is too long, max 2 characters.")]
    public string ShortName { get; set; }
  }
}
