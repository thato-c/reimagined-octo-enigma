using Automotive.Interfaces;
using Automotive.Models;
using Automotive.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Automotive.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ILogger<ServiceController> _logger;
        private IServiceRepository serviceRepository;

        public ServiceController(ILogger<ServiceController> logger, IServiceRepository serviceRepository)
        {
            _logger = logger;
            this.serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            try
            {
                ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                if (searchString !=  null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;
                var services = from s in serviceRepository.GetServices() select s;

                if (!String.IsNullOrEmpty(searchString))
                {
                    services = services.Where(s => s.Name.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        services = services.OrderByDescending(s => s.Name);
                        break;
                    default:
                        services.OrderBy(s => s.Name);
                        break;
                }
                int pageSize = 8;
                return View(await PaginatedList<Service>.CreateAsync((IQueryable<Service>)services, pageNumber ?? 1,pageSize));

            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                // Optionally, log additional details
                // Log the SQL statement causing the exception
                Console.WriteLine($"SQL: {ex.InnerException?.InnerException?.Message}");
                ModelState.AddModelError("", "An error occurred while retrieving data from the database.");

                ViewBag.Message = "An error occurred while retrieving data from the database.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Map the viewModel to the SService Model
                    var service = new Models.Service
                    {
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        Price = viewModel.Price,
                        LaborHours = viewModel.LaborHours,
                        WarrantyInMonths = viewModel.WarrantyInMonths,
                    };

                    // Add and save the new service to the database
                    serviceRepository.InsertService(service);
                    serviceRepository.Save();
                    return RedirectToAction("Index");
                }
                return View(viewModel);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception detai;s
                _logger.LogError(ex, "An error occurred while inserting data into the database.");

                // Optionally, log additional details
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                }
                if (ex.InnerException?.InnerException != null)
                {
                    _logger.LogError("SQL: {Message}", ex.InnerException.InnerException.Message);
                }

                ModelState.AddModelError("", "An error occurred while inserting data into the database.");
                ViewBag.Message = "An error occurred while inserting data into the database.";
                return View();
            }
        }
    }
}
