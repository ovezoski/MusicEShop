using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Domain.DomainModels
{
    public class Album
    {
        [Key]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }

        // Foreign Key for Artist (One-to-Many)
        public Guid ArtistId { get; set; }
        public Artist? Artist { get; set; }

        // One-to-Many: Album → Tracks
        public virtual ICollection<Track>? Tracks { get; set; }

    }
}
