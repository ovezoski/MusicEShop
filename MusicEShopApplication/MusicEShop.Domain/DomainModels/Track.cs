using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Domain.DomainModels
{
    public class Track
    {
        [Key]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public int? Duration { get; set; } // Duration in seconds

        // Foreign Key for Album (One-to-Many)
        public Guid AlbumId { get; set; }
        public Album? Album { get; set; }

        // Many-to-Many: Track ↔ Artists
        public virtual ICollection<ArtistTrack>? ArtistTracks { get; set; }

        // Many-to-Many: Track ↔ Playlists
        public virtual ICollection<PlaylistTrack>? PlaylistTracks { get; set; }

    }
}
