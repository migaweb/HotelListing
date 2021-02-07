using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CountriesController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountriesController> _logger;
    private readonly IMapper _mapper;

    public CountriesController(IUnitOfWork unitOfWork, 
                               ILogger<CountriesController> logger,
                               IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _logger = logger;
      _mapper = mapper;
    }

    [HttpGet]
    [ApiVersion("1.0", Deprecated = true)]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 70)]
    [HttpCacheValidation(MustRevalidate = true)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
    {
      var countries = await _unitOfWork.Countries.GetAll(requestParams);
      var result = _mapper.Map<IList<CountryDTO>>(countries);
      return Ok(countries);
    }

    [HttpGet("{id:int}", Name = nameof(GetCountry))]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCountry(int id)
    {
      var country = await _unitOfWork.Countries.Get(e => e.Id == id, new List<string> { "Hotels" });
      
      if (country == null)
      {
        return NotFound($"Country with id {id} was not found.");
      }

      return Ok(_mapper.Map<CountryDTO>(country));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
    {
      if (!ModelState.IsValid)
      {
        _logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
        return BadRequest(ModelState);
      }

      var country = _mapper.Map<Country>(countryDTO);
      await _unitOfWork.Countries.Insert(country);
      await _unitOfWork.Save();

      return CreatedAtRoute(nameof(GetCountry), new { id = country.Id }, country);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
    {
      if (!ModelState.IsValid || id < 1)
      {
        _logger.LogError($"Invalid PUT attempt in {nameof(UpdateCountry)}");
        return BadRequest(ModelState);
      }

      var country = await _unitOfWork.Countries.Get(c => c.Id == id);
      if (country == null)
      {
        return NotFound($"Country with the specified id {id} was not found.");
      }

      _mapper.Map(countryDTO, country);
      _unitOfWork.Countries.Update(country);
      await _unitOfWork.Save();

      return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCountry(int id)
    {
      if (id < 1)
      {
        _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
        return BadRequest("Submitted data is invalid.");
      }

      var country = await _unitOfWork.Countries.Get(h => h.Id == id);
      if (country == null)
      {
        _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
        return BadRequest("Submitted data is invalid.");
      }

      await _unitOfWork.Countries.Delete(id);
      await _unitOfWork.Save();

      return NoContent();
    }
  }
}
