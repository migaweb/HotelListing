using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
  [ApiVersion("2.0")]
  [Route("api/countries")]
  //[Route("api/{v:apiVersion}/countries")]
  [ApiController]
  public class CountriesV2Controller : ControllerBase
  { 
    private DatabaseContext _context;

    public CountriesV2Controller(DatabaseContext context)
    {
      _context = context;
    }

    [HttpGet]
    //[ResponseCache(CacheProfileName = "120SecondsDuration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountries()
    {
      var countries = await _context.Contries.ToListAsync();
      return Ok(countries);
    }
  }
}
