using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PRISM.Models
{
    public partial class devprismdbContext : DbContext
    {
        public devprismdbContext()
        {
        }

        public devprismdbContext(DbContextOptions<devprismdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Absance> Absances { get; set; } = null!;
        public virtual DbSet<AppUser> AppUsers { get; set; } = null!;
        public virtual DbSet<ChangeLog> ChangeLogs { get; set; } = null!;
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<CustomValue> CustomValues { get; set; } = null!;
        public virtual DbSet<Distribution> Distributions { get; set; } = null!;
        public virtual DbSet<DriverQuestion> DriverQuestions { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<FilterHistory> FilterHistories { get; set; } = null!;
        public virtual DbSet<IndividualShift> IndividualShifts { get; set; } = null!;
        public virtual DbSet<LeaveType> LeaveTypes { get; set; } = null!;
        public virtual DbSet<Lnedetail> Lnedetails { get; set; } = null!;
        public virtual DbSet<LogTypeEntry> LogTypeEntries { get; set; } = null!;
        public virtual DbSet<LookupEntity> LookupEntities { get; set; } = null!;
        public virtual DbSet<Machine> Machines { get; set; } = null!;
        public virtual DbSet<MachineShiftVstp> MachineShiftVstps { get; set; } = null!;
        public virtual DbSet<MileStoneEntry> MileStoneEntries { get; set; } = null!;
        public virtual DbSet<Module> Modules { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleModule> RoleModules { get; set; } = null!;
        public virtual DbSet<Route> Routes { get; set; } = null!;
        public virtual DbSet<ShiftContact> ShiftContacts { get; set; } = null!;
        public virtual DbSet<ShiftDetailEmployee> ShiftDetailEmployees { get; set; } = null!;
        public virtual DbSet<ShiftFilter> ShiftFilters { get; set; } = null!;
        public virtual DbSet<ShiftNumber> ShiftNumbers { get; set; } = null!;
        public virtual DbSet<ShiftRosterDetail> ShiftRosterDetails { get; set; } = null!;
        public virtual DbSet<ShiftTemplate> ShiftTemplates { get; set; } = null!;
        public virtual DbSet<UserLog> UserLogs { get; set; } = null!;
        public virtual DbSet<Week> Weeks { get; set; } = null!;
        public virtual DbSet<WeekArrangement> WeekArrangements { get; set; } = null!;
        public virtual DbSet<WeeklyComment> WeeklyComments { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=devprismnew.database.windows.net;user=devprismnew;password='devprism?123';database=devprismdb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Absance>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Reason).HasMaxLength(200);

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.FullName).HasMaxLength(80);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasMaxLength(250);
            });

            modelBuilder.Entity<ChangeLog>(entity =>
            {
                entity.ToTable("ChangeLog");

                entity.Property(e => e.ChangeDate).HasColumnType("datetime");

                entity.Property(e => e.ChangePeriod).HasMaxLength(50);

                entity.Property(e => e.ChangeType).HasMaxLength(50);

                entity.Property(e => e.ChangedBy).HasMaxLength(500);

                entity.Property(e => e.ContactName).HasMaxLength(50);

                entity.Property(e => e.FurtherAction).HasMaxLength(50);

                entity.Property(e => e.InstigatedBy).HasMaxLength(50);

                entity.Property(e => e.LogShiftDate).HasColumnType("datetime");

                entity.Property(e => e.MachineNum).HasMaxLength(250);

                entity.Property(e => e.MoreInformation).HasMaxLength(500);

                entity.Property(e => e.Ptonumber)
                    .HasMaxLength(50)
                    .HasColumnName("PTONumber");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Company).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.FaxNumber).HasMaxLength(50);

                entity.Property(e => e.JobTitle).HasMaxLength(50);

                entity.Property(e => e.MobileNumber).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CustomValue>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Distribution>(entity =>
            {
                entity.Property(e => e.ActiveStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress).HasMaxLength(200);
            });

            modelBuilder.Entity<DriverQuestion>(entity =>
            {
                entity.Property(e => e.Comments).HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsSetisfied)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedId).HasMaxLength(200);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.ContactNumber).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Department).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.EmployeeId).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Gang).HasMaxLength(50);

                entity.Property(e => e.Initials).HasMaxLength(50);

                entity.Property(e => e.JobTitle).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.Manager).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ReportsTo).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<FilterHistory>(entity =>
            {
                entity.ToTable("FilterHistory");

                entity.Property(e => e.Act).HasColumnName("ACT");

                entity.Property(e => e.FromWeek).HasMaxLength(50);

                entity.Property(e => e.MachineNumber).HasMaxLength(50);

                entity.Property(e => e.MachineStatus).HasMaxLength(50);

                entity.Property(e => e.Otm).HasColumnName("OTM");

                entity.Property(e => e.Otpm).HasColumnName("OTPM");

                entity.Property(e => e.Ott).HasColumnName("OTT");

                entity.Property(e => e.ShiftStatus).HasMaxLength(50);

                entity.Property(e => e.Template).HasMaxLength(50);

                entity.Property(e => e.ToWeek).HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(200);
            });

            modelBuilder.Entity<IndividualShift>(entity =>
            {
                entity.Property(e => e.ActualFinish).HasColumnType("datetime");

                entity.Property(e => e.ActualStart).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.ModifiedBy).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PlannedFinish).HasColumnType("datetime");

                entity.Property(e => e.PlannedStart).HasColumnType("datetime");

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.Property(e => e.ColorCode).HasMaxLength(10);

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Lnedetail>(entity =>
            {
                entity.ToTable("LNEDetails");

                entity.Property(e => e.CancellationDate).HasMaxLength(255);

                entity.Property(e => e.CancelledBy).HasMaxLength(255);

                entity.Property(e => e.CnupledMachines)
                    .HasMaxLength(255)
                    .HasColumnName("CNupledMachines");

                entity.Property(e => e.ConsistSleeperType).HasMaxLength(255);

                entity.Property(e => e.Contractor).HasMaxLength(255);

                entity.Property(e => e.Customer).HasMaxLength(255);

                entity.Property(e => e.EnteredBy).HasMaxLength(255);

                entity.Property(e => e.FinishDate).HasColumnType("datetime");

                entity.Property(e => e.FinishTime).HasMaxLength(255);

                entity.Property(e => e.JobNumber).HasMaxLength(255);

                entity.Property(e => e.LineToArriveOn).HasMaxLength(255);

                entity.Property(e => e.MachineNum).HasMaxLength(255);

                entity.Property(e => e.MachineSupply).HasMaxLength(255);

                entity.Property(e => e.MachineType).HasMaxLength(255);

                entity.Property(e => e.Mileage).HasMaxLength(255);

                entity.Property(e => e.OrderDate).HasMaxLength(255);

                entity.Property(e => e.OrderRevision).HasMaxLength(255);

                entity.Property(e => e.OrderStatus).HasMaxLength(255);

                entity.Property(e => e.PartOfBlockade).HasMaxLength(255);

                entity.Property(e => e.PossessionDetails).HasMaxLength(255);

                entity.Property(e => e.PpreAct)
                    .HasMaxLength(50)
                    .HasColumnName("PpreACT");

                entity.Property(e => e.PpreAssessor)
                    .HasMaxLength(255)
                    .HasColumnName("PPreAssessor");

                entity.Property(e => e.PpreCompany)
                    .HasMaxLength(250)
                    .HasColumnName("PPreCompany");

                entity.Property(e => e.PpreDay)
                    .HasMaxLength(50)
                    .HasColumnName("PPreDay");

                entity.Property(e => e.PpreDra)
                    .HasMaxLength(50)
                    .HasColumnName("PpreDRA");

                entity.Property(e => e.PpreDriver)
                    .HasMaxLength(255)
                    .HasColumnName("PPreDriver");

                entity.Property(e => e.PpreEmptyComment)
                    .HasMaxLength(500)
                    .HasColumnName("PPreEmptyComment");

                entity.Property(e => e.PpreFailureComments)
                    .HasMaxLength(500)
                    .HasColumnName("PPreFailureComments");

                entity.Property(e => e.PpreHeadCode)
                    .HasMaxLength(50)
                    .HasColumnName("PPreHeadCode");

                entity.Property(e => e.PpreInternalComments)
                    .HasMaxLength(500)
                    .HasColumnName("PPreInternalComments");

                entity.Property(e => e.PpreLogNumber)
                    .HasMaxLength(50)
                    .HasColumnName("PPreLogNumber");

                entity.Property(e => e.PpreOperator)
                    .HasMaxLength(255)
                    .HasColumnName("PPreOperator");

                entity.Property(e => e.PprePathTime)
                    .HasMaxLength(50)
                    .HasColumnName("PPrePathTime");

                entity.Property(e => e.PprePlannedHours)
                    .HasMaxLength(50)
                    .HasColumnName("PPrePlannedHours");

                entity.Property(e => e.PprePlannedWork)
                    .HasColumnType("ntext")
                    .HasColumnName("PPrePlannedWork");

                entity.Property(e => e.PprePlfield)
                    .HasMaxLength(50)
                    .HasColumnName("PprePLField");

                entity.Property(e => e.PpreShiftDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PPreShiftDate");

                entity.Property(e => e.PpreSummary)
                    .HasMaxLength(500)
                    .HasColumnName("PPreSummary");

                entity.Property(e => e.Ppsreference)
                    .HasMaxLength(255)
                    .HasColumnName("PPSReference");

                entity.Property(e => e.PriorityStatusShift).HasMaxLength(255);

                entity.Property(e => e.Ptonumber)
                    .HasMaxLength(255)
                    .HasColumnName("PTONumber");

                entity.Property(e => e.Remarks).HasColumnType("ntext");

                entity.Property(e => e.Route).HasMaxLength(255);

                entity.Property(e => e.Shift)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("(N'Live')");

                entity.Property(e => e.ShiftTimeDetail).HasColumnType("ntext");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasMaxLength(255);

                entity.Property(e => e.Supplier).HasMaxLength(255);

                entity.Property(e => e.TimeBackToYard).HasMaxLength(255);

                entity.Property(e => e.TimeOutOfYard).HasMaxLength(255);

                entity.Property(e => e.TrainId).HasMaxLength(255);

                entity.Property(e => e.TrainOrderType).HasMaxLength(255);

                entity.Property(e => e.WeekNo).HasMaxLength(255);

                entity.Property(e => e.WonNumber).HasMaxLength(50);

                entity.Property(e => e.WorkCompleted).HasMaxLength(255);

                entity.Property(e => e.WorkDescription).HasMaxLength(255);

                entity.Property(e => e.WorkYardage).HasMaxLength(255);

                entity.Property(e => e.WorksiteDescriptor).HasMaxLength(255);

                entity.Property(e => e.WorksiteDetails).HasMaxLength(255);

                entity.Property(e => e.WorksiteElr)
                    .HasMaxLength(255)
                    .HasColumnName("WorksiteELR");

                entity.Property(e => e.WorksiteNo).HasMaxLength(255);

                entity.Property(e => e.YardIn).HasMaxLength(255);

                entity.Property(e => e.YardOut).HasMaxLength(255);
            });

            modelBuilder.Entity<LogTypeEntry>(entity =>
            {
                entity.ToTable("LogTypeEntry");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.LogName).HasMaxLength(50);

                entity.Property(e => e.LogType)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LookupEntity>(entity =>
            {
                entity.ToTable("LookupEntity");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.LookupType).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.Property(e => e.Area).HasMaxLength(100);

                entity.Property(e => e.Capabilities).HasMaxLength(200);

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.HeadCode).HasMaxLength(50);

                entity.Property(e => e.ManagerName).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Nrn1)
                    .HasMaxLength(50)
                    .HasColumnName("NRN1");

                entity.Property(e => e.Nrn2)
                    .HasMaxLength(50)
                    .HasColumnName("NRN2");

                entity.Property(e => e.Number).HasMaxLength(50);

                entity.Property(e => e.OwnerName).HasMaxLength(50);

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Specification).HasMaxLength(200);

                entity.Property(e => e.Speed).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.Property(e => e.Weight).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<MachineShiftVstp>(entity =>
            {
                entity.ToTable("MachineShiftVSTP");

                entity.Property(e => e.Comments)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.DestLoc).HasMaxLength(200);

                entity.Property(e => e.DestName).HasMaxLength(200);

                entity.Property(e => e.DestStanox)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.DestTime).HasMaxLength(50);

                entity.Property(e => e.HeadCode)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedId).HasMaxLength(200);

                entity.Property(e => e.NumberOfVehicles)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.OriginLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.OriginName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OriginStanox)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.OriginTime).HasMaxLength(50);

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ShiftId).HasColumnName("ShiftID");

                entity.Property(e => e.Vstpcontact)
                    .HasMaxLength(300)
                    .HasColumnName("VSTPContact");
            });

            modelBuilder.Entity<MileStoneEntry>(entity =>
            {
                entity.Property(e => e.Actuall).HasMaxLength(50);

                entity.Property(e => e.Comments).HasMaxLength(200);

                entity.Property(e => e.LogEntry).HasMaxLength(50);

                entity.Property(e => e.MileStoneEntryDetail).HasMaxLength(100);

                entity.Property(e => e.Planned).HasMaxLength(50);

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.ModuleId).HasMaxLength(50);

                entity.Property(e => e.ModuleName).HasMaxLength(50);

                entity.Property(e => e.ModuleUrl).HasMaxLength(200);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<RoleModule>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.ModuleId });
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.Property(e => e.Comments)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ShortCode).HasMaxLength(50);
            });

            modelBuilder.Entity<ShiftContact>(entity =>
            {
                entity.Property(e => e.ContactType).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShiftDetailEmployee>(entity =>
            {
                entity.Property(e => e.ContactNumber).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmployeeType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.JobTitle).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ShiftFilter>(entity =>
            {
                entity.Property(e => e.Filters).HasMaxLength(50);

                entity.Property(e => e.FromWeek).HasMaxLength(50);

                entity.Property(e => e.MachineNumber).HasMaxLength(50);

                entity.Property(e => e.MachineStatus).HasMaxLength(50);

                entity.Property(e => e.ShiftStatus).HasMaxLength(50);

                entity.Property(e => e.Template).HasMaxLength(50);

                entity.Property(e => e.ToWeek).HasMaxLength(50);
            });

            modelBuilder.Entity<ShiftNumber>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EndShiftDateTime).HasColumnType("datetime");

                entity.Property(e => e.StartShiftDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShiftRosterDetail>(entity =>
            {
                entity.ToTable("ShiftRosterDetail");

                entity.Property(e => e.RosterShiftDescription).HasMaxLength(500);
            });

            modelBuilder.Entity<ShiftTemplate>(entity =>
            {
                entity.Property(e => e.Columns).HasMaxLength(500);

                entity.Property(e => e.TemplateName).HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(200);
            });

            modelBuilder.Entity<UserLog>(entity =>
            {
                entity.ToTable("UserLog");

                entity.Property(e => e.ActionType)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.AppName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.RecordStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasMaxLength(300);
            });

            modelBuilder.Entity<Week>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<WeekArrangement>(entity =>
            {
                entity.Property(e => e.ColumnNo1).HasMaxLength(100);

                entity.Property(e => e.ColumnNo2).HasMaxLength(100);

                entity.Property(e => e.ColumnNo3).HasMaxLength(100);

                entity.Property(e => e.ColumnNo4).HasMaxLength(100);

                entity.Property(e => e.ColumnNo5).HasMaxLength(100);

                entity.Property(e => e.ColumnNo6).HasMaxLength(100);

                entity.Property(e => e.ColumnNo7).HasMaxLength(100);

                entity.Property(e => e.ColumnNo8).HasMaxLength(100);

                entity.Property(e => e.UserId).HasMaxLength(200);
            });

            modelBuilder.Entity<WeeklyComment>(entity =>
            {
                entity.Property(e => e.CoursesAndOthers).IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EngineeringSupport).IsUnicode(false);

                entity.Property(e => e.UserId).HasMaxLength(200);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
