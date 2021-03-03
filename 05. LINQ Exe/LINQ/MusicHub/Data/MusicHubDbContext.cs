using MusicHub.Data.Models;

namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }

        public DbSet<Writer> Writers { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Producer> Producers { get; set; }

        public DbSet<SongPerformer> SongsPerformers { get; set; }

        public DbSet<Performer> Performers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder); // not sure if needed
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<SongPerformer>(songperformer =>
            {
                songperformer.HasKey(sp => new
                {
                    sp.PerformerId,
                    sp.SongId
                });
            });
        }
    }
}
