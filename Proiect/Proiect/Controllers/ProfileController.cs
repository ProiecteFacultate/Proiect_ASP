using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using Proiect.Data;
using Proiect.Models;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;


namespace Proiect.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProfileController(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _env = env;
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
            ViewBag.VisitorIsFriend = checkFriend(id);
            ViewBag.ProfileUsername = profile.ProfileUsername.ToString();
            ViewBag.Description = profile.Description.ToString();
            ViewBag.Privacy = profile.Privacy;
            ViewBag.posts = getPosts(id);

            if (profile.ProfileImage != null)
                ViewBag.ProfileImage = "/UserAddedImages/profile/" + id + ".jpg";
            else
                ViewBag.ProfileImage = "/UserAddedImages/profile/default.jpg";

            return View();
        }

        public IActionResult Edit(string id)          
        {
            if (id == _userManager.GetUserId(User))
            {
                Profile profile = (from profile2 in db.Profiles
                                   where profile2.Id == id
                                   select profile2).SingleOrDefault();

                profile.Priv = getPrivacyOptions();
                return View(profile);
            }
            else
            {
                TempData["message"] = "You can't edit this profile!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Profile updatedProfile, IFormFile? ProfileImage)       //IFormFile ProfileImage
        {
            Profile profile = db.Profiles.Find(id);
            Console.WriteLine(updatedProfile.Privacy);
            if (ModelState.IsValid)
            {
                if (profile.Id == _userManager.GetUserId(User))
                {
                    profile.ProfileUsername = updatedProfile.ProfileUsername;
                    profile.Description = updatedProfile.Description;
                    profile.Privacy = updatedProfile.Privacy;

                    if (ProfileImage != null)    //ProfileImage.Length > 0
                    {
                        var storagePath = Path.Combine(_env.WebRootPath, "UserAddedImages/profile", id + ".jpg");
                        var databaseFileName = "/UserAddedImages/profile/" + id + ".jpg";
                        using (var fileStream = new FileStream(storagePath, FileMode.Create))
                        {
                            await ProfileImage.CopyToAsync(fileStream);
                        }

                        profile.ProfileImage = databaseFileName;
                    }

                    db.SaveChanges();
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
                    TempData["message"] = "You can't edit this profile!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                Console.WriteLine("Model invalid");
                profile.Priv = getPrivacyOptions();
                return View(updatedProfile);
            }
        }

        public IActionResult AddFriend(string id)
        {
            if (User.Identity.IsAuthenticated == true)
            {
               if(checkFriend(id) == false)
                {
                    Friendship friendship = new Friendship();
                    friendship.Friend_1_Key = _userManager.GetUserId(User);
                    friendship.Friend_2_Key = id;
                    db.Friendships.Add(friendship);
                    db.SaveChanges();
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
                    TempData["message"] = "You are already friends!";
                    return RedirectToAction("Warning", "Home");
                }

            }
            else
            {
                TempData["message"] = "You can't add friends as guest!";
                return RedirectToAction("Warning", "Home");
            }
        }

        public IActionResult DeleteFriend(string id)
        {
            if (User.Identity.IsAuthenticated == true)
            {
                if (checkFriend(id) == true)
                {
                    Friendship friendship1 = (from friendship in db.Friendships
                                              where friendship.Friend_1_Key == id
                                              && friendship.Friend_2_Key == _userManager.GetUserId(User)
                                              select friendship).SingleOrDefault();
                    if (friendship1 != null)
                        db.Friendships.Remove(friendship1);

                    Friendship friendship2 = (from friendship in db.Friendships
                                              where friendship.Friend_2_Key == id
                                              && friendship.Friend_1_Key == _userManager.GetUserId(User)
                                              select friendship).SingleOrDefault();
                    if (friendship2 != null)
                        db.Friendships.Remove(friendship2);

                    db.SaveChanges();
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
                    TempData["message"] = "You are not friends!";
                    return RedirectToAction("Warning", "Home");
                }

            }
            else
            {
                TempData["message"] = "You can't delete friends as guest!";
                return RedirectToAction("Warning", "Home");
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
            profile.Privacy = "Public";

            db.Profiles.Add(profile);
            db.SaveChanges();
        }

        ArrayList getPosts(string id)          //the id of the profile; it gets also the comments for every post
        {
            ArrayList posts = new ArrayList();

            var query = (from post in db.Posts
                         where post.CreatorId == id
                         select post);

            foreach (var post in query)
            {
                ArrayList aux = new ArrayList();
                ArrayList comments = new ArrayList();

                var creatorUsername = (from profile in db.Profiles
                                       where profile.Id == post.CreatorId
                                       select profile.ProfileUsername).SingleOrDefault();

                //now we get the comments for the post

                var query2 = (from post_comment in db.Post_Comments
                              where post_comment.PostId == post.Id
                              select post_comment);

                foreach(var comment in query2)
                {
                    var commentCreatorUsername = (from profile in db.Profiles
                                                  where profile.Id == comment.CreatorId
                                                  select profile.ProfileUsername).SingleOrDefault();

                    ArrayList aux2 = new ArrayList();

                    aux2.Add(comment.Id);
                    aux2.Add(commentCreatorUsername.ToString());
                    aux2.Add(comment.Message.ToString());
                    aux2.Add(comment.DatePosted);
                    aux2.Add(comment.CreatorId);

                    comments.Add(aux2);
                }

                aux.Add(post.Id);
                aux.Add(creatorUsername.ToString());
                aux.Add(post.Message.ToString());
                aux.Add(post.DatePosted);
                aux.Add(comments);
                posts.Add(aux);
            }

            return posts;
        }

        IEnumerable<SelectListItem> getPrivacyOptions()
        {
            var optionsList = new List<SelectListItem>();

            optionsList.Add(new SelectListItem
            {
                Value = "Public",
                Text = "Public"
            });

            optionsList.Add(new SelectListItem
            {
                Value = "Private",
                Text = "Private"
            });

            return optionsList;
        }

        bool checkFriend(string id)      //the id of the visited profile
        {
            var query1 = (from friendship in db.Friendships
                          where friendship.Friend_1_Key == id
                          && friendship.Friend_2_Key == _userManager.GetUserId(User)
                          select friendship).SingleOrDefault();

            var query2 = (from friendship in db.Friendships
                          where friendship.Friend_2_Key == id
                          && friendship.Friend_1_Key == _userManager.GetUserId(User)
                          select friendship).SingleOrDefault();

            return query1 != null || query2 != null;
        }
    }
}
