using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fall2025_Project3_nchubbell.Data;
using Fall2025_Project3_nchubbell.Models;
using Fall2025_Project3_nchubbell.Models.ViewModels;
using Fall2025_Project3_nchubbell.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VaderSharp2;

namespace Fall2025_Project3_nchubbell.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIReviewService _aiReviewService;

        public MoviesController(ApplicationDbContext context, IAIReviewService aiReviewService)
        {
            _context = context;
            _aiReviewService = aiReviewService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.ActorMovies)
                    .ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var aiReviewsRaw = await _aiReviewService.GenerateMovieReviewsAsync(
                movie.Title,
                null,          
                reviewCount: 10);

            var aiReviews = aiReviewsRaw
                .Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            var analyzer = new SentimentIntensityAnalyzer();
            var reviewViewModels = new List<ReviewWithSentiment>();

            foreach (var text in aiReviews)
            {
                var scores = analyzer.PolarityScores(text);

                string label;
                if (scores.Compound >= 0.05)
                {
                    label = "Positive";
                }
                else if (scores.Compound <= -0.05)
                {
                    label = "Negative";
                }
                else
                {
                    label = "Neutral";
                }

                reviewViewModels.Add(new ReviewWithSentiment
                {
                    Text = text,
                    Compound = scores.Compound,
                    Positive = scores.Positive,
                    Negative = scores.Negative,
                    Neutral = scores.Neutral,
                    Label = label
                });
            }

            double overallCompound = reviewViewModels.Any()
                ? reviewViewModels.Average(r => r.Compound)
                : 0.0;

            string overallLabel;
            if (overallCompound >= 0.05)
            {
                overallLabel = "Overall Positive";
            }
            else if (overallCompound <= -0.05)
            {
                overallLabel = "Overall Negative";
            }
            else
            {
                overallLabel = "Overall Neutral";
            }

            var viewModel = new MovieDetailsViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Genre = movie.Genre,
                Year = movie.Year,
                ImdbUrl = movie.ImdbUrl,
                Poster = movie.Poster,
                Actors = movie.ActorMovies
                    .Select(am => am.Actor)
                    .Where(a => a != null)
                    .ToList(),
                Reviews = reviewViewModels,
                OverallCompoundSentiment = overallCompound,
                OverallSentimentLabel = overallLabel
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Year,ImdbUrl,Poster")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Year,ImdbUrl,Poster")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
