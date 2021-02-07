using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
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

    //[Authorize]
    [HttpGet("{id:int}", Name = nameof(Get))]
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

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
    {
      if (!ModelState.IsValid)
      {
        _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
        return BadRequest(ModelState);
      }

      try
      {
        var hotel = _mapper.Map<Hotel>(hotelDTO);
        await _unitOfWork.Hotels.Insert(hotel);
        await _unitOfWork.Save();

        return CreatedAtRoute(nameof(Get), new { id = hotel.Id }, hotel);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error in {nameof(CreateHotel)}.");
        return StatusCode(500, "Something went wrong. Please try again later. ");

      }
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
    {
      if (!ModelState.IsValid || id < 1)
      {
        _logger.LogError($"Invalid PUT attempt in {nameof(UpdateHotel)}");
        return BadRequest(ModelState);
      }

      try
      {
        var hotel = await _unitOfWork.Hotels.Get(h => h.Id == id);
        if (hotel == null)
        {
          return NotFound($"Hotel with id {id} could not be found.");
        }

        _mapper.Map(hotelDTO, hotel);
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.Save();

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error in {nameof(UpdateHotel)}.");
        return StatusCode(500, "Something went wrong. Please try again later. ");

      }
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
      if (id < 1)
      {
        _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
        return BadRequest("Submitted data is invalid.");
      }

      try
      {
        var hotel = await _unitOfWork.Hotels.Get(h => h.Id == id);
        if (hotel == null)
        {
          _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
          return BadRequest("Submitted data is invalid.");
        }

        await _unitOfWork.Hotels.Delete(id);
        await _unitOfWork.Save();

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error in {nameof(DeleteHotel)}.");
        return StatusCode(500, "Something went wrong. Please try again later. ");
      }
    }
  }
}
