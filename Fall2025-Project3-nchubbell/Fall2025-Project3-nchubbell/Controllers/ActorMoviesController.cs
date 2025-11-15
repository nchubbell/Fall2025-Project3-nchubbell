using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_nchubbell.Data;
using Fall2025_Project3_nchubbell.Models;

namespace Fall2025_Project3_nchubbell.Controllers
{
    public class ActorMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActorMovies
                .Include(a => a.Actor)
                .Include(a => a.Movie);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? actorId, int? movieId)
        {
            if (actorId == null || movieId == null)
            {
                return NotFound();
            }

            var actorMovie = await _context.ActorMovies
                .Include(a => a.Actor)
                .Include(a => a.Movie)
                .FirstOrDefaultAsync(m =>
                    m.ActorId == actorId &&
                    m.MovieId == movieId);

            if (actorMovie == null)
            {
                return NotFound();
            }

            return View(actorMovie);
        }

        public IActionResult Create()
        {
            ViewData["ActorId"] = new SelectList(_context.Actors, "ActorId", "Name");
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActorId,MovieId")] ActorMovie actorMovie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actorMovie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActorId"] = new SelectList(_context.Actors, "ActorId", "Name", actorMovie.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", actorMovie.MovieId);
            return View(actorMovie);
        }

        public async Task<IActionResult> Edit(int? actorId, int? movieId)
        {
            if (actorId == null || movieId == null)
            {
                return NotFound();
            }

            var actorMovie = await _context.ActorMovies
                .FirstOrDefaultAsync(m =>
                    m.ActorId == actorId &&
                    m.MovieId == movieId);

            if (actorMovie == null)
            {
                return NotFound();
            }

            ViewData["ActorId"] = new SelectList(_context.Actors, "ActorId", "Name", actorMovie.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", actorMovie.MovieId);
            return View(actorMovie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int actorId, int movieId, [Bind("ActorId,MovieId")] ActorMovie actorMovie)
        {
            if (actorId != actorMovie.ActorId || movieId != actorMovie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actorMovie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorMovieExists(actorMovie.ActorId, actorMovie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActorId"] = new SelectList(_context.Actors, "ActorId", "Name", actorMovie.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "Title", actorMovie.MovieId);
            return View(actorMovie);
        }

        public async Task<IActionResult> Delete(int? actorId, int? movieId)
        {
            if (actorId == null || movieId == null)
            {
                return NotFound();
            }

            var actorMovie = await _context.ActorMovies
                .Include(a => a.Actor)
                .Include(a => a.Movie)
                .FirstOrDefaultAsync(m =>
                    m.ActorId == actorId &&
                    m.MovieId == movieId);

            if (actorMovie == null)
            {
                return NotFound();
            }

            return View(actorMovie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int actorId, int movieId)
        {
            var actorMovie = await _context.ActorMovies.FindAsync(actorId, movieId);
            if (actorMovie != null)
            {
                _context.ActorMovies.Remove(actorMovie);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ActorMovieExists(int actorId, int movieId)
        {
            return _context.ActorMovies
                .Any(e => e.ActorId == actorId && e.MovieId == movieId);
        }
    }
}
