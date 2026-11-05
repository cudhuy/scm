//using Auth.API.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace Auth.API.Data
//{
//    public partial class scm_dbContext : DbContext
//    {
//        public scm_dbContext(DbContextOptions<scm_dbContext> options)
//            : base(options)
//        {
//        }

//        public virtual DbSet<User> Users { get; set; } = null!;
//        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
//        public virtual DbSet<Token> Tokens { get; set; } = null!;
//        public DbSet<Branch> Branches { get; set; }
//        public DbSet<StaffInfo> StaffInfos { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {

//            // =======================
//            // QUAN HỆ VỚI STAFFINFO & BRANCH
//            // =======================

//            // 1-N: Một Branch có nhiều StaffInfo, một StaffInfo thuộc về một Branch
//            modelBuilder.Entity<StaffInfo>()
//                .HasOne(si => si.Branch)
//                .WithMany(b => b.Staffs)
//                .HasForeignKey(si => si.BranchId);

//            // -----------------------------
//            // Cấu hình cho entity User
//            // -----------------------------
//            modelBuilder.Entity<User>(entity =>
//            {
//                entity.ToTable("user");

//                // Mối quan hệ với Userrole
//                entity.HasOne(e => e.Role)
//                      .WithMany(r => r.Users)
//                      .HasForeignKey(e => e.RoleId)
//                      .HasConstraintName("FK_User_Userrole");

//                // Mối quan hệ với Token
//                entity.HasMany(e => e.Tokens)
//                      .WithOne(t => t.User)
//                      .HasForeignKey(t => t.UserId)
//                      .HasConstraintName("FK_Token_User");
//            });

//            // 1-1: Một User có một StaffInfo, một StaffInfo chỉ thuộc về một User
//            modelBuilder.Entity<User>()
//                .HasOne(u => u.StaffInfo)
//                .WithOne(si => si.User)
//                .HasForeignKey<StaffInfo>(si => si.UserId);

//            // -----------------------------
//            // Cấu hình cho entity Userrole
//            // -----------------------------
//            modelBuilder.Entity<UserRole>(entity =>
//            {
//                entity.ToTable("userrole");

//                // Mối quan hệ với User
//                entity.HasMany(r => r.Users)
//                        .WithOne(u => u.Role)
//                      .HasForeignKey(u => u.RoleId)
//                      .HasConstraintName("FK_User_Userrole");
//            });

//            // -----------------------------
//            // Cấu hình cho entity Token
//            // -----------------------------
//            modelBuilder.Entity<Token>(entity =>
//            {
//                entity.ToTable("token");

//                // Mối quan hệ với User
//                entity.Property(e => e.UserId)
//                      .HasColumnName("UserID");
//                entity.HasOne(e => e.User)
//                      .WithMany(u => u.Tokens)
//                      .HasForeignKey(e => e.UserId)
//                      .HasConstraintName("FK_Token_User");
//            });
//        }
//    }
//}