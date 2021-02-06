using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
  public class AccountsController : ControllerBase
  {
    private readonly UserManager<ApiUser> _userManager;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly ILogger<AccountsController> _logger;
    private readonly IMapper _mapper;

    public AccountsController(UserManager<ApiUser> userManager, 
                              IAuthenticationManager authenticationManager,
                              ILogger<AccountsController> logger,
                              IMapper mapper)
    {
      _userManager = userManager;
      _authenticationManager = authenticationManager;
      _logger = logger;
      _mapper = mapper;
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
      _logger.LogInformation($"Registration attempt for {userDTO.Email}");

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        var user = _mapper.Map<ApiUser>(userDTO);
        user.UserName = user.Email;
        var result = await _userManager.CreateAsync(user, userDTO.Password);

        if (!result.Succeeded)
        {
          // result.Errors, log or return some info
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(error.Code, error.Description);
          }
          return BadRequest(ModelState);
        }

        await _userManager.AddToRolesAsync(user, userDTO.Roles);
        return Accepted();
      } 
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred in the {nameof(Register)}");
        return Problem($"An error occurred in the {nameof(Register)}", statusCode: 500);
      }
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
    {
      _logger.LogInformation($"Login attempt for {userDTO.Email}");

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      try
      {
        if (!(await _authenticationManager.ValidateUser(userDTO)))
        {
          // result.Errors, log or return some info
          return Unauthorized(userDTO);
        }

        return Accepted(
          new { Token = await _authenticationManager.CreateToken() }
          );
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred in the {nameof(Login)}");
        return Problem($"An error occurred in the {nameof(Login)}", statusCode: 500);

      }
    }
  }
}
