using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicEShop.Domain.DomainModels;
using MusicEShop.Repository;
using MusicEShop.Service.Interface;

namespace MusicEShop.Web.Controllers
{
    [Authorize]
    public class TracksController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IAlbumService _albumService;
        private readonly ICartService _cartService;

        public TracksController(ITrackService trackService, IAlbumService albumService, ICartService cartService)
        {
            this._trackService = trackService;
            _albumService = albumService;
            _cartService = cartService;
        }

        // GET: Tracks
        public IActionResult Index()
        {

            var tracks=_trackService.GetAllTracks();
            
            
            return View(tracks);
        }

        // GET: Tracks/Details/5
        public IActionResult Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = _trackService.GetTrackById(id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // GET: Tracks/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_albumService.GetAllAlbums(), "Id", "Title");
            return View();
        }

        // POST: Tracks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Duration,Price,AlbumId,Id")] Track track)
        {
            if (ModelState.IsValid)
            {
                track.Id = Guid.NewGuid();
                track.Album=_albumService.GetAlbumById(track.AlbumId);
                _trackService.CreateTrack(track);
                
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_albumService.GetAllAlbums(), "Id", "Title", track.AlbumId);
            return View(track);
        }

        // GET: Tracks/Edit/5
        public IActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = _trackService.GetTrackById(id);
            if (track == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_albumService.GetAllAlbums(), "Id", "Title", track.AlbumId);
            return View(track);
        }

        // POST: Tracks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Title,Duration,Price,AlbumId,Id")] Track track)
        {
            if (id != track.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _trackService.UpdateTrack(track);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackExists(track.Id))
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
            ViewData["AlbumId"] = new SelectList(_albumService.GetAllAlbums(), "Id", "Title", track.AlbumId);
            return View(track);
        }

        // GET: Tracks/Delete/5
        public IActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = _trackService.GetTrackById(id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var track = _trackService.GetTrackById(id);
            if (track != null)
            {
                _trackService.DeleteTrack(id);  
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult AddToCart(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = _trackService.GetTrackById(id);

            CartItem ps = new CartItem();

            if (_trackService != null)
            {
                ps.TrackId = track.Id;
            }

            return View(ps);
        }

        [HttpPost]
        public IActionResult AddToCartConfirmed(CartItem model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _cartService.AddToShoppingConfirmed(model, userId);



            return RedirectToAction(nameof(Index));
        }
        private bool TrackExists(Guid id)
        {
            return _trackService.GetAllTracks().Any(e => e.Id == id);
        }
    }
}
