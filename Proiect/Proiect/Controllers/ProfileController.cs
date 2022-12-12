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

        public IActionResult Show(string id)   //this is the id for the user who got the visited profile; which can or not be the same user from _userManager (is different when we visit other people profile)
        {
            if (profileExists(id) == false)
                createProfile(id);
                

            string profileUsername = (from profile in db.Profiles
                                      where profile.Id == id
                                      select profile.ProfileUsername).SingleOrDefault();

            string description = (from profile in db.Profiles
                                  where profile.Id == id
                                  select profile.Description).SingleOrDefault();

            ViewBag.ProfileUserId = id;
            ViewBag.ProfileUsername = profileUsername;
            ViewBag.Description = description;
            ViewBag.Id = id;

            return View();
        }

        public IActionResult Edit(string id)
        {
            Profile profile = (from profile2 in db.Profiles
                               where profile2.Id == id
                               select profile2).SingleOrDefault();

            return View(profile);
        }

        [HttpPost]
        public IActionResult Edit(string id, Profile updatedProfile)       
        {
            Profile profile = db.Profiles.Find(id);
        
            if (ModelState.IsValid)
            {
                Console.WriteLine("Model valid");

                profile.ProfileUsername = updatedProfile.ProfileUsername;
                profile.Description = updatedProfile.Description;
                db.SaveChanges();

                return RedirectToAction("Show", new { id = id });
            }
            else
            {
                Console.WriteLine("Model invalid");
                return View(updatedProfile);
            }
        }

        bool profileExists(string id)       //checks if profile already exists for the given user;   returns true if profile exists, false otherwise
        {
            string profileId = (from profile in db.Profiles
                                where profile.Id == id
                                select profile.Id).SingleOrDefault();

            if (profileId == null)
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

        void createProfile(string id)
        {
            Profile profile = new Profile();
            profile.Id = id;
            profile.ProfileUsername = "Username Unset";
            profile.Description = "Description";

            db.Profiles.Add(profile);
            db.SaveChanges();

        }
    }
}
