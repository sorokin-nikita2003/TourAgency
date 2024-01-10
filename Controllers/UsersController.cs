using agency.Data;
using agency.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace agency.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _context.Users.Where(p => p.Id != userId).ToListAsync();
            var roles = await _context.UserRoles.Where(p => p.UserId != userId).ToListAsync();
            UserChangeRole ucr = new UserChangeRole();
            ucr.users = users;
            ucr.userRoles = roles;

            return View(ucr);
        }
        [HttpPost]
        public async Task<IActionResult> Search(string email)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = await _context.Users.Where(p => p.Id != userId).ToListAsync();
            if(!String.IsNullOrEmpty(email))
            {
                users = users.Where(p => p.UserName.ToLower().Contains(email.ToLower())).ToList();
            }
            var roles = await _context.UserRoles.Where(p => p.UserId != userId).ToListAsync();
            UserChangeRole ucr = new UserChangeRole();
            
            ucr.users = users;
            ucr.userRoles = roles;
            return PartialView(ucr);
        }

        public async Task<IActionResult> SaveRole(string id ,string ans)
        {

            if(ans == "1")
            {
                var usr1 = await _context.UserRoles.FindAsync(id, "2");
                var usr2 = await _context.UserRoles.FindAsync(id, "1");
                if(usr1 != null)
                {
                    _context.UserRoles.Remove(usr1);
                }
                if (usr2 != null)
                {
                    _context.UserRoles.Remove(usr2);
                }
                var r = new IdentityUserRole<string>
                {
                    RoleId = "1",
                    UserId = id
                };
                await _context.UserRoles.AddAsync(r);

            }
            else if(ans == "2")
            {
                var usr1 = await _context.UserRoles.FindAsync(id, "2");
                var usr2 = await _context.UserRoles.FindAsync(id, "1");
                if (usr1 != null)
                {
                    _context.UserRoles.Remove(usr1);
                }
                if (usr2 != null)
                {
                    _context.UserRoles.Remove(usr2);
                }

                var r = new IdentityUserRole<string>
                {
                    RoleId = "2",
                    UserId = id
                };
                await _context.UserRoles.AddAsync(r);
            }
            else if (ans == null)
            {
                var usr1 = await _context.UserRoles.FindAsync(id, "2");
                var usr2 = await _context.UserRoles.FindAsync(id, "1");
                if (usr1 != null)
                {
                    _context.UserRoles.Remove(usr1);
                }
                if (usr2 != null)
                {
                    _context.UserRoles.Remove(usr2);
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
