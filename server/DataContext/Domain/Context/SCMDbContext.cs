using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Context
{
    public class SCMDbContext : DbContext
    {
        public SCMDbContext(DbContextOptions<SCMDbContext> options) : base(options)
        {

        }

        // =======================
        // KHAI BÁO CÁC BẢNG (DbSet)
        // =======================
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<MembershipRanks> MembershipRanks { get; set; }
        public DbSet<MembershipRanksVouchers> MembershipRanksVouchers { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherUsage> VoucherUsages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotifications> UserNotifications { get; set; }
        public DbSet<StaffInfo> StaffInfos { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<WorkShift> WorkShifts { get; set; }
        public DbSet<EmployeeShift> EmployeeShifts { get; set; }
        public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<InventoryItem> InventoryItem { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<LowStockAlert> LowStockAlerts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CancelReason> CancelReasons { get; set; }
        public DbSet<OrderCancellation> OrderCancellations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BranchTable> BranchTables { get; set; }
        public DbSet<InventoryReceipt> InventoryReceipts { get; set; }
        public DbSet<UserWishList> UserWishLists { get; set; }
        public DbSet<PromotionalBanner> PromotionalBanners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =======================
            // KHAI BÁO CÁC QUAN HỆ
            // =======================

            // Cấu hình quan hệ giữa InventoryItem và LowStockAlert
            modelBuilder.Entity<LowStockAlert>()
                .HasOne(lsa => lsa.InventoryItem)
                .WithMany(ii => ii.LowStockAlerts)
                .HasForeignKey(lsa => lsa.InventoryItemId);

            // Cấu hình quan hệ giữa inventory receipt và inventory item
            modelBuilder.Entity<InventoryReceipt>()
            .HasMany(r => r.InventoryItems)
            .WithOne(i => i.InventoryReceipt)
            .HasForeignKey(i => i.InventoryReceiptId);

            // Cấu hình quan hệ giữa Branch và Table
            modelBuilder.Entity<BranchTable>()
               .HasOne(t => t.Branch)
               .WithMany(b => b.BranchTables)
               .HasForeignKey(t => t.BranchId);

            // Cấu hình quan hệ giữa Order và Table
            modelBuilder.Entity<Order>()
               .HasOne(o => o.Table)
               .WithMany(t => t.Orders)
               .HasForeignKey(o => o.TableId);

            // Cấu hình quan hệ giữa Order và Branch
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Branch)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BranchId);

            // low stock alert
            modelBuilder.Entity<LowStockAlert>()
                .HasOne(l => l.Inventory)
                .WithMany(b => b.LowStockAlerts)
                .HasForeignKey(l => l.InventoryId); // Hoặc SetNull nếu muốn

            modelBuilder.Entity<LowStockAlert>()
                .HasOne(l => l.Ingredient)
                .WithMany(i => i.LowStockAlerts)
                .HasForeignKey(l => l.IngredientId);

            //InventoryReceipt

            modelBuilder.Entity<InventoryReceipt>()
                .HasOne(ir => ir.Inventory)
                .WithMany(b => b.InventoryReceipts)
                .HasForeignKey(ir => ir.InventoryId);

            modelBuilder.Entity<InventoryReceipt>()
                .HasOne(ir => ir.Ingredient)
                .WithMany(i => i.InventoryReceipts)
                .HasForeignKey(ir => ir.IngredientId);


            // 1-1: Một chi nhánh chỉ có một kho duy nhất
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Inventory)
                .WithOne(i => i.Branch)
                .HasForeignKey<Inventory>(i => i.BranchId);

            // 1-N: Một kho có nhiều InventoryItems
            modelBuilder.Entity<InventoryItem>()
                .HasOne(ii => ii.Inventory)
                .WithMany(i => i.InventoryItems)
                .HasForeignKey(ii => ii.InventoryId);

            // 1-N: Một nguyên liệu có thể xuất hiện trong nhiều kho
            modelBuilder.Entity<InventoryItem>()
                .HasOne(ii => ii.Ingredient)
                .WithMany(i => i.InventoryItems)
                .HasForeignKey(ii => ii.IngredientId);



            modelBuilder.Entity<UserNotifications>()
       .HasKey(un => new { un.Id });

            modelBuilder.Entity<UserNotifications>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId);

            modelBuilder.Entity<UserNotifications>()
                .HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId);

            // 1-N: Một Category có thể chứa nhiều Products, một Product thuộc một Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // N-N: Một Product có thể chứa nhiều Ingredients, một Ingredient có thể thuộc nhiều Products
            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductIngredient>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(i => i.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-N: Một Order có thể có nhiều OrderDetails, một OrderDetail thuộc một Order
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-N: Một Product có thể thuộc nhiều OrderDetails, một OrderDetail thuộc một Product
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-1: Một Order có thể có một OrderCancellation, một OrderCancellation thuộc một Order
            modelBuilder.Entity<OrderCancellation>()
                .HasOne(oc => oc.Order)
                .WithMany()
                .HasForeignKey(oc => oc.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-N: Một CancelReason có thể thuộc nhiều OrderCancellations, một OrderCancellation có thể có một CancelReason
            modelBuilder.Entity<OrderCancellation>()
                .HasOne(oc => oc.CancelReason)
                .WithMany()
                .HasForeignKey(oc => oc.CancelReasonId)
                .OnDelete(DeleteBehavior.SetNull);

            // 1-N: Một Order có thể có nhiều Payments, một Payment thuộc một Order
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            // =======================
            // QUAN HỆ USER - NOTIFICATION (N-N)
            // =======================

            modelBuilder.Entity<UserNotifications>()
                .HasKey(un => new { un.UserId, un.NotificationId });

            modelBuilder.Entity<UserNotifications>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId);

            modelBuilder.Entity<UserNotifications>()
                .HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId);

            // Cấu hình quan hệ giữa User và Feedbacks
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId);

            // Thiết lập quan hệ giữa Order và Feedback (1-1)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Feedback)
                .WithOne(f => f.Order)
                .HasForeignKey<Feedback>(f => f.OrderId);

            // Thiết lập quan hệ giữa Feedback và Branch (N-1)
            modelBuilder.Entity<Feedback>()
               .HasOne(f => f.Branch)
               .WithMany(b => b.Feedbacks)
               .HasForeignKey(f => f.BranchId);

            // =======================
            // QUAN HỆ VỚI BẢNG USER
            // =======================

            // 1-N: Một User có một Role, một Role có nhiều User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // 1-N: Một User có một MembershipRank, một MembershipRank có nhiều User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Ranks)
                .WithMany(mr => mr.Users)
                .HasForeignKey(u => u.RankId);

            // 1-1: Một User có một StaffInfo, một StaffInfo chỉ thuộc về một User
            modelBuilder.Entity<User>()
                .HasOne(u => u.StaffInfo)
                .WithOne(si => si.User)
                .HasForeignKey<StaffInfo>(si => si.UserId);


            // =======================
            // QUAN HỆ LIÊN QUAN ĐẾN MEMBERSHIPRANKS & VOUCHERS
            // =======================

            // N-N: Một MembershipRank có nhiều Voucher, một Voucher có thể thuộc nhiều MembershipRank
            modelBuilder.Entity<MembershipRanksVouchers>()
                .HasOne(mrv => mrv.MembershipRank)
                .WithMany(mr => mr.MembershipRanksVouchers)
                .HasForeignKey(mrv => mrv.MembershipRankId);

            modelBuilder.Entity<MembershipRanksVouchers>()
                .HasOne(mrv => mrv.Voucher)
                .WithMany(v => v.MembershipRanksVouchers)
                .HasForeignKey(mrv => mrv.VoucherId);

            // 1-N: Một User có thể sử dụng nhiều Voucher, một Voucher có thể được sử dụng bởi nhiều User
            modelBuilder.Entity<VoucherUsage>()
                .HasOne(vu => vu.User)
                .WithMany()
                .HasForeignKey(vu => vu.UserId);

            modelBuilder.Entity<VoucherUsage>()
                .HasOne(vu => vu.Voucher)
                .WithMany(v => v.VoucherUsages)
                .HasForeignKey(vu => vu.VoucherId);

            // =======================
            // QUAN HỆ VỚI STAFFINFO & BRANCH
            // =======================

            // 1-N: Một Branch có nhiều StaffInfo, một StaffInfo thuộc về một Branch
            modelBuilder.Entity<StaffInfo>()
                .HasOne(si => si.Branch)
                .WithMany(b => b.Staffs)
                .HasForeignKey(si => si.BranchId);

            // =======================
            // QUAN HỆ VỚI BẢNG SALARY
            // =======================

            // 1-N: Một User có thể có nhiều Salary, nhưng mỗi Salary chỉ thuộc về một User
            modelBuilder.Entity<Salary>()
                .HasOne(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.EmployeeId);

            // =======================
            // QUAN HỆ VỚI WORKSHIFT & EMPLOYEE SHIFT
            // =======================

            // 1-N: Một WorkShift có nhiều EmployeeShift, nhưng một EmployeeShift chỉ thuộc về một WorkShift
            modelBuilder.Entity<EmployeeShift>()
                .HasOne(es => es.WorkShift)
                .WithMany()
                .HasForeignKey(es => es.WorkShiftId);

            // 1-N: Một User có thể có nhiều EmployeeShift, nhưng mỗi EmployeeShift chỉ thuộc về một User
            modelBuilder.Entity<EmployeeShift>()
                .HasOne(es => es.Employee)
                .WithMany()
                .HasForeignKey(es => es.EmployeeId);

            // =======================
            // QUAN HỆ VỚI EMPLOYEE ATTENDANCE
            // =======================

            // 1-N: Một EmployeeShift có nhiều EmployeeAttendance, nhưng mỗi EmployeeAttendance chỉ thuộc về một EmployeeShift
            modelBuilder.Entity<EmployeeAttendance>()
                .HasOne(ea => ea.EmployeeShift)
                .WithMany()
                .HasForeignKey(ea => ea.EmployeeShiftId);

            // 1-N: Một WorkShift có nhiều EmployeeAttendance, nhưng mỗi EmployeeAttendance chỉ thuộc về một WorkShift
            modelBuilder.Entity<EmployeeAttendance>()
                .HasOne(ea => ea.WorkShift)
                .WithMany()
                .HasForeignKey(ea => ea.WorkShiftId);
        }
    }
}
