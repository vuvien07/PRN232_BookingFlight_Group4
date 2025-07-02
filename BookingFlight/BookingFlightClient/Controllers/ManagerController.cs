using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingFlightClient.Controllers
{
    public class ManagerController : Controller
    {
        private void SetUserRole()
        {
            // Get role from JWT token claims
            var roleIdClaim = HttpContext.User?.FindFirst("RoleId")?.Value;
            var roleNameClaim = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
            
            // Debug logging
            Console.WriteLine($"ManagerController SetUserRole - RoleId claim: {roleIdClaim}");
            Console.WriteLine($"ManagerController SetUserRole - Role name claim: {roleNameClaim}");
            
            if (int.TryParse(roleIdClaim, out int roleId))
            {
                ViewBag.UserRole = roleId;
                
                // Map roleId to correct roleName if roleName claim is missing or incorrect
                string correctRoleName = GetRoleNameFromId(roleId);
                ViewBag.RoleName = !string.IsNullOrEmpty(roleNameClaim) ? roleNameClaim : correctRoleName;
                
                Console.WriteLine($"ManagerController SetUserRole - Set ViewBag.UserRole = {roleId}, ViewBag.RoleName = {ViewBag.RoleName}");
                Console.WriteLine($"ManagerController SetUserRole - Original role name claim: {roleNameClaim}, Mapped role name: {correctRoleName}");
            }
            else
            {
                // Default to role 4 (Manager) if not found - corrected based on actual database
                ViewBag.UserRole = 4;
                ViewBag.RoleName = "Manager";
                Console.WriteLine($"ManagerController SetUserRole - Using default: ViewBag.UserRole = 4, ViewBag.RoleName = Manager");
            }
        }
        
        private string GetRoleNameFromId(int roleId)
        {
            // Database mapping: 1=Admin, 2=Supporter, 3=Customer, 4=Manager
            return roleId switch
            {
                1 => "Admin",
                2 => "Supporter", 
                3 => "Customer",
                4 => "Manager",
                _ => "Unknown"
            };
        }

        public IActionResult Dashboard()
        {
            SetUserRole();
            return View();
        }

        public IActionResult Services()
        {
            SetUserRole();
            return View();
        }

        public IActionResult ManageItems()
        {
            SetUserRole();
            return View();
        }

        public IActionResult Reports()
        {
            SetUserRole();
            return View();
        }

        public IActionResult Profile()
        {
            SetUserRole();
            return View();
        }
    }
}
