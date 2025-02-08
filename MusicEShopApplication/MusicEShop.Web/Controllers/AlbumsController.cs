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
    public class AlbumsController : Controller
    {
        private readonly IAlbumService _albumService;
        private readonly IArtistService _artistService;
        private readonly ICartService _cartService;
        public AlbumsController(IAlbumService albumService, IArtistService artistService, ICartService cartService)
        {
            _albumService = albumService;
            _artistService = artistService;
            _cartService = cartService;
        }

        //List<Album> GetAllAlbums();
        //Album GetAlbumById(Guid id);
        //void CreateAlbum(Album album);
        //void UpdateAlbum(Album album);
        //void DeleteAlbum(Guid id);
        //List<Album> GetAlbumsByArtist(Guid artistId);
        // GET: Albums
        public IActionResult Index()
        {
            var albums = _albumService.GetAllAlbums();
            foreach (var album in albums)
            {
                album.Artist = _artistService.GetArtistById(album.ArtistId);
            }

            return View(albums);
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = _albumService.GetAlbumById(id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_artistService.GetAllArtists(), "Id", "Name");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Genre,Details,ReleaseDate,coverImage,Price,ArtistId,Id")] Album album, IFormFile coverImage)
        {
            if (ModelState.IsValid)
            {
                album.Id = Guid.NewGuid();

                if (coverImage != null && coverImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = album.Title+"-"+_artistService.GetArtistById(album.ArtistId).Name + Path.GetExtension(coverImage.FileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        coverImage.CopyTo(fileStream);
                    }

                    album.CoverImage = "/img/" + uniqueFileName;
                }
                _albumService.CreateAlbum(album);

                return RedirectToAction(nameof(Index));
            }

            ViewData["ArtistId"] = new SelectList(_artistService.GetAllArtists(), "Id", "Id", album.ArtistId);
            return View(album);
        }

        // GET: Albums/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult  Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = _albumService.GetAlbumById(id);
            if (album == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_artistService.GetAllArtists(), "Id", "Name", album.ArtistId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid id, [Bind("Title,Genre,Details,ReleaseDate,Price,CoverImage,ArtistId,Id")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _albumService.UpdateAlbum(album);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
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
            ViewData["ArtistId"] = new SelectList(_artistService.GetAllArtists(), "Id", "Name", album.ArtistId);
            return View(album);
        }

        // GET: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = _albumService.GetAlbumById(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var album = _albumService.GetAlbumById(id);
            if (album != null)
            {
                _albumService.DeleteAlbum(id);
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult ListAlbumsByArtist(Guid artistId)
        {
            var albums = _albumService.GetAlbumsByArtist(artistId);

            if (albums == null || !albums.Any())
            {
                return NotFound("No albums found for this artist.");
            }

            return View(albums);
        }

        public IActionResult AddToCart(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = _albumService.GetAlbumById(id);

            CartItem ps = new CartItem();

            if (album != null)
            {
                ps.AlbumId = album.Id;
            }

            return View(ps);
        }

        [HttpPost]
        public IActionResult AddToCartConfirmed(CartItem model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _cartService.AddToShoppingConfirmed(model, userId);



            return View("Index", _albumService.GetAllAlbums());
        }


        private bool AlbumExists(Guid id)
        {
            return _albumService.GetAllAlbums().Any(e => e.Id == id);
        }
    }
}
