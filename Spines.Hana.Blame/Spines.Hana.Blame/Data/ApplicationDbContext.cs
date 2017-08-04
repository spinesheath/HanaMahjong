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


      base.OnModelCreating(builder);
      // Customize the ASP.NET Identity model and override the defaults if needed.
      // For example, you can rename the ASP.NET Identity table names and more.
      // Add your customizations after calling base.OnModelCreating(builder);
    }
  }
}