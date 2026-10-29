using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using GestionOrdinateurs.Shared;

namespace AuthentificationApi.Controllers;

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
}

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    // POST /auth/login : authentifie un utilisateur 
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Login))
        {
            return BadRequest("Le login est requis.");
        }

        // récupération de l'utilisateur auprès de GestionOrdinateurs.API
        var client = _httpClientFactory.CreateClient();
        var apiUrl = _configuration["GestionOrdinateursApi:BaseUrl"] ?? "http://localhost:5019";

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync($"{apiUrl}/users/{request.Login}");
        }
        catch (Exception ex)
        {
            return StatusCode(503, $"Impossible de joindre l'API des utilisateurs : {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
        {
            return NotFound(new { message = $"L'utilisateur '{request.Login}' n'existe pas." });
        }

        var user = await response.Content.ReadFromJsonAsync<User>();
        if (user == null)
        {
            return NotFound(new { message = "Données utilisateur invalides." });
        }

        // claims pour le JWT
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // clé secrète pour signer le JWT
        var jwtKey = _configuration["Jwt:Key"] ?? "CleParDefautDeSecuriteTresLongue123456789!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // création du JWT
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            login = user.Login,
            role = user.Role.ToString()
        });
    }
}