// Controllers/UserController.cs
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;
using System.Linq;
using System.Threading.Tasks;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context; // Supondo que você tenha um DbContext

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /User/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /User/Register
    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            // Aqui você pode adicionar a lógica de hash de senha (ex: BCrypt ou outra)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        return View(user);
    }

    // GET: /User/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /User/Login
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            // Lógica básica de sessão
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ErrorMessage = "Usuário ou senha incorretos.";
        return View();
    }

    // GET: /User/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
