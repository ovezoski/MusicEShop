using MusicEShop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Service.Interface
{
    public interface IAlbumService
    {
        List<Album> GetAllAlbums();
        Album GetAlbumById(Guid id);
        void CreateAlbum(Album album);
        void UpdateAlbum(Album album);
        void DeleteAlbum(Guid id);
        List<Album> GetAlbumsByArtist(Guid artistId);
    }
}
