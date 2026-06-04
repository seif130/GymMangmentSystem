using GymManagmet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagmet.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name).HasColumnType("varchar").HasMaxLength(50);

            builder.Property(x => x.Description).HasColumnType("varchar").HasMaxLength(200);

            builder.Property(x => x.Price).HasPrecision(10, 2);

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.ToTable(Tb =>
            {

                Tb.HasCheckConstraint("DurationDaysCheck", "DurationDays Between 1 AND 365");
                   
            });

        }
    }
}
