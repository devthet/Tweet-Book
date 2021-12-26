using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tweet_Book.Domain;
using Tweetbook.Domain;

namespace Tweet_Book.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<PostTag>()
            //    .HasOne(p => p.Post)
            //    .WithMany(t => t.Tags)
            //    .HasForeignKey(k => k.PostId);
            //builder.Entity<PostTag>()
            //     .HasOne(t => t.Tag)
            //     .WithMany(pt => pt.Tags)
            //     .HasForeignKey(k => k.CreatorId);
            base.OnModelCreating(builder);
            builder.Entity<PostTag>()
                .Ignore(xx => xx.Post)
                .HasKey(x => new { x.PostId, x.TagName });

        }

    }
}
