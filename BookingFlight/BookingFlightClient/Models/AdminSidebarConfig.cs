using System.Collections.Generic;

namespace BookingFlightClient.Models
{
    public class SidebarMenuItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Action { get; set; }
        public string Badge { get; set; }
        public bool IsExternal { get; set; }
        public bool IsLogout { get; set; }
        public List<SidebarMenuItem> SubItems { get; set; }
        public bool HasPermission { get; set; } = true;
    }

    public class SidebarMenuSection
    {
        public string SectionTitle { get; set; }
        public List<SidebarMenuItem> Items { get; set; }
        public bool HasPermission { get; set; } = true;
    }

    public static class AdminSidebarConfig
    {
        public static List<SidebarMenuSection> GetMenuSections()
        {
            return new List<SidebarMenuSection>
            {
                new SidebarMenuSection
                {
                    SectionTitle = "Main",
                    Items = new List<SidebarMenuItem>
                    {
                        new SidebarMenuItem
                        {
                            Title = "Dashboard",
                            Icon = "fas fa-chart-line",
                            Url = "/Admin/Dashboard",
                            Action = "Dashboard"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Analytics",
                            Icon = "fas fa-chart-pie",
                            Url = "/Admin/Analytics",
                            Action = "Analytics"
                        }
                    }
                },
                new SidebarMenuSection
                {
                    SectionTitle = "Management",
                    Items = new List<SidebarMenuItem>
                    {
                        new SidebarMenuItem
                        {
                            Title = "Users",
                            Icon = "fas fa-users",
                            Url = "/Admin/Users",
                            Action = "Users",
                            Badge = "12"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Flights",
                            Icon = "fas fa-plane-departure",
                            Url = "/Admin/Flights",
                            Action = "Flights"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Bookings",
                            Icon = "fas fa-ticket-alt",
                            Url = "/Admin/Bookings",
                            Action = "Bookings",
                            Badge = "5"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Airports",
                            Icon = "fas fa-map-marker-alt",
                            Url = "/Admin/Airports",
                            Action = "Airports"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Aircraft",
                            Icon = "fas fa-plane",
                            Url = "/Admin/Aircraft",
                            Action = "Aircraft"
                        }
                    }
                },
                new SidebarMenuSection
                {
                    SectionTitle = "Financial",
                    Items = new List<SidebarMenuItem>
                    {
                        new SidebarMenuItem
                        {
                            Title = "Revenue",
                            Icon = "fas fa-dollar-sign",
                            Url = "/Admin/Revenue",
                            Action = "Revenue"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Payments",
                            Icon = "fas fa-credit-card",
                            Url = "/Admin/Payments",
                            Action = "Payments"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Reports",
                            Icon = "fas fa-file-alt",
                            Url = "/Admin/Reports",
                            Action = "Reports"
                        }
                    }
                },
                new SidebarMenuSection
                {
                    SectionTitle = "System",
                    Items = new List<SidebarMenuItem>
                    {
                        new SidebarMenuItem
                        {
                            Title = "Settings",
                            Icon = "fas fa-cog",
                            Url = "/Admin/Settings",
                            Action = "Settings"
                        },
                        new SidebarMenuItem
                        {
                            Title = "System Logs",
                            Icon = "fas fa-list-alt",
                            Url = "/Admin/Logs",
                            Action = "Logs"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Backup",
                            Icon = "fas fa-database",
                            Url = "/Admin/Backup",
                            Action = "Backup"
                        },
                        new SidebarMenuItem
                        {
                            Title = "Back to Website",
                            Icon = "fas fa-home",
                            Url = "/Home",
                            Action = "Home",
                            IsExternal = true
                        },
                        new SidebarMenuItem
                        {
                            Title = "Logout",
                            Icon = "fas fa-sign-out-alt",
                            Url = "/Login/Logout",
                            Action = "Logout",
                            IsLogout = true
                        }
                    }
                }
            };
        }
    }
}
