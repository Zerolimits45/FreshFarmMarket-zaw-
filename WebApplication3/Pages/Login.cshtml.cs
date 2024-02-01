using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace FreshFarmMarket.Pages
{
	[ValidateAntiForgeryToken]
    public class LoginModel : PageModel
	{

		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;

		private readonly UserManager<ApplicationUser> userManager;

		private readonly AuthDbContext dbContext;
		public LoginModel(AuthDbContext dbContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.dbContext = dbContext;
		}

		public void OnGet()
		{
		}

		public bool ValidateCaptcha()
		{
			string Response = Request.Form["g-recaptcha-response"];
			bool valid = false;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LejIWIpAAAAALTfePrqJSlr3URirHR6M1cIT32k&response=" + Response);
			try
			{
				using (WebResponse wResponse = request.GetResponse())
				{
					using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
					{
						string jsonResponse = readStream.ReadToEnd();
						var data = JsonSerializer.Deserialize<CapRes>(jsonResponse);
						valid = Convert.ToBoolean(data.success);
					}
				}
				return valid;
			}
			catch (WebException ex)
			{
				throw ex;
			}

		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				if (!ValidateCaptcha())
				{
					ModelState.AddModelError("", "Captcha is not valid");
					return Page();
				}
				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
				LModel.RememberMe, false);
				if (identityResult.Succeeded)
				{
					var user = await userManager.FindByEmailAsync(LModel.Email); // finds user by email
					string guid = Guid.NewGuid().ToString(); // generates a new guid
					if (user != null)
					{
						HttpContext.Session.SetString("Token", guid); // sets the token to session
						user.Token = guid; // set token to user
						await userManager.UpdateAsync(user); // updates the user
					}
					var audit = new Audit
					{
						UserID = user.Id,
						Action = "Logged in",
						ActionTime = DateTime.Now,
					};
					dbContext.AuditLog.Add(audit);
					await dbContext.SaveChangesAsync();
				}
				{
					return RedirectToPage("Index");
				}
				ModelState.AddModelError("", "Username or Password incorrect");
			}
			return Page();
		}


	}
}
