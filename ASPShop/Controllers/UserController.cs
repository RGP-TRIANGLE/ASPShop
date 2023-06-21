using ASPShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace ASPShop.Controllers
{
    public class UserController : Controller
    {
        ApplicationContext db;

        private IMemoryCache cache;

        private readonly ILogger<UserController> _logger;

        CookieOptions _cookieOptions = new CookieOptions();


        public UserController(ILogger<UserController> logger, ApplicationContext context, IMemoryCache memoryCache)
        {
            _cookieOptions.Expires = DateTime.Now.AddDays(365);
            _logger = logger;
            db = context;
            cache = memoryCache;
        }

        public IActionResult Profile()
        {
            if (Request.Cookies["entered"] != null)
            {
                Console.WriteLine("-    Войденный аккаунт обнаружен в кеше!");
                User? user = db.Users.FirstOrDefaultAsync(x => x.Password == Request.Cookies["entered"]).Result;
                return View(user);
            }
            else
            {
                Console.WriteLine("-    Войденный аккаунт НЕ обнаружен в кеше!");
                return RedirectToAction("Login");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            if(db.Users.FirstOrDefaultAsync(x=>x.Email == Email && x.Password == Password).Result != null)
            {
                User? user = db.Users.FirstOrDefaultAsync(x => x.Email == Email && x.Password == Password).Result;

                Response.Cookies.Append("entered", Password, _cookieOptions);

                Console.WriteLine("-    Пользователь найден");

                return RedirectToAction("Profile");
            }
            else
            {
                Console.WriteLine("-    Пользователь не найден");
                return View();
            }
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(User user)
        {
            Console.WriteLine("Пользователь: " + user.Name + " / " + user.Email + " / " + user.Password);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            Console.WriteLine("-    Пользователь сохранен");
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
