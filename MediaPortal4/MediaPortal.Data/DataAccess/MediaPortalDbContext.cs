namespace MediaPortal.Data.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using MediaPortal.Data.EntitiesModel;

    public partial class MediaPortalDbContext : DbContext
    {
        public MediaPortalDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public virtual DbSet<FileSystem> FileSystems { get; set; }
        public virtual DbSet<FileTag> FileTags { get; set; }
        public virtual DbSet<SharedAccess> SharedAccesses { get; set; }        
        public virtual DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSystem>()
                .HasMany(e => e.FileSystem1)
                .WithOptional(e => e.FileSystem2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<FileSystem>()
                .HasMany(e => e.FileTags)
                .WithRequired(e => e.FileSystem)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FileSystem>()
                .HasMany(e => e.SharedAccesses)
                .WithRequired(e => e.FileSystem)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tag>()
                .HasMany(e => e.FileTags)
                .WithRequired(e => e.Tag)
                .WillCascadeOnDelete(false);
        }
    }
}
