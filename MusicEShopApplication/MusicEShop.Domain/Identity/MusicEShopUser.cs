using Microsoft.AspNetCore.Identity;
using MusicEShop.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Domain.Identity
{
    public class MusicEShopUser : IdentityUser
    {
        public string name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // One-to-Many: User → Playlists
        public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
