using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext _context;

    public UsersController(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // GET /users : récupérer tous les utilisateurs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // GET /users/{login} : chercher un utilisateur par son login
    [HttpGet("{login}")]
    public async Task<ActionResult<User>> GetByLogin(string login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null)
        {
            return NotFound($"Utilisateur avec le login '{login}' introuvable.");
        }
        return Ok(user);
    }
}