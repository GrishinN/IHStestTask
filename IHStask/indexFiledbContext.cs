using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IHStask
{
    public partial class indexFiledbContext : DbContext
    {
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Inverse> Inverse { get; set; }
        public virtual DbSet<Word> Words { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=localhost;Database=indexFiledb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.DocId);

                entity.Property(e => e.DocId).HasColumnName("Doc_Id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nvchar(200)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("nvchar(20)");
            });

            modelBuilder.Entity<Inverse>(entity =>
            {
                entity.Property(e => e.DocId).HasColumnName("Doc_Id");

                entity.Property(e => e.WordId).HasColumnName("Word_Id");

                entity.HasOne(d => d.Doc)
                    .WithMany(p => p.Inverse)
                    .HasForeignKey(d => d.DocId)
                    .HasConstraintName("FK_Doc_Id");

                entity.HasOne(d => d.Word)
                    .WithMany(p => p.Inverse)
                    .HasForeignKey(d => d.WordId)
                    .HasConstraintName("FK_Word_Id");
            });

            modelBuilder.Entity<Word>(entity =>
            {
                entity.HasKey(e => e.WordId);

                entity.Property(e => e.WordId).HasColumnName("Word_Id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("nvchar(200)");
            });
        }
    }
}
