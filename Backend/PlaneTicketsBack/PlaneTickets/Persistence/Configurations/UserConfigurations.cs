using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaneTickets.Models;

namespace PlaneTickets.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<UserDb>
{
    public void Configure(EntityTypeBuilder<UserDb> builder)
    {
        builder.HasKey(f => f.Username);
        builder.Property(f => f.Username).ValueGeneratedNever();
    }
}
