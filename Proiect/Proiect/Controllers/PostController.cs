using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PostController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult NewPost()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Post post = new Post();
                return View(post);
            }
            else
            {
                TempData["message"] = "You need to be authenticated to create posts!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public IActionResult NewPost(Post newPost)
        {
            newPost.DatePosted = DateTime.Now;
            newPost.CreatorId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    db.Posts.Add(newPost);
                    db.SaveChanges();
                    return RedirectToAction("Show", "Profile", new { id = _userManager.GetUserId(User) });
                }
                else
                {
                    TempData["message"] = "You need to be authenticated to create posts!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                return View(newPost);
            }
        }

        public IActionResult Edit(int id)           //the id of the post to be edited
        {
            Post post = (from post2 in db.Posts
                            where post2.Id == id
                            select post2).SingleOrDefault();

            if (post.CreatorId == _userManager.GetUserId(User))
                return View(post);
            else
            {
                TempData["message"] = "You can't edit this post!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Post updatedPost)
        {
            Post post = db.Posts.Find(id);

            if (ModelState.IsValid)
            {
                if (post.CreatorId == _userManager.GetUserId(User))
                {
                    post.Message = updatedPost.Message;
                    db.SaveChanges();
                    return RedirectToAction("Feed", "Home");
                }
                else
                {
                    TempData["message"] = "You can't edit this post!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                return View(updatedPost);
            }
        }

        public IActionResult Delete(int id)           //the id of the post to be deleted
        {
            Post post = (from post2 in db.Posts
                         where post2.Id == id
                         select post2).SingleOrDefault();

            if(post.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                var comments = (from post_comment in db.Post_Comments
                                where post_comment.PostId == post.Id
                                select post_comment);

                foreach (Post_Comment comment in comments)
                    db.Post_Comments.Remove(comment);

                db.Posts.Remove(post);
                db.SaveChanges();
                return RedirectToAction("Feed", "Home");
            }
            else
            {
                TempData["message"] = "You can't delete this post!";
                return RedirectToAction("Warning", "Home");
            }
        }
    }
}
