using Microsoft.AspNetCore.Identity;

namespace HomeLoanPortal.Data
{
    public static class SeedRoles
    {
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = new[] { "Admin" };

            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                {
                    await roleManager.CreateAsync(new IdentityRole(r));
                }
            }
        }
    }
}
