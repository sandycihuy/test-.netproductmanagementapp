using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductManagementApp.Data;
using ProductManagementApp.Models;
using Microsoft.AspNetCore.Identity;

namespace ProductManagementApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AverageProductPrice { get; set; }
        public int ActiveProductsCount { get; set; }
        public int ActiveProductsPercentage => TotalProducts > 0 ? (int)Math.Round((double)ActiveProductsCount / TotalProducts * 100) : 0;
        public decimal ProductsPerCategory => TotalCategories > 0 ? Math.Round((decimal)TotalProducts / TotalCategories, 1) : 0;
        public string LastUpdatedTime => DateTime.Now.ToString("h:mm tt");

        public List<Product> RecentProducts { get; set; } = new();
        public List<ActivityViewModel> RecentActivities { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Page();
            }

            await LoadDashboardData();
            return Page();
        }

        private async Task LoadDashboardData()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted);

            if (!isAdmin)
            {
                productsQuery = productsQuery.Where(p => p.UserId == currentUser.Id);
            }

            TotalProducts = await productsQuery.CountAsync();
            TotalCategories = await _context.ProductCategories.CountAsync(c => !c.IsDeleted);
            
            var activeProducts = productsQuery.Where(p => p.IsActive);
            ActiveProductsCount = await activeProducts.CountAsync();
            
            TotalInventoryValue = await productsQuery.SumAsync(p => p.Price);
            AverageProductPrice = TotalProducts > 0 ? await productsQuery.AverageAsync(p => p.Price) : 0;

            RecentProducts = await productsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            await GenerateRecentActivities(currentUser, isAdmin);
        }

        private async Task GenerateRecentActivities(ApplicationUser currentUser, bool isAdmin)
        {
            var activities = new List<ActivityViewModel>();
            var now = DateTime.UtcNow;

            var recentProducts = await _context.Products
                .Where(p => !p.IsDeleted && (isAdmin || p.UserId == currentUser.Id))
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .Select(p => new { p.Name, p.CreatedAt, p.User.UserName })
                .ToListAsync();

            foreach (var product in recentProducts)
            {
                var timeAgo = GetTimeAgo(now, product.CreatedAt);
                activities.Add(new ActivityViewModel
                {
                    Title = "Product Added",
                    Description = $"{product.Name} was added by {product.UserName}",
                    TimeAgo = timeAgo,
                    Icon = "box",
                    Color = "primary"
                });
            }

            if (isAdmin)
            {
                var recentUsers = await _userManager.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(2)
                    .Select(u => new { u.UserName, u.CreatedAt })
                    .ToListAsync();

                foreach (var user in recentUsers)
                {
                    var timeAgo = GetTimeAgo(now, user.CreatedAt);
                    activities.Add(new ActivityViewModel
                    {
                        Title = "New User",
                        Description = $"{user.UserName} registered",
                        TimeAgo = timeAgo,
                        Icon = "user-plus",
                        Color = "success"
                    });
                }
            }

            activities.Add(new ActivityViewModel
            {
                Title = "System Update",
                Description = "Dashboard has been updated with new features",
                TimeAgo = "Just now",
                Icon = "sync",
                Color = "info"
            });

            RecentActivities = activities
                .OrderByDescending(a => a.TimeAgo)
                .Take(5)
                .ToList();
        }

        private string GetTimeAgo(DateTime now, DateTime dateTime)
        {
            var timeSpan = now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "Just now";

            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} min ago";

            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";

            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";

            return dateTime.ToString("MMM d, yyyy");
        }
    }

    public class ActivityViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }
}
