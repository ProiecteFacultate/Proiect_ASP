using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;
using System;
using System.Data;
using System.Diagnostics;


namespace Proiect.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProfileController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Show(string profileUserId)   //this is the id for the user who got the visited profile; which can or not be the same user from _userManager (is different when we visit other people profile)
        {
            if (profileExists(profileUserId) == false)
                createProfile(profileUserId);
                

            string profileUsername = (from profile in db.Profiles
                                      where profile.Id == profileUserId
                                      select profile.ProfileUsername).SingleOrDefault();

            string description = (from profile in db.Profiles
                                  where profile.Id == profileUserId
                                  select profile.Description).SingleOrDefault();

            ViewBag.ProfileUserId = profileUserId;
            ViewBag.ProfileUsername = profileUsername;
            ViewBag.Description = description;

            ViewBag.ProfileUserId = profileUserId;

            return View();
        }

        public IActionResult Edit(string profileUserId)
        {
            ViewBag.ProfileUserId = profileUserId;
            return View();
        }

        [HttpPost]
        public IActionResult Edit(string profileUserId, Profile updatedProfile)
        {
            Profile profile = db.Profiles.Find(profileUserId);
            Console.WriteLine("Nume actual: " + updatedProfile.ProfileUsername);

            if(ModelState.IsValid)
            {
                Console.WriteLine("New name: " + updatedProfile.ProfileUsername);
                Console.WriteLine("New desc: " + updatedProfile.Description);

                profile.ProfileUsername = updatedProfile.ProfileUsername;
                profile.Description = updatedProfile.Description;
                db.SaveChanges();       
            }
            string profileUserId2 = profileUserId;
            return RedirectToAction("Show", new { profileUserId  = profileUserId2 });
        }

        bool profileExists(string profileUserId)       //checks if profile already exists for the given user;   returns true if profile exists, false otherwise
        {
            string id = (from profile in db.Profiles
                         where profile.Id == profileUserId
                         select profile.Id).SingleOrDefault();

            if (id == null)
            {
                Console.WriteLine("Profile doesn't exist!");
                return false;
            }
            else
            {
                Console.WriteLine("Profile exists!");
                return true;
            }
        }

        void createProfile(string profileUserId)
        {
            Profile profile = new Profile();
            profile.Id = profileUserId;
            profile.ProfileUsername = "Username Unset";
            profile.Description = "Description";

            db.Profiles.Add(profile);
            db.SaveChanges();

        }
    }
}
