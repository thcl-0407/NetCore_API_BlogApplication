using Microsoft.EntityFrameworkCore;
using System;
using DAL.Entities;

namespace DAL
{
    public class BlogApplicationDbContext:DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<ImageGallery> ImageGalleries { get; set; }

        public BlogApplicationDbContext(DbContextOptions<BlogApplicationDbContext> options):base(options) { }

        #region For Testing
        public BlogApplicationDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=GLENPC; Database=BlogDB_API; Integrated Security=true;");
        }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Set Default Value For isAuthenticated in User's Table
            modelBuilder.Entity<User>().Property(u => u.isAuthenticated).HasDefaultValue(false);

            //Set Default Value For DateCreate in Comment's Table
            modelBuilder.Entity<Comment>().Property(c => c.DateCreate).HasDefaultValue(DateTime.Now);

            //Set Foreign Key For Post's Table
            modelBuilder.Entity<Post>().HasOne<User>(p => p.User).WithMany(u => u.Posts).HasForeignKey(p => p.UserID);
            modelBuilder.Entity<Post>().HasOne<ImageGallery>(p => p.ImageGallery).WithMany(ig => ig.Posts).HasForeignKey(p => p.ImageID);

            //Set Foregin Key For Token's Table
            modelBuilder.Entity<Token>().HasOne<User>(t => t.User).WithOne(u => u.Token).HasForeignKey<Token>(t => t.UserID);

            //Set Primary Key For PostComment's Table
            modelBuilder.Entity<PostComment>().HasKey(pc => new { pc.PostID, pc.CommentID });
            modelBuilder.Entity<PostComment>().HasOne<Post>(pc => pc.Posts).WithMany(p => p.PostComments).HasForeignKey(p => p.PostID);
            modelBuilder.Entity<PostComment>().HasOne<Comment>(pc => pc.Comments).WithMany(c => c.PostComments).HasForeignKey(p => p.CommentID);
        }
    }
}
