

using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context
{
    public class ApplicactionContext : DbContext
    {
        public ApplicactionContext(DbContextOptions<ApplicactionContext> options) : base(options)
        {
        }

        public DbSet<Vehiculo> vehiculos { get; set; }
        public DbSet<Reserva> reservas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehiculo>().ToTable(nameof(Vehiculo)); 
            modelBuilder.Entity<Reserva>().ToTable(nameof(Reserva));


            modelBuilder.Entity<Vehiculo>().HasKey(r => r.VehiculoId);

            modelBuilder.Entity<Reserva>().HasKey(r => r.ReservaId);



            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Vehiculo)
                .WithMany( v => v.Reservas)
                .HasForeignKey( x => x.VehiculoId).OnDelete(DeleteBehavior.Cascade);

         
        }
    }
}
