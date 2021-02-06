using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
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
  public class HotelsController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelsController> _logger;
    private readonly IMapper _mapper;

    public HotelsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HotelsController> logger)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
      try
      {
        var hotels = await _unitOfWork.Hotels.GetAll();
        return Ok(_mapper.Map<IList<HotelDTO>>(hotels));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error in {nameof(GetAll)}.");
        return StatusCode(500, "Something went wrong. Please try again later. ");
      }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id)
    {
      try
      {
        var hotel = await _unitOfWork.Hotels.Get(h => h.Id == id, new List<string> { "Country" });

        if (hotel == null)
        {
          return NotFound($"Hotel with id {id} was not found.");
        }

        return Ok(_mapper.Map<HotelDTO>(hotel));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error in {nameof(Get)}.");
        return StatusCode(500, "Something went wrong. Please try again later. ");
      }
    }
  }
}
