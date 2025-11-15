using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fall2025_Project3_nchubbell.Data;
using Fall2025_Project3_nchubbell.Models;
using Fall2025_Project3_nchubbell.Models.ViewModels;
using Fall2025_Project3_nchubbell.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VaderSharp2;

namespace Fall2025_Project3_nchubbell.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAIReviewService _aiReviewService;

        public ActorsController(ApplicationDbContext context, IAIReviewService aiReviewService)
        {
            _context = context;
            _aiReviewService = aiReviewService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .Include(a => a.ActorMovies)
                    .ThenInclude(am => am.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            var aiTweetsRaw = await _aiReviewService.GenerateActorTweetsAsync(
                actor.Name,
                tweetCount: 20);

            var aiTweets = aiTweetsRaw
                .Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            var analyzer = new SentimentIntensityAnalyzer();
            var tweetViewModels = new List<ReviewWithSentiment>();

            foreach (var text in aiTweets)
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

                tweetViewModels.Add(new ReviewWithSentiment
                {
                    Text = text,
                    Compound = scores.Compound,
                    Positive = scores.Positive,
                    Negative = scores.Negative,
                    Neutral = scores.Neutral,
                    Label = label
                });
            }

            double overallCompound = tweetViewModels.Any()
                ? tweetViewModels.Average(t => t.Compound)
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

            var viewModel = new ActorDetailsViewModel
            {
                Id = actor.Id,
                Name = actor.Name,
                Gender = actor.Gender,
                Age = actor.Age,
                ImdbUrl = actor.ImdbUrl,
                Photo = actor.Photo,
                Movies = actor.ActorMovies
                    .Select(am => am.Movie)
                    .Where(m => m != null)
                    .ToList(),
                Tweets = tweetViewModels,
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
        public async Task<IActionResult> Create(Actor actor, IFormFile photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await photoFile.CopyToAsync(ms);
                actor.Photo = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(actor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,ImdbUrl,Photo")] Actor actor)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
    }
}
