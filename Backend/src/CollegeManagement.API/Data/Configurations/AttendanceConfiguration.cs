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
            builder.ToTable("attendances");

            // Primary Key
            builder.HasKey(a => a.AttendanceId);

            #endregion

            #region Properties

            builder.Property(a => a.AttendanceSessionId)
                .IsRequired();

            builder.Property(a => a.StudentId)
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
            builder.HasIndex(a => new { a.StudentId, a.AttendanceSessionId })
                .IsUnique()
                .HasDatabaseName("UX_Attendances_Student_Session");

            // Query Indexes
            builder.HasIndex(a => a.StudentId).HasDatabaseName("IX_Attendances_StudentId");

            #endregion

            #region Relationships

            builder.HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.AttendanceSession)
                .WithMany()
                .HasForeignKey(a => a.AttendanceSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
