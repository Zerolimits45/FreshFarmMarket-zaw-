using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.Model;
using FreshFarmMarket.ViewModels;
using System.Text.Encodings.Web;
using System.Web;

namespace FreshFarmMarket.Pages
{
	[ValidateAntiForgeryToken]
	public class RegisterModel : PageModel
	{

		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }

		[BindProperty]
		public Register RModel { get; set; }

		public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}



		public void OnGet()
		{
		}


		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var extension = Path.GetExtension(RModel.Photo.FileName).ToLower();
				var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
				var protector = dataProtectionProvider.CreateProtector("MySecretKey");

				if (extension != ".jpg" && extension != ".jpeg")
				{
					ModelState.AddModelError("RModel.Photo", "Only .jpg and .jpeg files are allowed");
					return Page();
				}
				var user = new ApplicationUser()
				{
					FullName = RModel.FullName,
					UserName = RModel.Email,
					CreditCard = protector.Protect(RModel.CreditCard),
					Gender = RModel.Gender,
					MobileNo = RModel.MobileNo,
					DeliveryAddress = RModel.DeliveryAddress,
					Email = RModel.Email,
					Photo = RModel.Photo.FileName,
					AboutMe = HttpUtility.HtmlEncode(RModel.AboutMe)

				};
				var result = await userManager.CreateAsync(user, RModel.Password);
				if (result.Succeeded)
				{
					await signInManager.SignInAsync(user, false);
					return RedirectToPage("Index");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return Page();
		}

	}
}
