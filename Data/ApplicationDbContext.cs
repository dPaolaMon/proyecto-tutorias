using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tutorias.Models;

namespace tutorias.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carrera> Carreras => Set<Carrera>();
        public DbSet<Materia> Materias => Set<Materia>();
        public DbSet<TutorPerfil> TutoresPerfiles => Set<TutorPerfil>();
        public DbSet<SolicitudTutor> SolicitudesTutor => Set<SolicitudTutor>();
        public DbSet<SesionTutoria> SesionesTutoria => Set<SesionTutoria>();
        public DbSet<Asistencia> Asistencias => Set<Asistencia>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación ApplicationUser -> Carrera
            builder.Entity<ApplicationUser>()
                .Property(u => u.Activo)
                .HasDefaultValue(true);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Carrera)
                .WithMany(c => c.Alumnos)
                .HasForeignKey(u => u.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación ApplicationUser -> TutorPerfil (1 a 0..1)
            builder.Entity<TutorPerfil>()
                .HasOne(p => p.User)
                .WithOne(u => u.PerfilTutor)
                .HasForeignKey<TutorPerfil>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Materia -> Carrera
            builder.Entity<Materia>()
                .HasOne(m => m.Carrera)
                .WithMany(c => c.Materias)
                .HasForeignKey(m => m.CarreraId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación SesionTutoria -> Tutor (ApplicationUser)
            builder.Entity<SesionTutoria>()
                .HasOne(s => s.Tutor)
                .WithMany()
                .HasForeignKey(s => s.TutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Asistencia -> Alumno (ApplicationUser)
            builder.Entity<Asistencia>()
                .HasOne(a => a.Alumno)
                .WithMany()
                .HasForeignKey(a => a.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
