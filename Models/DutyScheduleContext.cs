using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DocumentationAndReports.Models;

public partial class DutyScheduleContext : DbContext
{
    public DutyScheduleContext()
    {
    }

    public DutyScheduleContext(DbContextOptions<DutyScheduleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Soldier> Soldiers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("schedule_pkey");

            entity.ToTable("schedule");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.SoldierName)
                .HasMaxLength(255)
                .HasColumnName("soldier_name");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Soldier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("soldiers_pkey");

            entity.ToTable("soldiers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.Holidays).HasColumnName("holidays");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.RelativesEmail)
                .HasMaxLength(255)
                .HasColumnName("relatives_email");
            entity.Property(e => e.SickLeaves).HasColumnName("sick_leaves");
            entity.Property(e => e.State)
                .HasMaxLength(255)
                .HasColumnName("state");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
