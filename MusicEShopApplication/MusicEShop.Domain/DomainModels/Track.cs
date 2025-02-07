using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Domain.DomainModels
{
    public class Track : BaseEntity
    {
        public string? Title { get; set; }
        public int? Duration { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Foreign Key for Album (One-to-Many)
        public Guid AlbumId { get; set; }
        public Album? Album { get; set; }

        // Many-to-Many: Track ↔ Artists
        public virtual ICollection<ArtistTrack>? ArtistTracks { get; set; }

        // Many-to-Many: Track ↔ Playlists
        public virtual ICollection<PlaylistTrack>? PlaylistTracks { get; set; }

    }
}
