using CRM.Domain.Entities;
using CRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Property(t => t.TicketNumber).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Subject).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Priority).HasDefaultValue(TicketPriority.Medium);
        builder.Property(t => t.Status).HasDefaultValue(TicketStatus.Open);

        builder.HasIndex(t => t.TicketNumber).IsUnique();

        builder.HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Contact)
            .WithMany()
            .HasForeignKey(t => t.ContactId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
