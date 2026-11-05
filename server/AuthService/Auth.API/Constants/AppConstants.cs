namespace Auth.API.Constants
{
    public static class AppConstants
    {
        public static readonly string SYSTEM_ROLE_ID = "ebbad797-e393-11ef-818e-0242ac1b0000";
        public static readonly string ADMIN_ROLE_ID = "ebbad797-e393-11ef-818e-0242ac1b0002";
        public static readonly string USER_ROLE_ID = "ebbad797-e393-11ef-818e-0242ac1b0003";
        public static readonly string STAFF_ROLE_ID = "ebbad797-e393-11ef-818e-0242ac1b0004";
        public static readonly string MANAGER_ROLE_ID = "ebbad797-e393-11ef-818e-0242ac1b0005";

        public static readonly string MEMBERSHIP_REGULARS_ID = "ebbad797-e393-11ef-818e-0242ac1b0010";
        public static readonly string MEMBERSHIP_VIPS_ID = "ebbad797-e393-11ef-818e-0242ac1b0020";
        public static readonly string MEMBERSHIP_STARS_ID = "ebbad797-e393-11ef-818e-0242ac1b0030";

        public static readonly string BRANCH_MAIN_ID = "39ce2709-ac46-40d9-af81-a4ca4c762678";
        public static readonly string BRANCH_SUB1_ID = "39ce2709-ac46-40d9-af81-a4ca4c762679";
        public static readonly string BRANCH_SUB2_ID = "39ce2709-ac46-40d9-af81-a4ca4c762680";

        public static readonly Dictionary<string, string> RoleNames = new()
        {
            { SYSTEM_ROLE_ID, "System" },
            { ADMIN_ROLE_ID, "Admin" },
            { USER_ROLE_ID, "User" },
            { STAFF_ROLE_ID, "Staff" },
            { MANAGER_ROLE_ID, "Manager" }
        };

        public static readonly Dictionary<string, string> MembershipNames = new()
        {
            { MEMBERSHIP_REGULARS_ID, "Regular" },
            { MEMBERSHIP_VIPS_ID, "VIP" },
            { MEMBERSHIP_STARS_ID, "Star" }
        };

        public static string GetRoleName(string roleId)
        {
            return RoleNames.TryGetValue(roleId, out var name) ? name : "Unknown Role";
        }

        public static string GetMembershipName(string membershipId)
        {
            return MembershipNames.TryGetValue(membershipId, out var name) ? name : "Unknown Membership";
        }
    }
}
