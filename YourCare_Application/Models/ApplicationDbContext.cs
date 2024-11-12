using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace YourCare_Application.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public ApplicationDbContext()
        {

        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                    .GetConnectionString("DefaultConStr"));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(e =>
            {
                e.ToTable("AspNetUsers")
                .HasDiscriminator<string>("Discriminator")
                .HasValue<ApplicationUser>("ApplicationUser")
                .HasValue<Doctor>("Doctor");
            });

            builder.Entity<Specialization>(e =>
            {
                e.ToTable("Specialization");
                e.HasKey(x => x.Id);
            });


            builder.Entity<DoctorSpecialization>(e =>
            {
                e.ToTable("DoctorSpecialization");
                e.HasKey(e => new { e.DoctorID, e.SpecializationID });

                e.HasOne<Doctor>(x => x.Doctor)
                .WithMany(x => x.DoctorSpecializations)
                .HasForeignKey(x => x.DoctorID)
                .HasConstraintName("FK_DoctorSpecialization_Doctor");


                e.HasOne<Specialization>(x => x.Specialization)
                .WithMany(x => x.DoctorSpecializations)
                .HasForeignKey(x => x.SpecializationID)
                .HasConstraintName("FK_DoctorSpecialization_Specialization");
            });

            builder.Entity<Timetable>(e =>
            {
                e.ToTable("Timetable");
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Doctor)
                .WithMany(x => x.Timetables)
                .HasForeignKey(x => x.DoctorID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Timetable_Doctor");

                e.HasIndex(x => new { x.DoctorID, x.Date, x.StartTime, x.EndTime })
                .IsUnique();

                e.Property(x => x.StartTime)
                .HasColumnType("time");

                e.Property(x => x.EndTime)
                .HasColumnType("time");

                e.Property(x => x.Date)
                .HasColumnType("date");
            });


            builder.Entity<Appointment>(e =>
            {
                e.ToTable("Appointment");
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Doctor).WithMany(x => x.DoctorAppointments)
                .HasForeignKey(x => x.DoctorID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Appointment_Doctor");

                e.HasOne(x => x.PatientProfile).WithMany(x => x.Appointments)
               .HasForeignKey(x => x.PatientProfileID)
                .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("FK_Appointment_Patient");

                e.HasOne(x => x.CreatedByUser).WithMany(x => x.CreatedAppointments)
               .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_Appointment_User");

                e.HasOne(x => x.TimeTable).WithOne(x => x.Appointment)
                .HasForeignKey<Appointment>(x => x.TimetableID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Appointment_Timetable");

                e.Property(x=>x.UpdatedOn)
                .HasColumnType("date");

            });

            builder.Entity<PatientProfile>(e =>
            {

                e.ToTable("PatientProfile");
                e.HasKey(x => x.Id);

                e.HasMany(x => x.Appointments)
                .WithOne(x => x.PatientProfile)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Patient_Appointment");

                e.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.PatientProfiles)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Patient_User");


            });


            base.OnModelCreating(builder);
        }

    }
}
