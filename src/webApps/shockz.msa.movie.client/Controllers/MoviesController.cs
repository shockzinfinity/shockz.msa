using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using shockz.msa.movie.client.Models;
using shockz.msa.movie.client.Services;
using System.Diagnostics;

namespace shockz.msa.movie.client.Controllers
{
  [Authorize]
  public class MoviesController : Controller
  {
    private readonly IMovieApiService _movieApiService;
    private readonly IUserService _userService;

    public MoviesController(IMovieApiService movieApiService, IUserService userService)
    {
      _movieApiService = movieApiService ?? throw new ArgumentNullException(nameof(movieApiService));
      _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    // GET: Movies
    public async Task<IActionResult> Index()
    {
      var movies = await _movieApiService.GetMovies();
      movies = FilterMovies(movies.ToList());

      return View(movies);
    }

    public async Task LogTokenAndClaims()
    {
      var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

      Debug.WriteLine($"Identity token: {identityToken}");

      foreach (var claim in User.Claims) {
        Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
      }
    }

    public async Task Logout()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    }

    // GET: Movies/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      return View(await _movieApiService.GetMovie(id.Value));
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
    public async Task<IActionResult> Create([Bind(shockz.msa.common.Constant.Movies_Controller_Bind_Attribute)] Movie movie)
    {
      await _movieApiService.CreateMovie(movie);

      return RedirectToAction(shockz.msa.common.Constant.Movies_Controller_Action_Index);
    }

    // GET: Movies/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      return View(await _movieApiService.GetMovie(id.Value));
    }

    // POST: Movies/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind(shockz.msa.common.Constant.Movies_Controller_Bind_Attribute)] Movie movie)
    {
      var response = await _movieApiService.UpdateMovie(id, movie);

      if (response) {
        return RedirectToAction(shockz.msa.common.Constant.Movies_Controller_Action_Index);
      } else {
        Debug.WriteLine($"An error occurred");
        return View();
      }
    }

    // GET: Movies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      return View(await _movieApiService.GetMovie(id.Value));
    }

    // POST: Movies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var response = await _movieApiService.DeleteMovice(id);

      if (response) {
        return RedirectToAction(shockz.msa.common.Constant.Movies_Controller_Action_Index);
      } else {
        Debug.WriteLine($"An error occurred");
        return View();
      }
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> OnlyAdmin()
    {
      await LogTokenAndClaims();
      var userInfo = await _userService.GetUserInfo();

      return View(userInfo);
    }

    private List<Movie> FilterMovies(List<Movie> movies) => movies.FindAll(m => m.Owner.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase));
  }
}
