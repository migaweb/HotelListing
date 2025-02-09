﻿using HotelListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Services
{
  public interface IAuthenticationManager
  {
    Task<bool> ValidateUser(LoginUserDTO userDTO);
    Task<string> CreateToken();
  }
}
