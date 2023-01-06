using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;
using System.Collections;

namespace Proiect.Controllers
{
    public class SearchBarController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SearchBarController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult UsersResults(string name)
        {
            ArrayList profiles = new ArrayList();

            var query = (from profile in db.Profiles
                         where profile.ProfileUsername.ToLower().Contains(name.ToLower())
                         || name.ToLower().Contains(profile.ProfileUsername.ToLower())
                         select profile);

            foreach (var profil in query)
            {
                ArrayList aux = new ArrayList();
                aux.Add(profil.Id);
                aux.Add(profil.ProfileUsername);


                if (profil.ProfileImage != null)
                    aux.Add("/UserAddedImages/profile/" + profil.Id + ".jpg");
                else
                    aux.Add("/UserAddedImages/profile/default.jpg");

                profiles.Add(aux);
            }

            ViewBag.Name = name;
            ViewBag.Profiles = profiles;

            return View();
        }

        public IActionResult GroupsResults(string name)
        {
            ArrayList groups = new ArrayList();

            var query = (from grup in db.Groups
                         where  grup.Name.ToLower().Contains(name.ToLower())
                         || name.ToLower().Contains(grup.Name.ToLower())
                         select grup);

            foreach (var grup in query)
            {
                ArrayList aux = new ArrayList();
                aux.Add(grup.Id);
                aux.Add(grup.Name);


                if (grup.GroupImage != null)
                    aux.Add("/UserAddedImages/group/" + grup.Id + ".jpg");
                else
                    aux.Add("/UserAddedImages/group/default.jpg");

                groups.Add(aux);
            }

            ViewBag.Name = name;
            ViewBag.Groups = groups;

            return View();
        }
    }
}
