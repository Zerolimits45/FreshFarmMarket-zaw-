using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;

namespace FreshFarmMarket.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly AuthDbContext dbContext;
		public LogoutModel(AuthDbContext dbContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.dbContext = dbContext;
		}
		public void OnGet()
        {
        }
		public async Task<IActionResult> OnPostLogoutAsync()
		{
			var user = await userManager.GetUserAsync(User);
			user.Token = null;
			await userManager.UpdateAsync(user);
			var audit = new Audit
			{
				UserID = user.Id,
				Action = "Logged Out",
				ActionTime = DateTime.Now,
			};
			dbContext.AuditLog.Add(audit); // adds audit to database
			await dbContext.SaveChangesAsync(); // saves the changes made to database

			await signInManager.SignOutAsync();
			HttpContext.Session.Clear(); // clears session
			return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
