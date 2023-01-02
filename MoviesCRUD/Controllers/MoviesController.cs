using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MoviesCRUD.Models;
using MoviesCRUD.ViewModels;
using NToastNotify;

namespace MoviesCRUD.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IToastNotification _toastNotification; //toast notify lib

        private List<string> allowedExt = new List<string> { ".jpg", ".png", ".jpeg" };
        private long maxPosterSize = 1048578;

        public MoviesController(AppDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification; // inject toast in ctor
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Rate).ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync(),
            };
            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model); // return view with error
            }

            var files = Request.Form.Files; // get all files attached to form

            if (!files.Any()) // if there is no files with form
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please Add Movie Poster");
                return View("MovieForm", model);
            }

            var poster = files.FirstOrDefault();

            if (!allowedExt.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only jpg and png images allowed");
                return View("MovieForm", model);
            }

            if (poster.Length > maxPosterSize) //file size, 1048578 = 1MB
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Image Can not be more than 1MB");
                return View("MovieForm", model);
            }

            using var dataStream = new MemoryStream(); //create stream to save file into and using to close stream auto
            await poster.CopyToAsync(dataStream);

            var movies = new Movie
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Storyline = model.Storyline,
                Rate = model.Rate,
                Poster = dataStream.ToArray(),
            };

            _context.Movies.Add(movies);
            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie Created Successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                GenreId = movie.GenreId,
                Rate = movie.Rate,
                Storyline = movie.Storyline,
                Poster = movie.Poster,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync(),
            };

            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("MovieForm", model); // return view with error
            }

            var movie = await _context.Movies.FindAsync(model.Id);
            if (movie == null)
            {
                return NotFound();
            }

            var files = Request.Form.Files;
            if (files.Any()) // if there is  files with form
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray(); //to render image if error happen in next two ifs

                ////function this part bcs of duplication
                if (!allowedExt.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only jpg and png images allowed");
                    return View("MovieForm", model);
                }

                if (poster.Length > maxPosterSize) //file size, 1048578 = 1MB
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Image Can not be more than 1MB");
                    return View("MovieForm", model);
                }

                movie.Poster = model.Poster;
            }

            movie.Title = model.Title;
            movie.Year = model.Year;
            movie.GenreId = model.GenreId;
            movie.Rate = model.Rate;
            movie.Storyline = model.Storyline;

            _context.SaveChanges();

            _toastNotification.AddSuccessToastMessage("Movie Edited Successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.Include(m => m.Genre) //include genre to use it in details view || find does not work with include
                                            .SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok(); // to send success to ajax in index script
        }
    }
}
