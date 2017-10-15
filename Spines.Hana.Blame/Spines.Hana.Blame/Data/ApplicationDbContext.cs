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
    public DbSet<Match> Matches { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<RuleSet> RuleSets { get; set; }
    public DbSet<Room> Rooms { get; set; }

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
      builder.Entity<Player>().Property(p => p.Name).HasMaxLength(32).IsRequired();
      builder.Entity<Player>().HasIndex(p => p.Name).IsUnique();
      builder.Entity<Player>().HasMany(p => p.Participants);

      builder.Entity<Match>().ToTable("Match").HasKey(m => m.Id);
      builder.Entity<Match>().Property(m => m.ContainerName).HasMaxLength(16).IsRequired();
      builder.Entity<Match>().Property(m => m.FileName).HasMaxLength(64).IsRequired();
      builder.Entity<Match>().Property(m => m.UploadTime).IsRequired();
      builder.Entity<Match>().Property(m => m.CreationTime).IsRequired();
      builder.Entity<Match>().Property(m => m.RuleSetId).IsRequired();
      builder.Entity<Match>().Property(m => m.RoomId).IsRequired();
      builder.Entity<Match>().HasIndex(m => new { m.ContainerName, m.FileName }).IsUnique();
      builder.Entity<Match>().HasMany(m => m.Games);
      builder.Entity<Match>().HasMany(m => m.Participants);

      builder.Entity<Participant>().ToTable("Participant").HasKey(p => p.Id);
      builder.Entity<Participant>().Property(p => p.Seat).IsRequired();
      builder.Entity<Participant>().Property(p => p.PlayerId).IsRequired();
      builder.Entity<Participant>().Property(p => p.MatchId).IsRequired();
      builder.Entity<Participant>().HasIndex(p => new { p.PlayerId, p.MatchId }).IsUnique();
      builder.Entity<Participant>().HasIndex(p => new { p.MatchId, p.Seat }).IsUnique();

      builder.Entity<Game>().ToTable("Game");
      builder.Entity<Game>().HasKey(g => g.Id);
      builder.Entity<Game>().Property(g => g.MatchId).IsRequired();
      builder.Entity<Game>().Property(g => g.Index).IsRequired();
      builder.Entity<Game>().Property(g => g.FrameCount).IsRequired();
      builder.Entity<Game>().HasIndex(g => new { g.MatchId, g.Index }).IsUnique();

      builder.Entity<RuleSet>().ToTable("RuleSet");
      builder.Entity<RuleSet>().HasKey(r => r.Id);
      builder.Entity<RuleSet>().Property(r => r.Aka).IsRequired();
      builder.Entity<RuleSet>().Property(r => r.ExtraSecondsPerGame).IsRequired();
      builder.Entity<RuleSet>().Property(r => r.Kuitan).IsRequired();
      builder.Entity<RuleSet>().Property(r => r.Name).HasMaxLength(64).IsRequired();
      builder.Entity<RuleSet>().Property(r => r.PlayerCount).IsRequired();
      builder.Entity<RuleSet>().Property(r => r.Rounds).IsRequired();
      builder.Entity<RuleSet>().Property(r => r.SecondsPerAction).IsRequired();
      builder.Entity<RuleSet>().HasIndex(r => r.Name).IsUnique();

      builder.Entity<Room>().ToTable("Room");
      builder.Entity<Room>().HasKey(r => r.Id);
      builder.Entity<Room>().Property(r => r.Name).HasMaxLength(64).IsRequired();
      builder.Entity<Room>().HasIndex(r => r.Name).IsUnique();

      base.OnModelCreating(builder);
    }
  }
}