using Microsoft.AspNetCore.Mvc;
using shockz.msa.movie.client.Models;
using shockz.msa.movie.client.Services;

namespace shockz.msa.movie.client.Controllers
{
  public class MoviesController : Controller
  {
    private readonly IMovieApiService _movieApiService;

    public MoviesController(IMovieApiService movieApiService)
    {
      _movieApiService = movieApiService ?? throw new ArgumentNullException(nameof(movieApiService));
    }

    // GET: Movies
    public async Task<IActionResult> Index()
    {
      //return _context.Movies != null ?
      //            View(await _context.Movies.ToListAsync()) :
      //            Problem("Entity set 'MovieContext.Movie'  is null.");
      return View(await _movieApiService.GetMovies());
    }

    // GET: Movies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      //if (id == null || _context.Movies == null) {
      //  return NotFound();
      //}

      //var movie = await _context.Movies
      //    .FirstOrDefaultAsync(m => m.Id == id);
      //if (movie == null) {
      //  return NotFound();
      //}

      //return View(movie);

      return View();
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Movies/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Genre,Rating,ReleaseDate,ImageUrl,Owner")] Movie movie)
    {
      //if (ModelState.IsValid) {
      //  _context.Add(movie);
      //  await _context.SaveChangesAsync();
      //  return RedirectToAction(nameof(Index));
      //}
      //return View(movie);

      return View();
    }

    // GET: Movies/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      //if (id == null || _context.Movies == null) {
      //  return NotFound();
      //}

      //var movie = await _context.Movies.FindAsync(id);
      //if (movie == null) {
      //  return NotFound();
      //}
      //return View(movie);

      return View();
    }

    // POST: Movies/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Rating,ReleaseDate,ImageUrl,Owner")] Movie movie)
    {
      //if (id != movie.Id) {
      //  return NotFound();
      //}

      //if (ModelState.IsValid) {
      //  try {
      //    _context.Update(movie);
      //    await _context.SaveChangesAsync();
      //  }
      //  catch (DbUpdateConcurrencyException) {
      //    if (!MovieExists(movie.Id)) {
      //      return NotFound();
      //    } else {
      //      throw;
      //    }
      //  }
      //  return RedirectToAction(nameof(Index));
      //}
      //return View(movie);

      return View();
    }

    // GET: Movies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      //if (id == null || _context.Movies == null) {
      //  return NotFound();
      //}

      //var movie = await _context.Movies
      //    .FirstOrDefaultAsync(m => m.Id == id);
      //if (movie == null) {
      //  return NotFound();
      //}

      //return View(movie);

      return View();
    }

    // POST: Movies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      //if (_context.Movies == null) {
      //  return Problem("Entity set 'MovieContext.Movies'  is null.");
      //}
      //var movie = await _context.Movies.FindAsync(id);
      //if (movie != null) {
      //  _context.Movies.Remove(movie);
      //}

      //await _context.SaveChangesAsync();
      //return RedirectToAction(nameof(Index));

      return View();
    }

    private bool MovieExists(int id)
    {
      //return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();

      return true;
    }
  }
}
