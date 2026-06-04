using GymManagmet.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Configurations
{
    internal class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name).HasColumnType("varchar").HasMaxLength(50);

            builder.Property(x => x.Email).HasColumnType("varchar").HasMaxLength(100);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.HasIndex(x => x.Phone).IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailCheck", "Email LIKE '_@_%._%'");
                tb.HasCheckConstraint("PhoneCheck", "Phone LIKE '010%' or Phone LIKE '012%' or Phone LIKE '011%'");
            });

            builder.OwnsOne(x => x.Address, a =>
            {
                a.Property(p => p.Street).HasColumnType("varchar").HasMaxLength(30);
                a.Property(p => p.City).HasColumnType("varchar").HasMaxLength(30);

            });
        }
    }
}
