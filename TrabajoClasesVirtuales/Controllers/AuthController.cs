using TrabajoClasesVirtuales.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrabajoClasesVirtuales.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TrabajoClasesVirtuales.Models.Proyecto.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Endpoint para registrar un usuario
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        // Verifica si el usuario o correo ya existen
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
        {
            return BadRequest("El usuario o correo ya está en uso.");
        }

        // Crea un nuevo usuario
        var user = new UserModel
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password)
        };

        // Guarda el usuario en la base de datos
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Usuario registrado exitosamente.");
    }

    // Endpoint para iniciar sesión
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // Busca al usuario por su nombre de usuario
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized("Credenciales inválidas.");
        }

        // Genera el token JWT
        var token = GenerateJwtToken(user.Username);
        return Ok(new { Token = token });
    }

    // Método para encriptar la contraseña
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    // Método para verificar la contraseña
    private bool VerifyPassword(string password, string passwordHash)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == passwordHash;
    }

    // Método para generar un token JWT
    private string GenerateJwtToken(string username)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}