using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;

namespace Proiect.Controllers
{
    public class PostCommentController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PostCommentController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult NewComment(int postId)        //the id of the post we are commenting to
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Post_Comment comment = new Post_Comment();
                ViewBag.PostId = postId;

                return View(comment);
            }
            else
            {
                TempData["message"] = "You need to be authenticated to post comments!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public IActionResult NewComment(int postId, Post_Comment newComment)
        {
            Console.WriteLine(postId);
            newComment.DatePosted = DateTime.Now;
            newComment.CreatorId = _userManager.GetUserId(User);
            newComment.PostId = postId;

            if (ModelState.IsValid)
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    db.Post_Comments.Add(newComment);
                    db.SaveChanges();
                    return RedirectToAction("Show", "Profile", new { id = getPostCreatorId(postId) });    //the creator of the comment can be other user, so you are sent to its profile, not to your own
                }
                else
                {
                    TempData["message"] = "You need to be authenticated to post comments!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                return View(newComment);
            }
        }

        public IActionResult Edit(int id)           //the id of the comment to be edited
        {
            Post_Comment comment = (from comment2 in db.Post_Comments
                                    where comment2.Id == id
                                    select comment2).SingleOrDefault();

            if (comment.CreatorId == _userManager.GetUserId(User))
                return View(comment);
            else
            {
                TempData["message"] = "You can't edit this comment!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Post_Comment updatedComment)
        {
            Post_Comment comment = db.Post_Comments.Find(id);

            if (ModelState.IsValid)
            {
                if (comment.CreatorId == _userManager.GetUserId(User))
                {
                    comment.Message = updatedComment.Message;
                    db.SaveChanges();
                    return RedirectToAction("Feed", "Home");
                }
                else
                {
                    TempData["message"] = "You can't edit this comment!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                return View(updatedComment);
            }
        }

        public IActionResult Delete(int id)           //the id of the post to be deleted
        {
            Post_Comment comment = (from comment2 in db.Post_Comments
                                    where comment2.Id == id
                                    select comment2).SingleOrDefault();

            if (comment.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Post_Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Feed", "Home");
            }
            else
            {
                TempData["message"] = "You can't delete this comment!";
                return RedirectToAction("Warning", "Home");
            }
        }


        //METHODS

        string getPostCreatorId(int postId)
        {
            return (from post in db.Posts
                    where post.Id == postId
                    select post.CreatorId).SingleOrDefault().ToString();
        }
    }
}
