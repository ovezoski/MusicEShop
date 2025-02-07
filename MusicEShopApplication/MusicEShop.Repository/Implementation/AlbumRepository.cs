using Microsoft.EntityFrameworkCore;
using MusicEShop.Domain.DomainModels;
using MusicEShop.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Repository.Implementation
{
    public class AlbumRepository : Repository<Album>, IAlbumRepository
    {
        public AlbumRepository(ApplicationDbContext context) : base(context) { }

        public List<Album> GetAlbumsByArtistId(Guid artistId)
        {
            return context.Albums.Where(a => a.ArtistId == artistId).ToList();
        }
    }
}
