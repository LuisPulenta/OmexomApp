using GenericApp.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GenericApp.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Obra> Obras { get; set; }
        public DbSet<ObrasDocumento> ObrasDocumentos { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<CityEntity> Cities { get; set; }
        public DbSet<CountryEntity> Countries { get; set; }
        public DbSet<DepartmentEntity> Departments { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }
        public DbSet<StateEntity> States { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryEntity>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<CountryEntity>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<DepartmentEntity>(dep =>
            {
                dep.HasIndex("Name", "CountryId").IsUnique();
                dep.HasOne(d => d.Country).WithMany(c => c.Departments).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductEntity>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<CityEntity>(cit =>
            {
                cit.HasIndex("Name", "DepartmentId").IsUnique();
                cit.HasOne(c => c.Department).WithMany(d => d.Cities).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TeamEntity>(dep =>
            {
                dep.HasIndex("Name", "CountryId").IsUnique();
                dep.HasOne(d => d.Country).WithMany(c => c.Teams).OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}