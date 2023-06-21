using ASPShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ASPShop.Controllers
{
	public class HomeController : Controller
	{
		ApplicationContext db;

		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger, ApplicationContext context)
		{
			_logger = logger;
            db = context;
        }

		public async Task<IActionResult> Index()
		{
			return View(await db.Products.ToListAsync());
		}
		public IActionResult Create()
		{
			return View();
		}

		//Создание объекта в БД
		[HttpPost]
		public async Task<IActionResult> Create(Product product)
		{
			db.Products.Add(product);
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		//Удаление объекта в БД
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Product? item = await db.Products.FirstOrDefaultAsync(p => p.ID == id);
                if (item != null)
                {
                    db.Products.Remove(item);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }

        //Переход к представлению для редактирования объекта в БД
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Product? item = await db.Products.FirstOrDefaultAsync(p => p.ID == id);
                if (item != null) return View(item);
            }
            return NotFound();
        }
        //Редактирование объекта в БД
        [HttpPost]
        public async Task<IActionResult> Edit(Product item)
        {
            db.Products.Update(item);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}