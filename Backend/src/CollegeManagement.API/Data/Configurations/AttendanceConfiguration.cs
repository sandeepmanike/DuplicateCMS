using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Data.Configurations
{
    /// <summary>
    /// Configuration for the Attendance entity.
    /// </summary>
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        private const int RemarksMaxLength = 500;

        /// <summary>
        /// Configures the database schema settings for the Attendance entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            #region Table & Keys

            // Table name
            builder.ToTable("Attendances");

            // Primary Key
            builder.HasKey(a => a.AttendanceId);

            #endregion

            #region Properties

            // Required fields and constraints
            builder.Property(a => a.AttendanceDate)
                .IsRequired();

            builder.Property(a => a.StudentId)
                .IsRequired();

            builder.Property(a => a.FacultyId)
                .IsRequired();

            builder.Property(a => a.BoardId)
                .IsRequired();

            builder.Property(a => a.AcademicYearId)
                .IsRequired();

            builder.Property(a => a.AcademicLevelId)
                .IsRequired();

            builder.Property(a => a.GroupId)
                .IsRequired();

            builder.Property(a => a.SectionId)
                .IsRequired();

            builder.Property(a => a.SubjectId)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired();

            builder.Property(a => a.Remarks)
                .HasMaxLength(RemarksMaxLength)
                .IsRequired(false);

            // Default values
            builder.Property(a => a.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(a => a.UpdatedAt)
                .IsRequired(false);

            #endregion

            #region Indexes

            // Unique Index
            builder.HasIndex(a => new { a.StudentId, a.SubjectId, a.AttendanceDate })
                .IsUnique();

            // Query Indexes
            builder.HasIndex(a => a.AttendanceDate);
            builder.HasIndex(a => a.StudentId);
            builder.HasIndex(a => a.FacultyId);
            builder.HasIndex(a => a.SectionId);
            builder.HasIndex(a => a.SubjectId);
            builder.HasIndex(a => a.BoardId);
            builder.HasIndex(a => a.AcademicYearId);
            builder.HasIndex(a => a.GroupId);

            #endregion

            #region Relationships

            // Foreign Keys (DeleteBehavior.Restrict)
            builder.HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Faculty)
                .WithMany()
                .HasForeignKey(a => a.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Board)
                .WithMany()
                .HasForeignKey(a => a.BoardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AcademicYear)
                .WithMany()
                .HasForeignKey(a => a.AcademicYearId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AcademicLevel)
                .WithMany()
                .HasForeignKey(a => a.AcademicLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Group)
                .WithMany()
                .HasForeignKey(a => a.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Section)
                .WithMany()
                .HasForeignKey(a => a.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Subject)
                .WithMany()
                .HasForeignKey(a => a.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
