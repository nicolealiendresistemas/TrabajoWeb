﻿using Microsoft.AspNetCore.Mvc;

namespace TrabajoClasesVirtuales.Models
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}