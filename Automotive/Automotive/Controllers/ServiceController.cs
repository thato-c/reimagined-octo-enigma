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

        [HttpGet]
        public async Task<IActionResult> Edit(Guid serviceId)
        {
            try
            {
                var service = await serviceRepository.GetServiceByIdAsync(serviceId);

                if (service == null)
                {
                    ViewBag.Message = "The service has not been found";
                    return View();
                }

                var viewModel = new ServiceDetailViewModel
                {
                    ServiceId = service.ServiceId,
                    Name = service.Name,
                    Description = service.Description,
                    Price = service.Price,
                    LaborHours = service.LaborHours,
                    WarrantyInMonths = service.WarrantyInMonths,
                    RowVersion = service.RowVersion,
                };

                return View(viewModel);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An error occurred while retrieving data from the database.");

                // Optionally, log additional details
                if (ex.InnerException != null) 
                {
                    _logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                }
                if (ex.InnerException?.InnerException != null)
                {
                    _logger.LogError("SQL: {Message}", ex.InnerException.InnerException.Message);
                }

                ModelState.AddModelError("", "An error occurred while retrieving data from the database.");
                ViewBag.Message = "An error occurred while retrieving data from the database.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceDetailViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceToEdit = await serviceRepository.GetServiceByIdAsync(viewModel.ServiceId);

                    if (serviceToEdit != null)
                    {
                        if (serviceToEdit.Name == viewModel.Name &&
                            serviceToEdit.Description == viewModel.Description &&
                            serviceToEdit.Price == viewModel.Price &&
                            serviceToEdit.LaborHours == viewModel.LaborHours &&
                            serviceToEdit.WarrantyInMonths == viewModel.WarrantyInMonths)
                        {
                            ModelState.AddModelError(string.Empty, "Data has not been modified.");
                            return View(viewModel);
                        }

                        else
                        {
                            serviceRepository.SetOriginalRowVersion(serviceToEdit, viewModel.RowVersion);

                            if (await TryUpdateModelAsync<Service>(
                                serviceToEdit,
                                "",
                                s => s.Name, s => s.Description, s => s.Price, s => s.LaborHours, s => s.WarrantyInMonths))
                            {
                                try
                                {
                                    serviceRepository.UpdateService(serviceToEdit);
                                    serviceRepository.Save();
                                }
                                catch (DbUpdateConcurrencyException ex)
                                {
                                    var exceptionEntry = ex.Entries.Single();
                                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                                    if (databaseEntry == null)
                                    {
                                        ModelState.AddModelError(string.Empty, "Unable to save changes. The service was deleted by another user.");
                                        return View(viewModel);
                                    }
                                    else
                                    {
                                        var databaseValues = (Service)databaseEntry.ToObject();
                                        var updatedService = new ServiceDetailViewModel
                                        {
                                            ServiceId = databaseValues.ServiceId,
                                            Name = databaseValues.Name,
                                            Description = databaseValues.Description,
                                            Price = databaseValues.Price,
                                            LaborHours = databaseValues.LaborHours,
                                            WarrantyInMonths = databaseValues.WarrantyInMonths,
                                            RowVersion = viewModel.RowVersion
                                        };

                                        if (databaseValues.Name != serviceToEdit.Name)
                                        {
                                            ModelState.AddModelError("Name", $"Current Value: {databaseValues.Name}");
                                        }
                                        if (databaseValues.Price != serviceToEdit.Price)
                                        {
                                            ModelState.AddModelError("Price", $"Current Value: {databaseValues.Price}");
                                        }
                                        if (databaseValues.Description != serviceToEdit.Description)
                                        {
                                            ModelState.AddModelError("Description", $"Current Value: {databaseValues.Description}");
                                        }
                                        if (databaseValues.LaborHours != serviceToEdit.LaborHours)
                                        {
                                            ModelState.AddModelError("LaborHours", $"Current Value: {databaseValues.LaborHours}");
                                        }
                                        if (databaseValues.WarrantyInMonths != serviceToEdit.WarrantyInMonths)
                                        {
                                            ModelState.AddModelError("WarrantyInMonths", $"Current Value: {databaseValues.WarrantyInMonths}");
                                        }

                                        ModelState.AddModelError(string.Empty, "The record you attempted to edit " +
                                            "was modofied by another user after you got the original value." +
                                            "The edit operation was canceled and the current values in the database" +
                                            "have been displayed. If you still want to edit this record, click " +
                                            "the Save button again. Otherwise, click the Back to Lisst hyperlink.");

                                        return View(updatedService);
                                    }
                                }
                            }
                            return RedirectToAction("Index");
                        }
                    }

                    ViewBag.Message = "Service was not found";
                    return View(viewModel);
                }
                return View(viewModel);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An error occurred while editing data in the database.");

                // Optionally, log additional details
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                }
                if (ex.InnerException?.InnerException != null)
                {
                    _logger.LogError("SQL: {Message}", ex.InnerException.InnerException.Message);
                }

                ModelState.AddModelError("", "An error occurred while editing data in the database.");
                ViewBag.Message = "An error occurred while editing data in the database.";
                return View();
            }
        }

    }
}
