using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicEShop.Domain.DomainModels;
using MusicEShop.Domain.Identity;
namespace MusicEShop.Repository
{
    public class ApplicationDbContext : IdentityDbContext<MusicEShopUser>
    {
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Track> Tracks { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }
        public virtual DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public virtual DbSet<ArtistTrack> ArtistTracks { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistTrack>()
                .HasOne(at => at.Artist)
                .WithMany(a => a.ArtistTracks)
                .HasForeignKey(at => at.ArtistId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ArtistTrack>()
                .HasOne(at => at.Track)
                .WithMany(t => t.ArtistTracks)
                .HasForeignKey(at => at.TrackId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Album>(b =>
            {
                b.HasMany(a => a.Tracks)
                    .WithOne(t => t.Album)
                    .HasForeignKey(t => t.AlbumId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(a => a.OrderItems)  // Configure cascade delete for OrderItems
                .WithOne(oi => oi.Album)
                .HasForeignKey(oi => oi.AlbumId)
                .OnDelete(DeleteBehavior.Cascade); //

            });

            modelBuilder.Entity<OrderItem>(b =>
            {
                b.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(oi => oi.Track)
                    .WithMany()
                    .HasForeignKey(oi => oi.TrackId)
                    .OnDelete(DeleteBehavior.Cascade); 

            });





            base.OnModelCreating(modelBuilder);
        }
    }
}