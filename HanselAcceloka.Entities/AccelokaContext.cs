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
    public virtual DbSet<user> Users { get; set; } = null!;

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

            entity.HasOne(d => d.user).WithMany(p => p.bookedtickets)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("bookedticket_user_id_fkey");
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

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();
            entity.HasIndex(e => e.username, "users_username_key").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
