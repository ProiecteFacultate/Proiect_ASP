using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Proiect.Data;
using Proiect.Models;
using System;
using System.Collections;
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
                
            Profile profile = (from profile2 in db.Profiles
                               where profile2.Id == id
                               select profile2).SingleOrDefault();

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                ViewBag.Authenticated = true;
                ViewBag.CurrentUser = _userManager.GetUserId(User);

                if (User.IsInRole("User"))
                    ViewBag.Role = "User";
                else if (User.IsInRole("Admin"))
                    ViewBag.Role = "Admin";
            }
            else
                ViewBag.Authenticated = false;

            ViewBag.Id = id;            //the id of the requested profile, not hte one of the user that visits the profile
            ViewBag.ProfileUsername = profile.ProfileUsername.ToString();
            ViewBag.Description = profile.Description.ToString();
            ViewBag.Privacy = profile.Visible;
            ViewBag.posts = getPosts(id);

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

        public IActionResult NewPost()
        {
            Post post = new Post();

            return View(post);
        }

        [HttpPost]
        public IActionResult NewPost(Post newPost)
        {
            newPost.DatePosted = DateTime.Now;
            newPost.CreatorId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Posts.Add(newPost);
                db.SaveChanges();
                return RedirectToAction("Show", new {id = _userManager.GetUserId(User)});
            }
            else
            {
                return View(newPost);
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
            profile.Visible = true;

            db.Profiles.Add(profile);
            db.SaveChanges();
        }

        ArrayList getPosts(string id)          //the id of the profile 
        {
            ArrayList posts = new ArrayList();

            var query = (from post in db.Posts
                         where post.CreatorId == id
                         select post);

            foreach (var post in query)
            {
                ArrayList aux = new ArrayList();
                
                var creatorUsername = (from profile in db.Profiles
                                       where profile.Id == post.CreatorId
                                       select profile.ProfileUsername).SingleOrDefault();

                aux.Add(post.Message.ToString());
                aux.Add(creatorUsername.ToString());
                aux.Add(post.DatePosted);

                posts.Add(aux);
            }

            return posts;
        }
    }
}
