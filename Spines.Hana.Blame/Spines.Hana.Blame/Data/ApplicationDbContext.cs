// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spines.Hana.Blame.Models;

namespace Spines.Hana.Blame.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
    {
    }

    public DbSet<Thread> Threads { get; set; }
    public DbSet<WwydThread> WwydThreads { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public DbSet<Player> Players { get; set; }
    public DbSet<Match> Matchs { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.Entity<Thread>().ToTable("Thread").HasKey(t => t.Id);
      builder.Entity<Thread>().HasMany(t => t.Comments);

      builder.Entity<WwydThread>().ToTable("WwydThread");
      builder.Entity<WwydThread>().Property(t => t.Hand).IsRequired();

      builder.Entity<Comment>().ToTable("Comment").HasKey(c => c.Id);
      builder.Entity<Comment>().Property(c => c.Time).IsRequired();
      builder.Entity<Comment>().Property(c => c.Message).IsRequired();
      builder.Entity<Comment>().Property(c => c.UserId).IsRequired();
      builder.Entity<Comment>().Property(c => c.ThreadId).IsRequired();



      builder.Entity<Player>().ToTable("Player").HasKey(p => p.Id);
      builder.Entity<Player>().Property(p => p.Name).IsRequired();
      builder.Entity<Player>().HasIndex(p => p.Name).IsUnique();
      builder.Entity<Player>().HasMany(p => p.Participants);

      builder.Entity<Match>().ToTable("Match").HasKey(m => m.Id);
      builder.Entity<Match>().Property(m => m.ContainerName).IsRequired();
      builder.Entity<Match>().Property(m => m.FileName).IsRequired();
      builder.Entity<Match>().Property(m => m.Hash).IsRequired();
      builder.Entity<Match>().Property(m => m.UploadTime).IsRequired();
      builder.Entity<Match>().HasIndex(m => new {m.ContainerName, m.FileName}).IsUnique();
      builder.Entity<Match>().HasMany(m => m.Games);
      builder.Entity<Match>().HasMany(m => m.Participants);

      builder.Entity<Participant>().ToTable("Participant");
      builder.Entity<Participant>().Property(p => p.Match).IsRequired();
      builder.Entity<Participant>().Property(p => p.Player).IsRequired();
      builder.Entity<Participant>().Property(p => p.Seat).IsRequired();
      builder.Entity<Participant>().HasIndex(p => new {p.Player, p.Match}).IsUnique();
      builder.Entity<Participant>().HasIndex(p => new {p.Match, p.Seat}).IsUnique();

      builder.Entity<Game>().ToTable("Game");
      builder.Entity<Game>().HasKey(g => g.Id);
      builder.Entity<Game>().Property(g => g.Match).IsRequired();
      builder.Entity<Game>().Property(g => g.Index).IsRequired();
      builder.Entity<Game>().Property(g => g.FrameCount).IsRequired();
      builder.Entity<Game>().HasIndex(g => new {g.Match, g.Index}).IsUnique();



      base.OnModelCreating(builder);
    }
  }
}