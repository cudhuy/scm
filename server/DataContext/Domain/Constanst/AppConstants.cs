namespace Domain.Constanst
{
    public class AppConstants
    {
        // Role ID
        public static readonly Guid SYSTEM_ROLE_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0000");

        public static readonly Guid ADMIN_ROLE_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0002");

        public static readonly Guid USER_ROLE_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0003");

        public static readonly Guid STAFF_ROLE_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0004");

        public static readonly Guid MANAGER_ROLE_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0005");

        // Role ID String (Dùng để phân quyền)
        public const string SYSTEM_ROLE_STRING = "ebbad797-e393-11ef-818e-0242ac1b0000";
        public const string ADMIN_ROLE_STRING = "ebbad797-e393-11ef-818e-0242ac1b0002";
        public const string USER_ROLE_STRING = "ebbad797-e393-11ef-818e-0242ac1b0003";
        public const string STAFF_ROLE_STRING = "ebbad797-e393-11ef-818e-0242ac1b0004";
        public const string MANAGER_ROLE_STRING = "ebbad797-e393-11ef-818e-0242ac1b0005";

        // Membership Rank ID
        public static readonly Guid MEMBERSHIP_REGULARS_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0010");

        public static readonly Guid MEMBERSHIP_VIPS_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0020");

        public static readonly Guid MEMBERSHIP_STARS_GUID = Guid.Parse("ebbad797-e393-11ef-818e-0242ac1b0030");

        // WorkShift ID
        public static readonly Guid MORNING_SHIFT = Guid.Parse("8ef08b6b-dfc0-4dd2-adef-baf78bba477d");

        public static readonly Guid AFTERNOON_SHIFT = Guid.Parse("602ddb77-ba9d-4bd7-85aa-2a34fb125253");

        public static readonly Guid EVENING_SHIFT = Guid.Parse("1f631589-5bc4-432f-9441-eb3bdceb891f");

        public static readonly Guid BRANCH_MAIN_GUID = Guid.Parse("39ce2709-ac46-40d9-af81-a4ca4c762678");

        public static readonly Guid BRANCH_SUB1_GUID = Guid.Parse("39ce2709-ac46-40d9-af81-a4ca4c762679");

        public static readonly Guid BRANCH_SUB2_GUID = Guid.Parse("39ce2709-ac46-40d9-af81-a4ca4c762680");

        // Category ID
        public static readonly Guid CategoryConstants_Coffee = Guid.Parse("39ce2709-ac46-40d9-af81-a4ca4c762680");
        public static readonly Guid CategoryConstants_Tea = Guid.Parse("8e5f3c4d-7b4a-4f0c-80e9-3a6c2b0e846a");
        public static readonly Guid CategoryConstants_Cake = Guid.Parse("b52f15c2-1d49-4317-8cf1-0b91b4b02c32");
        public static readonly Guid CategoryConstants_Juice = Guid.Parse("f927e709-bdf6-4ea3-bf82-df945dced6c5");
        public static readonly Guid CategoryConstants_Smoothie = Guid.Parse("3b8db21d-d601-47b9-92c1-06474ea5d6a1");
        public static readonly Guid CategoryConstants_Yogurt = Guid.Parse("b6b3dd8b-6584-49c4-8228-7d7bba73169e");
        public static readonly Guid CategoryConstants_Blended = Guid.Parse("c4b2d1ea-283e-470d-bb2b-8d45fbf69db1");
        public static readonly Guid CategoryConstants_Snacks = Guid.Parse("f1c9f9db-680e-4d2e-92d9-709cb8a44e3e");
        public static readonly Guid CategoryConstants_Cigarette = Guid.Parse("ab764b1a-1b29-4c65-bbaf-1e94c5801dbb");

        // Reason ID
        public static readonly Guid OutOfStock = Guid.Parse("1f4e5b28-9a8b-4ebc-bd72-5b4f1f8e1f1a");
        public static readonly Guid CustomerRequest = Guid.Parse("2d1a9b38-6c4d-482e-bfa8-ff7f7b9a6c1b");
        public static readonly Guid DeliveryIssue = Guid.Parse("3b5e8f72-a7c9-40f7-91d3-2e9f60b1a5cd");
        public static readonly Guid PaymentFailed = Guid.Parse("4a6e9c84-89b2-4df3-8b5a-1f2c3d9e6b7a");
        public static readonly Guid DuplicateOrder = Guid.Parse("5c8d7e92-2fbc-4c9a-a8b3-3d1a4b9e5f7c");

        public static readonly string DefaultPassword = "Cafe2025@";

        public static readonly Dictionary<Guid, string> RoleNames = new()
        {
            { SYSTEM_ROLE_GUID, "System" },
            { ADMIN_ROLE_GUID, "Admin" },
            { USER_ROLE_GUID, "User" },
            { STAFF_ROLE_GUID, "Staff" },
            { MANAGER_ROLE_GUID, "Manager" }
        };

        public static readonly Dictionary<Guid, string> MembershipNames = new()
        {
            { MEMBERSHIP_REGULARS_GUID, "REGULARS" },
            { MEMBERSHIP_VIPS_GUID, "VIPS" },
            { MEMBERSHIP_STARS_GUID, "STARS" }
        };

        public static string GetRoleName(Guid roleId)
        {
            return RoleNames.TryGetValue(roleId, out var name) ? name : "Unknown Role";
        }

        public static string GetMembershipName(Guid membershipId)
        {
            return MembershipNames.TryGetValue(membershipId, out var name) ? name : "Non-Rank";
        }
    }
}
