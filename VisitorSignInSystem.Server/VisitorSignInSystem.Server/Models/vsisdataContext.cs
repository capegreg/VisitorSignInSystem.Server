using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class vsisdataContext : DbContext
    {
        // cannot have two constructors!
        //public vsisdataContext()
        //{
        //}

        public vsisdataContext(DbContextOptions<vsisdataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AgentMetric> AgentMetrics { get; set; }
        public virtual DbSet<AgentStatus> AgentStatuses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }
        public virtual DbSet<GroupDevice> GroupDevices { get; set; }
        public virtual DbSet<Kpi> Kpis { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public virtual DbSet<Visitor> Visitors { get; set; }
        public virtual DbSet<VisitorMetric> VisitorMetrics { get; set; }
        public virtual DbSet<VisitorStatus> VisitorStatuses { get; set; }
        public virtual DbSet<VsisUser> VsisUsers { get; set; }
        public virtual DbSet<WaitTimeNotify> WaitTimeNotifies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=ConnectionStrings:VsisdataAlias", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.28-mysql"), x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.HasKey(e => e.AuthName)
                    .HasName("PRIMARY");

                entity.ToTable("agents");

                entity.Property(e => e.AuthName)
                    .HasMaxLength(100)
                    .HasColumnName("auth_name");

                entity.Property(e => e.Categories)
                    .HasColumnType("int(11)")
                    .HasColumnName("categories");

                entity.Property(e => e.Counter)
                    .HasMaxLength(100)
                    .HasColumnName("counter");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("status_name");

                entity.Property(e => e.VisitorId)
                    .HasColumnType("int(11)")
                    .HasColumnName("visitor_id");
            });

            modelBuilder.Entity<AgentMetric>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("agent_metrics");

                entity.Property(e => e.AuthName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("auth_name");

                entity.Property(e => e.CallTimeMtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("call_time_mtd");

                entity.Property(e => e.CallTimeToday)
                    .HasColumnType("int(11)")
                    .HasColumnName("call_time_today");

                entity.Property(e => e.CallTimeWtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("call_time_wtd");

                entity.Property(e => e.CallTimeYtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("call_time_ytd");

                entity.Property(e => e.Mtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("mtd");

                entity.Property(e => e.Today)
                    .HasColumnType("int(11)")
                    .HasColumnName("today");

                entity.Property(e => e.Wtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("wtd");

                entity.Property(e => e.Ytd)
                    .HasColumnType("int(11)")
                    .HasColumnName("ytd");
            });

            modelBuilder.Entity<AgentStatus>(entity =>
            {
                entity.HasKey(e => e.StatusName)
                    .HasName("PRIMARY");

                entity.ToTable("agent_status");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(25)
                    .HasColumnName("status_name");

                entity.Property(e => e.StatusDescription)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("status_description");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("bit(16)")
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.Icon)
                    .HasMaxLength(100)
                    .HasColumnName("icon");

                entity.Property(e => e.Location)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("location");
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasKey(e => e.Host)
                    .HasName("PRIMARY");

                entity.ToTable("counters");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Host)
                    .HasMaxLength(100)
                    .HasColumnName("host");

                entity.Property(e => e.Category)
                    .HasColumnType("bit(16)")
                    .HasColumnName("category");

                entity.Property(e => e.CounterNumber)
                    .HasMaxLength(3)
                    .HasColumnName("counter_number");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.DisplayDescription)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("display_description");

                entity.Property(e => e.Floor)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("floor");

                entity.Property(e => e.Icon)
                    .HasMaxLength(100)
                    .HasColumnName("icon");

                entity.Property(e => e.IsAvailable).HasColumnName("is_available");

                entity.Property(e => e.IsHandicap).HasColumnName("is_handicap");

                entity.Property(e => e.Location)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("location");
            });

            modelBuilder.Entity<DeviceType>(entity =>
            {
                entity.HasKey(e => e.Kind)
                    .HasName("PRIMARY");

                entity.ToTable("device_types");

                entity.Property(e => e.Kind)
                    .HasMaxLength(45)
                    .HasColumnName("kind");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Enabled).HasColumnName("enabled");
            });

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<GroupDevice>(entity =>
            {
                entity.ToTable("group_devices");

                entity.HasIndex(e => e.Kind, "fk_devices_device_types_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.CanReceive).HasColumnName("can_receive");

                entity.Property(e => e.CanSend).HasColumnName("can_send");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.Enabled).HasColumnName("enabled");

                entity.Property(e => e.Kind)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("kind");

                entity.Property(e => e.Location)
                    .HasColumnType("int(11)")
                    .HasColumnName("location");

                entity.Property(e => e.Name)
                    .HasMaxLength(45)
                    .HasColumnName("name");

                entity.HasOne(d => d.KindNavigation)
                    .WithMany(p => p.GroupDevices)
                    .HasForeignKey(d => d.Kind)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_devices_device_types");
            });

            modelBuilder.Entity<Kpi>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("kpi");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AgentsByHr)
                    .HasColumnType("int(11)")
                    .HasColumnName("agents_by_hr");

                entity.Property(e => e.AvgWaitTime)
                    .HasColumnType("int(11)")
                    .HasColumnName("avg_wait_time");

                entity.Property(e => e.CategoryiesByHr)
                    .HasColumnType("int(11)")
                    .HasColumnName("categoryies_by_hr");

                entity.Property(e => e.KpiEvent)
                    .HasColumnType("timestamp")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnName("kpi_event")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.MaxWaitTime)
                    .HasColumnType("int(11)")
                    .HasColumnName("max_wait_time");

                entity.Property(e => e.MinWaitTime)
                    .HasColumnType("int(11)")
                    .HasColumnName("min_wait_time");

                entity.Property(e => e.VisitsByHr)
                    .HasColumnType("int(11)")
                    .HasColumnName("visits_by_hr");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("tinyint(2)")
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("address");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.Open)
                    .HasColumnName("open")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PRIMARY");

                entity.ToTable("roles");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(e => e.Who)
                    .HasName("PRIMARY");

                entity.ToTable("user_activity_log");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Who)
                    .HasMaxLength(100)
                    .HasColumnName("who");

                entity.Property(e => e.Wat)
                    .IsRequired()
                    .HasMaxLength(5000)
                    .HasColumnName("wat");

                entity.Property(e => e.Wen)
                    .HasColumnType("timestamp")
                    .HasColumnName("wen")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Visitor>(entity =>
            {
                entity.ToTable("visitors");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasIndex(e => e.StatusName, "fk_visitor_status_visitor");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.AssignedCounter)
                    .HasMaxLength(25)
                    .HasColumnName("assigned_counter");

                entity.Property(e => e.CalledTime)
                    .HasColumnType("timestamp")
                    .HasColumnName("called_time");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsHandicap).HasColumnName("is_handicap");

                entity.Property(e => e.Kiosk)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("kiosk");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.Location)
                    .HasColumnType("int(11)")
                    .HasColumnName("location");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(15)
                    .HasColumnName("mobile");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("status_name")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("'WAITING'");

                entity.Property(e => e.VisitCategoryId)
                    .HasColumnType("bit(16)")
                    .HasColumnName("visit_category_id");

                entity.HasOne(d => d.StatusNameNavigation)
                    .WithMany(p => p.Visitors)
                    .HasForeignKey(d => d.StatusName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_visitor_status_visitor");
            });

            modelBuilder.Entity<VisitorMetric>(entity =>
            {
                entity.ToTable("visitor_metrics");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Agent)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("agent");

                entity.Property(e => e.AssignedCounter)
                    .HasMaxLength(25)
                    .HasColumnName("assigned_counter");

                entity.Property(e => e.CallDuration)
                    .HasColumnType("timestamp")
                    .HasColumnName("call_duration");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsHandicap).HasColumnName("is_handicap");

                entity.Property(e => e.Kiosk)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("kiosk");

                entity.Property(e => e.Location)
                    .HasColumnType("int(11)")
                    .HasColumnName("location");

                entity.Property(e => e.VisitCategoryId)
                    .HasColumnType("bit(16)")
                    .HasColumnName("visit_category_id");
            });

            modelBuilder.Entity<VisitorStatus>(entity =>
            {
                entity.HasKey(e => e.StatusName)
                    .HasName("PRIMARY");

                entity.ToTable("visitor_status");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(25)
                    .HasColumnName("status_name");

                entity.Property(e => e.StatusDescription)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("status_description");
            });

            modelBuilder.Entity<VsisUser>(entity =>
            {
                entity.HasKey(e => e.AuthName)
                    .HasName("PRIMARY");

                entity.ToTable("vsis_users");

                entity.HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasIndex(e => e.Location, "fk_locations_vsis_users");

                entity.Property(e => e.AuthName)
                    .HasMaxLength(100)
                    .HasColumnName("auth_name");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.Location)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("location");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("role")
                    .HasDefaultValueSql("'Agent'");

                entity.HasOne(d => d.LocationNavigation)
                    .WithMany(p => p.VsisUsers)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_locations_vsis_users");
            });

            modelBuilder.Entity<WaitTimeNotify>(entity =>
            {
                entity.HasKey(e => new { e.Mail, e.Category })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("wait_time_notify");

                entity.Property(e => e.Mail)
                    .HasMaxLength(500)
                    .HasColumnName("mail");

                entity.Property(e => e.Category)
                    .HasColumnType("bit(16)")
                    .HasColumnName("category");

                entity.Property(e => e.MaxWaitTimeMinutes)
                    .HasColumnType("tinyint(1) unsigned")
                    .HasColumnName("max_wait_time_minutes");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
