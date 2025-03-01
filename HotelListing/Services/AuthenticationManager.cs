﻿using HotelListing.Data;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Services
{
  public class AuthenticationManager : IAuthenticationManager
  {
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApiUser _user;
    
    public AuthenticationManager(UserManager<ApiUser> userManager, IConfiguration configuration)
    {
      _userManager = userManager;
      _configuration = configuration;
    }

    public async Task<bool> ValidateUser(LoginUserDTO userDTO)
    {
      _user = await _userManager.FindByNameAsync(userDTO.Email);

      return (_user != null && 
              await _userManager.CheckPasswordAsync(_user, userDTO.Password));
    }

    public async Task<string> CreateToken()
    {
      var signingCredentials = GetSigningCredentials();
      var claims = await GetClaims();
      var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

      return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
      var jwtSettings = _configuration.GetSection("Jwt");
      var expiration = DateTime.Now.AddMinutes(
        Convert.ToDouble(
          jwtSettings.GetSection("Lifetime").Value));

      var token = new JwtSecurityToken(
        issuer: jwtSettings.GetSection("Issuer").Value,
        claims: claims,
        expires: expiration,
        signingCredentials: signingCredentials
        );
      return token;
    }

    private async Task<List<Claim>> GetClaims()
    {
      var claims = new List<Claim> { 
        new Claim(ClaimTypes.Name, _user.UserName)
      };

      var roles = await _userManager.GetRolesAsync(_user);

      foreach (var role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }

      return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
      var key = Environment.GetEnvironmentVariable("KEY");
      var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

      return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
  }
}
