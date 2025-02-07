using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicEShop.Domain.DomainModels;
using MusicEShop.Repository;
using MusicEShop.Service.Interface;

namespace MusicEShop.Web.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly IAlbumService _albumService;
        private readonly IArtistService _artistService;
        public AlbumsController(IAlbumService albumService, IArtistService artistService)
        {
            _albumService = albumService;
            _artistService = artistService;
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
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_artistService.GetAllArtists(), "Id", "Name");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Genre,ReleaseDate,coverImage,Price,ArtistId,Id")] Album album, IFormFile coverImage)
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
            ViewData["ArtistId"] = new SelectList(_artistService.GetAllArtists(), "Id", "Id", album.ArtistId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Title,Genre,ReleaseDate,Price,CoverImage,ArtistId,Id")] Album album)
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
            ViewData["ArtistId"] = new SelectList(_artistService.GetAllArtists(), "Id", "Id", album.ArtistId);
            return View(album);
        }

        // GET: Albums/Delete/5
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
            var artists= _artistService.GetAllArtists();
            var allAlbumsByArtists = artists.FindAll(i => i.Albums.All(a => a.ArtistId.Equals(artistId)));
            return View(allAlbumsByArtists);
        }
        private bool AlbumExists(Guid id)
        {
            return _albumService.GetAllAlbums().Any(e => e.Id == id);
        }
    }
}
