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
    public class ArtistRepository : Repository<Artist>, IArtistRepository
    {
        public ArtistRepository(ApplicationDbContext context) : base(context) { }

        public List<Artist> GetArtistsWithAlbums()
        {
            return context.Artists.Include(a => a.Albums).ToList();
        }
    }
}
