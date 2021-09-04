using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace VisitorSignInSystem.Server.Models
{
    public partial class vsisdataContext : DbContext
    {

        public vsisdataContext(DbContextOptions<vsisdataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AgentStatus> AgentStatuses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Kpi> Kpis { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public virtual DbSet<Visitor> Visitors { get; set; }
        public virtual DbSet<VisitorStatus> VisitorStatuses { get; set; }
        public virtual DbSet<GroupDevice> GroupDevices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=ConnectionStrings:VsisdataAlias", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.28-mysql"), x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.ToTable("agents");

                entity.HasIndex(e => e.StatusName, "fk_agent_status_agent");

                entity.HasKey(e => e.AuthName)
                    .HasName("PRIMARY");
                                
                entity.Property(e => e.AuthName)
                    .HasMaxLength(100)
                    .HasColumnName("auth_name");

                entity.Property(e => e.Categories)
                    .HasColumnType("bit(16)")
                    .HasColumnName("categories");

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("status_name");

                entity.Property(e => e.VisitorId)
                    .HasColumnType("int")
                    .HasColumnName("visitor_id");

                entity.HasOne(d => d.AuthNameNavigation)
                    .WithOne(p => p.Agent)
                    .HasForeignKey<Agent>(d => d.AuthName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_agents");

                entity.HasOne(d => d.StatusNameNavigation)
                    .WithMany(p => p.Agents)
                    .HasForeignKey(d => d.StatusName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_agent_status_agent");
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

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Icon)
                    .HasMaxLength(100)
                    .HasColumnName("icon");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Location)
                    .HasColumnType("tinyint(2)")
                    .HasColumnName("location");

            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("counters");

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("host");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)")
                    .HasColumnName("name");

                entity.Property(e => e.Floor)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnType("varchar(2)")
                    .HasColumnName("floor");

                entity.Property(e => e.IsHandicap)
                    //.HasColumnType("tinyint(1)")
                    .HasColumnName("is_handicap");
                //.HasDefaultValueSql("'1'");

                entity.Property(e => e.IsAvailable)
                    //.HasColumnType("tinyint(1)")
                    .HasColumnName("is_available");
                    //.HasDefaultValueSql("'1'");

                entity.Property(e => e.Icon)
                    .HasColumnType("varchar(100)")
                    .HasColumnName("icon");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

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

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.AuthName)
                    .HasName("PRIMARY");

                entity.ToTable("users");

                entity.HasIndex(e => e.Location, "fk_locations_users");

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
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_locations_users");
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
                    .HasMaxLength(500)
                    .HasColumnName("wat");

                entity.Property(e => e.Wen)
                    .HasColumnType("timestamp")
                    .HasColumnName("wen")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Visitor>(entity =>
            {
                entity.ToTable("visitors");

                entity.HasIndex(e => e.StatusName, "fk_visitor_status_visitor");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsHandicap)
                    .HasColumnName("is_handicap");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.Kiosk)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("kiosk");

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

                entity.Property(e => e.AssignedCounter)
                    .HasMaxLength(25)
                    .HasColumnName("assigned_counter");

                entity.Property(e => e.VisitCategoryId)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("visit_category_id");

                entity.HasOne(d => d.StatusNameNavigation)
                    .WithMany(p => p.Visitors)
                    .HasForeignKey(d => d.StatusName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_visitor_status_visitor");
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

            modelBuilder.Entity<GroupDevice>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("group_devices");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Kind)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("kind");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(45)")
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(100)")
                    .HasColumnName("description");

                entity.Property(e => e.Location)
                    .HasColumnType("int(11)")
                    .HasColumnName("location");

                entity.Property(e => e.CanReceive)
                    .HasColumnType("tinyint(1)")
                    .HasColumnName("can_receive");

                entity.Property(e => e.CanSend)
                    .HasColumnType("tinyint(1)")
                    .HasColumnName("can_send");

                entity.Property(e => e.Enabled)
                    .HasColumnType("tinyint(1)")
                    .HasColumnName("enabled");

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp")
                    .HasColumnName("created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");



            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
