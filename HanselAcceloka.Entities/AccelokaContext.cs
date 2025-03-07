using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HanselAcceloka.Entities;

public partial class AccelokaContext : DbContext
{
    public AccelokaContext(DbContextOptions<AccelokaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<bookedticket> BookedTickets { get; set; } = null!;
    public virtual DbSet<bookedticketdetail> BookedTicketDetails { get; set; } = null!;
    public virtual DbSet<category> Categories { get; set; } = null!;
    public virtual DbSet<ticket> Tickets { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5431;Database=exam_acceloka;Username=postgres;Password=Hansel12345");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<bookedticket>(entity =>
        {
            entity.HasKey(e => e.booked_ticket_id).HasName("bookedticket_pkey");

            entity.ToTable("bookedticket");

            entity.Property(e => e.booked_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<bookedticketdetail>(entity =>
        {
            entity.HasKey(e => e.booked_ticket_detail_id).HasName("bookedticketdetail_pkey");

            entity.ToTable("bookedticketdetail");

            entity.Property(e => e.ticket_code).HasMaxLength(10);

            entity.HasOne(d => d.booked_ticket).WithMany(p => p.bookedticketdetails)
                .HasForeignKey(d => d.booked_ticket_id)
                .HasConstraintName("bookedticketdetail_booked_ticket_id_fkey");

            entity.HasOne(d => d.ticket_codeNavigation).WithMany(p => p.bookedticketdetails)
                .HasForeignKey(d => d.ticket_code)
                .HasConstraintName("bookedticketdetail_ticket_code_fkey");
        });

        modelBuilder.Entity<category>(entity =>
        {
            entity.HasKey(e => e.category_id).HasName("category_pkey");

            entity.ToTable("category");

            entity.HasIndex(e => e.category_name, "category_category_name_key").IsUnique();

            entity.Property(e => e.category_name).HasMaxLength(100);
        });

        modelBuilder.Entity<ticket>(entity =>
        {
            entity.HasKey(e => e.ticket_code).HasName("ticket_pkey");

            entity.ToTable("ticket");

            entity.Property(e => e.ticket_code).HasMaxLength(10);
            entity.Property(e => e.event_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ticket_name).HasMaxLength(255);

            entity.HasOne(d => d.category).WithMany(p => p.tickets)
                .HasForeignKey(d => d.category_id)
                .HasConstraintName("ticket_category_id_fkey");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
