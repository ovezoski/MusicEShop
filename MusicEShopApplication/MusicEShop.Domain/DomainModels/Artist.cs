using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Domain.DomainModels
{
    public class Artist : BaseEntity
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Genre { get; set; }

        // One-to-Many: Artist → Albums
        public virtual ICollection<Album>? Albums { get; set; }

        // Many-to-Many: Artist ↔ Tracks
        public virtual ICollection<ArtistTrack>? ArtistTracks { get; set; }
    }
}
