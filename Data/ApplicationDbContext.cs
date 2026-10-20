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
        public DbSet<MateriaEquivalencia> MateriasEquivalencias => Set<MateriaEquivalencia>();
        public DbSet<TutorPerfil> TutoresPerfiles => Set<TutorPerfil>();
        public DbSet<TutorMateria> TutoresMaterias => Set<TutorMateria>();
        public DbSet<TutorHorario> TutoresHorarios => Set<TutorHorario>();
        public DbSet<SolicitudTutor> SolicitudesTutor => Set<SolicitudTutor>();
        public DbSet<SesionTutoria> SesionesTutoria => Set<SesionTutoria>();
        public DbSet<Asistencia> Asistencias => Set<Asistencia>();
        public DbSet<SesionQrToken> SesionesQrTokens => Set<SesionQrToken>();
        public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Materia>()
                .HasIndex(m => m.CodigoEquivalencia);

            builder.Entity<MateriaEquivalencia>()
                .HasOne(e => e.MateriaOrigen)
                .WithMany()
                .HasForeignKey(e => e.MateriaOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MateriaEquivalencia>()
                .HasOne(e => e.MateriaEquivalente)
                .WithMany()
                .HasForeignKey(e => e.MateriaEquivalenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación ApplicationUser -> Carrera
            builder.Entity<ApplicationUser>()
                .Property(u => u.Activo)
                .HasDefaultValue(true);

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Boleta)
                .IsUnique();

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
                .HasIndex(a => new { a.SesionId, a.AlumnoId })
                .IsUnique();

            builder.Entity<Asistencia>()
                .HasOne(a => a.Alumno)
                .WithMany()
                .HasForeignKey(a => a.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación TutorMateria (TutorPerfil N:M Materia)
            builder.Entity<TutorMateria>()
                .HasIndex(tm => new { tm.TutorPerfilId, tm.MateriaId })
                .IsUnique();

            builder.Entity<TutorMateria>()
                .HasOne(tm => tm.TutorPerfil)
                .WithMany(tp => tp.MateriasImpartidas)
                .HasForeignKey(tm => tm.TutorPerfilId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TutorMateria>()
                .HasOne(tm => tm.Materia)
                .WithMany()
                .HasForeignKey(tm => tm.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación TutorHorario
            builder.Entity<TutorHorario>()
                .HasOne(th => th.TutorPerfil)
                .WithMany(tp => tp.Horarios)
                .HasForeignKey(th => th.TutorPerfilId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TutorHorario>()
                .HasOne(th => th.Materia)
                .WithMany()
                .HasForeignKey(th => th.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación SesionQrToken
            builder.Entity<SesionQrToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            builder.Entity<SesionQrToken>()
                .HasOne(t => t.Sesion)
                .WithMany()
                .HasForeignKey(t => t.SesionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Notificacion
            builder.Entity<Notificacion>()
                .HasIndex(n => new { n.UserId, n.Leida, n.FechaEnvio });

            builder.Entity<Notificacion>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notificacion>()
                .HasOne(n => n.Emisor)
                .WithMany()
                .HasForeignKey(n => n.EmisorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
