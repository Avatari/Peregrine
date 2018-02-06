using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TaskMgrModels
{
    public partial class TaskMgrContext : DbContext
    {
        public virtual DbSet<KeyValues> KeyValues { get; set; }
        public virtual DbSet<Queues> Queues { get; set; }
        public virtual DbSet<QueueSteps> QueueSteps { get; set; }
        public virtual DbSet<Schedules> Schedules { get; set; }
        public virtual DbSet<Steps> Steps { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<TaskSteps> TaskSteps { get; set; }

        public TaskMgrContext(DbContextOptions<TaskMgrContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KeyValues>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.Property(e => e.Key)
                    .HasMaxLength(20)
                    .ValueGeneratedNever();

                entity.Property(e => e.Dtval)
                    .HasColumnName("DTVal")
                    .HasColumnType("datetime");

                entity.Property(e => e.StrVal).HasMaxLength(200);
            });

            modelBuilder.Entity<Queues>(entity =>
            {
                entity.HasKey(e => e.QueueId);

                entity.Property(e => e.ScheduledStart).HasColumnType("datetime");

                entity.Property(e => e.Completed).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(10);

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.Queues)
                    .HasForeignKey(d => d.ScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Queue_Schedules");
            });

            modelBuilder.Entity<QueueSteps>(entity =>
            {
                entity.HasKey(e => e.QueueStepId);

                entity.Property(e => e.Added).HasColumnType("datetime");

                entity.Property(e => e.ExecutionCompleted).HasColumnType("datetime");

                entity.Property(e => e.ExecutionStarted).HasColumnType("datetime");

                entity.Property(e => e.LastExecutionSuspended).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(10);

                entity.Property(e => e.PostExecutionDecision).HasMaxLength(15);

                entity.HasOne(d => d.Queue)
                    .WithMany(p => p.QueueSteps)
                    .HasForeignKey(d => d.QueueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QueueSteps_Queue");

                entity.HasOne(d => d.Step)
                    .WithMany(p => p.QueueSteps)
                    .HasForeignKey(d => d.StepId)
                    .HasConstraintName("FK_QueueSteps_Steps");
            });

            modelBuilder.Entity<Schedules>(entity =>
            {
                entity.HasKey(e => e.ScheduleId);

                entity.HasIndex(e => e.Name)
                    .HasName("UniqueScheduleNames")
                    .IsUnique();

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.EndDt).HasColumnType("datetime");

                entity.Property(e => e.Freq)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.NextQueue).HasColumnType("datetime");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedules_Tasks");
            });

            modelBuilder.Entity<Steps>(entity =>
            {
                entity.HasKey(e => e.StepId);

                entity.HasIndex(e => e.Name)
                    .HasName("UniqueStepNames")
                    .IsUnique();

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.HasKey(e => e.TaskId);

                entity.HasIndex(e => e.Name)
                    .HasName("UniqueTaskNames")
                    .IsUnique();

                entity.Property(e => e.CompletedEmails).HasMaxLength(1000);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.FailureEmails).HasMaxLength(1000);

                entity.Property(e => e.Modified).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.StartedEmails).HasMaxLength(1000);
            });

            modelBuilder.Entity<TaskSteps>(entity =>
            {
                entity.HasKey(e => e.TaskStepId);

                entity.Property(e => e.PostExecutionDecision).HasMaxLength(15);

                entity.HasOne(d => d.Step)
                    .WithMany(p => p.TaskSteps)
                    .HasForeignKey(d => d.StepId)
                    .HasConstraintName("FK_TaskSteps_Steps");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskSteps)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TaskSteps_Tasks");
            });
        }
    }
}
