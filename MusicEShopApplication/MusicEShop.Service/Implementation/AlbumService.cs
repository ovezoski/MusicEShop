using MusicEShop.Domain.DomainModels;
using MusicEShop.Repository.Interface;
using MusicEShop.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Service.Implementation
{
    public class AlbumService : IAlbumService
    {
        private readonly IRepository<Album> _albumRepository;

        public AlbumService(IRepository<Album> albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public void CreateAlbum(Album album)
        {
            _albumRepository.Insert(album);
        }

        public void DeleteAlbum(Guid id)
        {
            var album = _albumRepository.GetById(id);
            _albumRepository.Delete(album);
        }

        public Album GetAlbumById(Guid id)
        {
            var album = _albumRepository.GetById(id);
            return album;
        }

        public List<Album> GetAlbumsByArtist(Guid artistId)
        {
            var allAlbums = _albumRepository.GetAll();

            var albumsByArtist = allAlbums.Where(album => album.ArtistId == artistId).ToList();

            return albumsByArtist;
        }

        public List<Album> GetAllAlbums()
        {
            return _albumRepository.GetAll().ToList();
        }

        public void UpdateAlbum(Album album)
        {
            _albumRepository.Update(album);
        }
    }
}
