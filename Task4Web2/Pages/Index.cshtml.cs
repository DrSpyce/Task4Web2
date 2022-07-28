using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Task4Web2.Areas.Identity.Data;
using Task4Web2.Data;

namespace Task4Web2.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly Task4Web2Context _context;
        private DateTime EndDate = new DateTime(2222, 06, 06);

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(ILogger<IndexModel> logger, Task4Web2Context context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public IList<User> Users { get; set; } = default!;
        [BindProperty]
        public List<string> AreChecked { get; set; }
        [TempData]
        public string Message { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Users != null)
            {
                Users = await _context.Users.ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            int i = 0;
            foreach (var userId in AreChecked)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user.Email == User.Identity.Name)
                {
                    await _signInManager.SignOutAsync();
                    await _userManager.DeleteAsync(user);
                    return RedirectToPage();
                }    
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                    i++;
                }
            }
            Message += $"{i} users were deleted";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostBlockAsync()
        {
            int i = 0;
            foreach (var userId in AreChecked)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user.Email == User.Identity.Name)
                {
                    await _signInManager.SignOutAsync();
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, EndDate);
                    user.Status = "Blocked";
                    await _userManager.UpdateAsync(user);
                    return RedirectToPage();
                }
                if (user != null)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, EndDate);
                    await _userManager.UpdateSecurityStampAsync(user);
                    user.Status = "Blocked";
                    await _userManager.UpdateAsync(user);
                    i++;
                }
            }
            Message += $"{i} users were blocked";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnblockAsync()
        {
            int i = 0;
            foreach (var userId in AreChecked)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.SetLockoutEnabledAsync(user, false);
                    await _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));
                    await _userManager.UpdateSecurityStampAsync(user);
                    user.Status = "Active";
                    await _userManager.UpdateAsync(user);
                    i++;
                }
            }
            Message += $"{i} users were unblocked";
            return RedirectToPage();
        }
    }
}