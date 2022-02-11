using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

/// <summary>
/// This is scaffolded database first ef project. 
/// ****************************************************************
/// Refer to ef-commands.md
/// Backup this project before regenerating new scaffolded context.
/// It is Ok to edit this file, but be careful on migrations
/// before doing a dotnet ef database update.
/// ****************************************************************
/// Use to get MySQL info
/// SELECT VERSION();
/// SHOW CHARACTER SET;
/// </summary>

namespace VisitorSignInSystem.Server.Models
{
    public partial class vsisdataContext : DbContext
    {
        // Remove extra constructor created by scaffolding
        // Cannot have two constructors!
        //public vsisdataContext() {}

        public vsisdataContext(DbContextOptions<vsisdataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AgentMetric> AgentMetrics { get; set; }
        public virtual DbSet<AgentStatus> AgentStatuses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }
        public virtual DbSet<GroupDevice> GroupDevices { get; set; }
        public virtual DbSet<Kpi> Kpis { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public virtual DbSet<Visitor> Visitors { get; set; }
        public virtual DbSet<VisitorMetric> VisitorMetrics { get; set; }
        public virtual DbSet<VisitorStatus> VisitorStatuses { get; set; }
        public virtual DbSet<VisitorTransferLog> VisitorTransfersLog { get; set; }
        public virtual DbSet<VsisUser> VsisUsers { get; set; }
        public virtual DbSet<WaitTimeNotify> WaitTimeNotifies { get; set; }
        public virtual DbSet<IconInventory> IconInventories { get; set; }
        public virtual DbSet<CategoryMetric> CategoryMetrics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Pluto MySQL version = 5.7.35-log
                optionsBuilder.UseMySql("name=ConnectionStrings:VsisdataAlias",
                    Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.35-mysql"), x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // utf8mb4 can store 4 byte characters
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.HasKey(e => e.AuthName)
                    .HasName("PRIMARY");

                entity.ToTable("agents");

                entity.HasIndex(e => e.StatusName, "fk_agents_agent_status_idx");

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

                entity.HasOne(d => d.StatusNameNavigation)
                    .WithMany(p => p.Agents)
                    .HasForeignKey(d => d.StatusName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_agents_agent_status");
            });

            modelBuilder.Entity<AgentMetric>(entity =>
            {
                entity.HasKey(e => e.AuthName)
                    .HasName("PRIMARY");

                entity.ToTable("agent_metrics");

                entity.Property(e => e.AuthName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("auth_name");

                entity.Property(e => e.VisitorsToday)
                    .HasColumnType("int(11)")
                    .HasColumnName("visitors_today");

                entity.Property(e => e.VisitorsWtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("visitors_wtd");

                entity.Property(e => e.VisitorsMtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("visitors_mtd");

                entity.Property(e => e.VisitorsYtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("visitors_ytd");

                entity.Property(e => e.CallTimeToday)
                    .HasColumnType("double")
                    .HasColumnName("calltime_today");

                entity.Property(e => e.CallTimeWtd)
                    .HasColumnType("double")
                    .HasColumnName("calltime_wtd");

                entity.Property(e => e.CallTimeMtd)
                    .HasColumnType("double")
                    .HasColumnName("calltime_mtd");

                entity.Property(e => e.CallTimeYtd)
                    .HasColumnType("double")
                    .HasColumnName("calltime_ytd");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            });

            modelBuilder.Entity<AgentStatus>(entity =>
            {
                entity.HasKey(e => e.StatusName)
                    .HasName("PRIMARY");

                entity.ToTable("agent_status");

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

                entity.Property(e => e.DepartmentId)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("department_id");

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

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");

                entity.Property(e => e.Id)
                    .HasColumnType("tinyint(2)")
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("department_name");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("symbol");

                entity.Property(e => e.SymbolType)
                    .IsRequired()
                    .HasMaxLength(45)
                    .HasColumnName("symbol_type");

                entity.Property(e => e.OrderBy)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("orderby");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

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
                    .HasColumnType("timestamp")
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
                    .HasColumnType("timestamp")
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
                    .HasConstraintName("group_devices_ibfk_1");
            });

            modelBuilder.Entity<Kpi>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("kpi");

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

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("transfers");

                entity.HasIndex(e => e.Department, "fk_transfers_departments_idx");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Department)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("department");

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

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Transfers)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("fk_transfers_departments");
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(e => e.Who)
                    .HasName("PRIMARY");

                entity.ToTable("user_activity_log");

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
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("visitors");

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

                entity.Property(e => e.IsTransfer).HasColumnName("is_transfer");

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

                entity.Property(e => e.VisitDepartmentId)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("visit_department_id");

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
                    .HasConstraintName("visitors_ibfk_1");
            });

            modelBuilder.Entity<VisitorMetric>(entity =>
            {
                entity.ToTable("visitor_metrics");

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

                entity.Property(e => e.StatusName)
                    .HasMaxLength(25)
                    .HasColumnName("status_name");

                entity.Property(e => e.StatusDescription)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("status_description");
            });

            modelBuilder.Entity<VisitorTransferLog>(entity =>
            {
                // entity.HasNoKey();

                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("visitor_transfers_log");

                entity.HasIndex(e => e.Department, "fk_visitor_transfers_departments");

                entity.Property(e => e.CalledTime)
                    .HasColumnType("timestamp")
                    .HasColumnName("called_time");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Department)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("department");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.VisitorId)
                    .HasColumnType("int(11)")
                    .HasColumnName("visitor_id");

                entity.Property(e => e.Location)
                    .HasColumnType("int(11)")
                    .HasColumnName("location");

                entity.Property(e => e.VisitCategoryId)
                    .HasColumnType("bit(16)")
                    .HasColumnName("visit_category_id");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_visitor_transfers_departments");
            });

            modelBuilder.Entity<VsisUser>(entity =>
            {
                entity.HasKey(e => e.AuthName)
                    .HasName("PRIMARY");

                entity.ToTable("vsis_users");

                entity.HasIndex(e => e.Location, "fk_locations_vsis_users");

                entity.HasIndex(e => e.Department, "fk_vsis_users_departments_idx");

                entity.Property(e => e.AuthName)
                    .HasMaxLength(100)
                    .HasColumnName("auth_name");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.AgentAppVersion)
                    .HasMaxLength(45)
                    .HasColumnName("agent_app_version");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Department)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("department");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(35)
                    .HasColumnName("last_name");

                entity.Property(e => e.Location)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("location");

                entity.Property(e => e.ManagerAppVersion)
                    .HasMaxLength(45)
                    .HasColumnName("manager_app_version");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("role")
                    .HasDefaultValueSql("'Agent'");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.VsisUsers)
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_vsis_users_departments");

                entity.HasOne(d => d.LocationNavigation)
                    .WithMany(p => p.VsisUsers)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vsis_users_ibfk_1");
            });

            modelBuilder.Entity<WaitTimeNotify>(entity =>
            {
                //entity.HasKey(e => new { e.Mail, e.Category })
                //    .HasName("PRIMARY")
                //    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasKey(e => new { e.Category })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("wait_time_notify");

                entity.HasIndex(e => e.Category, "fk_categories_wait_time_notify_idx");

                entity.Property(e => e.Mail)
                    .HasMaxLength(500)
                    .HasColumnName("mail");

                entity.Property(e => e.Category)
                    .HasColumnType("bit(16)")
                    .HasColumnName("category");

                entity.Property(e => e.MaxWaitTimeMinutes)
                    .HasColumnType("tinyint(1) unsigned")
                    .HasColumnName("max_wait_time_minutes");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.WaitTimeNotifies)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("wait_time_notify_ibfk_1");
            });

            modelBuilder.Entity<IconInventory>(entity =>
            {
                // entity.HasNoKey();

                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("icons");

                entity.Property(e => e.Icon)
                    .HasColumnType("varchar(100)")
                    .HasColumnName("icon");

                entity.Property(e => e.ControlType)
                    .HasColumnType("int")
                    .HasColumnName("control_type");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("description");

            });

            modelBuilder.Entity<CategoryMetric>(entity =>
            {
                entity.HasKey(e => e.Category)
                    .HasName("PRIMARY");

                entity.ToTable("category_metrics");

                entity.Property(e => e.Category)
                    .HasColumnType("bit(16)")
                    .ValueGeneratedNever()
                    .HasColumnName("category");

                entity.Property(e => e.CategoryToday)
                    .HasColumnType("int(11)")
                    .HasColumnName("category_today");

                entity.Property(e => e.CategoryWtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("category_wtd");

                entity.Property(e => e.CategoryMtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("category_mtd");

                entity.Property(e => e.CategoryYtd)
                    .HasColumnType("int(11)")
                    .HasColumnName("category_ytd");

                entity.Property(e => e.CallTimeToday)
                    .HasColumnType("double")
                    .HasColumnName("calltime_today");

                entity.Property(e => e.CallTimeWtd)
                    .HasColumnType("double")
                    .HasColumnName("calltime_wtd");

                entity.Property(e => e.CallTimeMtd)
                    .HasColumnType("double")
                    .HasColumnName("calltime_mtd");

                entity.Property(e => e.CallTimeYtd)
                    .HasColumnType("double")
                    .HasColumnName("calltime_ytd");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
