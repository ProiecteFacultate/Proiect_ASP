using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;
using System.Text.RegularExpressions;

namespace Proiect.Controllers
{
    public class GroupMessageController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public GroupMessageController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult NewMessage(int groupId)       
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Group_Member isMember = (from group_member in db.Group_Members
                                         where group_member.GroupId == groupId
                                         && group_member.UserId == _userManager.GetUserId(User)
                                         select group_member).SingleOrDefault();

                if(isMember != null)
                {
                    Group_Message message = new Group_Message();
                    ViewBag.GroupId = groupId;

                    return View(message);
                }
            }

            //else
            TempData["message"] = "You need to be member of this group to write messages!";
            return RedirectToAction("Warning", "Home");
        }

        [HttpPost]
        public IActionResult NewMessage(int groupId, Group_Message newMessage)
        {
            newMessage.DatePosted = DateTime.Now;
            newMessage.CreatorId = _userManager.GetUserId(User);
            newMessage.GroupId = groupId;
            newMessage.Status = "Active";

            if (ModelState.IsValid)
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    Group_Member isMember = (from group_member in db.Group_Members
                                             where group_member.GroupId == groupId
                                             && group_member.UserId == _userManager.GetUserId(User)
                                             select group_member).SingleOrDefault();

                    if (isMember != null)
                    {
                        db.Group_Messages.Add(newMessage);
                        db.SaveChanges();
                        return RedirectToAction("Show", "Group", new { id = groupId });
                    }
                }

                //else
                TempData["message"] = "You need to be member of this group to write messages!";
                return RedirectToAction("Warning", "Home");
            }
            else
            {
                return View(newMessage);
            }
        }

        public IActionResult Edit(int id)           //the id of the message to be edited
        {
            Group_Message message = (from message2 in db.Group_Messages
                                    where message2.Id == id
                                    select message2).SingleOrDefault();

            Group_Member isMember = (from group_member in db.Group_Members
                                     where group_member.GroupId == message.GroupId
                                     && group_member.UserId == _userManager.GetUserId(User)
                                     select group_member).SingleOrDefault();

            if (message.CreatorId == _userManager.GetUserId(User) && isMember != null)    //we still need to check if the user is member because he may send a message and then leave the group
            {
                if(message.Status.Equals("Active"))
                    return View(message);
                else
                {
                    TempData["message"] = "This message was deleted!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                TempData["message"] = "You can't edit this message!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Group_Message updatedMessage)
        {
            Group_Message message = db.Group_Messages.Find(id);

            if (ModelState.IsValid)
            {
                Group_Member isMember = (from group_member in db.Group_Members
                                         where group_member.GroupId == message.GroupId
                                         && group_member.UserId == _userManager.GetUserId(User)
                                         select group_member).SingleOrDefault();

                if (message.CreatorId == _userManager.GetUserId(User) && isMember != null)    //we still need to check if the user is member because he may send a message and then leave the group
                {
                    message.Message = updatedMessage.Message;
                    db.SaveChanges();
                    return RedirectToAction("Show", "Group", new { id = message.GroupId });
                }
                else
                {
                    TempData["message"] = "You can't edit this message!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                return View(updatedMessage);
            }
        }
    }
}
