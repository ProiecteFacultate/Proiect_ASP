using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Proiect.Data;
using Proiect.Models;
using System.Collections;

namespace Proiect.Controllers
{
    public class GroupController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public GroupController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Show(int id)   //group id
        {
            Group group = (from group2 in db.Groups
                           where group2.Id == id
                           select group2).SingleOrDefault();
          
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                ViewBag.Authenticated = true;
                ViewBag.CurrentUser = _userManager.GetUserId(User);

                Group_Member isMember = (from group_member in db.Group_Members
                                         where group_member.GroupId == id
                                         && group_member.UserId == _userManager.GetUserId(User)
                                         select group_member).SingleOrDefault();

                if (isMember != null)
                {
                    var roleInGroup = (from group_member in db.Group_Members
                                       where group_member.GroupId == id
                                       && group_member.UserId == _userManager.GetUserId(User)
                                       select group_member.Role).SingleOrDefault();

                    ViewBag.IsMember = true;
                    ViewBag.RoleInGroup = roleInGroup.ToString();
                }
                else
                {
                    ViewBag.IsMember = false;
                }

                if (User.IsInRole("User"))
                    ViewBag.Role = "User";
                else if (User.IsInRole("Admin"))
                    ViewBag.Role = "Admin";
            }
            else
            {
                ViewBag.Authenticated = false;
                ViewBag.IsMember = false;
            }

            ViewBag.GroupId = id;    
            ViewBag.GroupName = group.Name.ToString();
            ViewBag.Description = group.Description.ToString();
            ViewBag.Members = getGroupMembers(id);
            ViewBag.Messages = getMessages(id);

            return View();
        }

        public IActionResult NewGroup()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Group group = new Group();
                return View(group);
            }
            else
            {
                TempData["message"] = "You need to be authenticated to create a group!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public IActionResult NewGroup(Group newGroup)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    db.Groups.Add(newGroup);
                    db.SaveChanges();

                    Group_Member groupMember = new Group_Member();
                    groupMember.GroupId = newGroup.Id;
                    groupMember.UserId = _userManager.GetUserId(User);
                    groupMember.Role = "Admin";
                    db.Group_Members.Add(groupMember);
                    db.SaveChanges();
                    return RedirectToAction("Feed", "Home");
                }
                else
                {
                    TempData["message"] = "You need to be authenticated to create a group!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                return View(newGroup);
            }
        }

        public IActionResult Edit(int id)           //the id of the group to be edited
        {
            Group group = (from group2 in db.Groups
                           where group2.Id == id
                           select group2).SingleOrDefault();

            Group_Member isMember = (from group_member in db.Group_Members
                                     where group_member.GroupId == id
                                     && group_member.UserId == _userManager.GetUserId(User)
                                     select group_member).SingleOrDefault();

            if (isMember != null)
            {
                var roleInGroup = (from group_member in db.Group_Members
                                   where group_member.GroupId == id
                                   && group_member.UserId == _userManager.GetUserId(User)
                                   select group_member.Role).SingleOrDefault();

                if(roleInGroup.ToString().Equals("Admin"))
                    return View(group);
                else
                {
                    TempData["message"] = "You can't edit this group!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                TempData["message"] = "You can't edit this group!";
                return RedirectToAction("Warning", "Home");
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Group updatedGroup)
        {
            Group group = db.Groups.Find(id);

            if (ModelState.IsValid)
            {
                Group_Member isMember = (from group_member in db.Group_Members
                                         where group_member.GroupId == id
                                         && group_member.UserId == _userManager.GetUserId(User)
                                         select group_member).SingleOrDefault();

                if (isMember != null)
                {
                    var roleInGroup = (from group_member in db.Group_Members
                                       where group_member.GroupId == id
                                       && group_member.UserId == _userManager.GetUserId(User)
                                       select group_member.Role).SingleOrDefault();

                    if (roleInGroup.ToString().Equals("Admin"))
                    {
                        group.Name = updatedGroup.Name;
                        group.Description = updatedGroup.Description;
                        db.SaveChanges();
                        return RedirectToAction("Feed", "Home");
                    }
                    else
                    {
                        TempData["message"] = "You can't edit this group!";
                        return RedirectToAction("Warning", "Home");
                    }
                }
                else
                {
                    TempData["message"] = "You can't edit this group!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                return View(updatedGroup);
            }
        }

        public IActionResult Delete(int id)           //the id of the group to be deleted
        {
            Group group = (from group2 in db.Groups
                           where group2.Id == id
                           select group2).SingleOrDefault();

            Group_Member isMember = (from group_member in db.Group_Members
                                     where group_member.GroupId == id
                                     && group_member.UserId == _userManager.GetUserId(User)
                                     select group_member).SingleOrDefault();

            if (isMember != null || User.IsInRole("Admin"))
            {
                var roleInGroup = (from group_member in db.Group_Members
                                   where group_member.GroupId == id
                                   && group_member.UserId == _userManager.GetUserId(User)
                                   select group_member.Role).SingleOrDefault();

                if (User.IsInRole("Admin") || roleInGroup.ToString().Equals("Admin"))
                {
                    var group_members = (from group_member in db.Group_Members
                                         where group_member.GroupId == id
                                         select group_member);

                    foreach (Group_Member member in group_members)
                        db.Group_Members.Remove(member);

                    db.Groups.Remove(group);
                    db.SaveChanges();
                    return RedirectToAction("Feed", "Home");
                }
            }

            //   else
            TempData["message"] = "You can't delete this group!";
            return RedirectToAction("Warning", "Home");
        }

        public IActionResult Kick(int groupId, string userId)         //the id of the user to be kicked
        {
            Group_Member isMember = (from group_member in db.Group_Members
                                     where group_member.GroupId == groupId
                                     && group_member.UserId == _userManager.GetUserId(User)
                                     select group_member).SingleOrDefault();

            Group_Member kickedUserIsMember = (from group_member in db.Group_Members
                                               where group_member.GroupId == groupId
                                               && group_member.UserId == userId
                                               select group_member).SingleOrDefault();


            if (isMember != null && kickedUserIsMember != null)
            {
                var roleInGroup = (from group_member in db.Group_Members
                                   where group_member.GroupId == groupId
                                   && group_member.UserId == _userManager.GetUserId(User)
                                   select group_member.Role).SingleOrDefault();

                var kickedUserRoleInGroup = (from group_member in db.Group_Members
                                   where group_member.GroupId == groupId
                                   && group_member.UserId == userId
                                   select group_member.Role).SingleOrDefault();

                if (roleInGroup.ToString().Equals("Admin") && !kickedUserRoleInGroup.ToString().Equals("Admin"))
                {
                    db.Group_Members.Remove(kickedUserIsMember);
                    db.SaveChanges();
                    return RedirectToAction("Show", "Group", new {id = groupId});
                }
            }

            //else 
            TempData["message"] = "You can't kcik members from this group!";
            return RedirectToAction("Warning", "Home");
        }

        public IActionResult Leave(int groupId, string userId)     //user to be kicked
        {
            if(userId == _userManager.GetUserId(User))
            {
                Group_Member member = (from group_member in db.Group_Members
                                       where group_member.GroupId == groupId
                                       && group_member.UserId == userId
                                       select group_member).SingleOrDefault();

                if(member != null)
                {
                    db.Group_Members.Remove(member);
                    db.SaveChanges();
                    return RedirectToAction("Feed", "Home");
                }
                else
                {
                    TempData["message"] = "You are not in this group!";
                    return RedirectToAction("Warning", "Home");
                }
            }
            else
            {
                TempData["message"] = "You can't make other members leave from this group!";
                return RedirectToAction("Warning", "Home");
            }
        }

        //Methods

        ArrayList getGroupMembers(int id)    //group id
        {
            ArrayList groupMembers = new ArrayList();

            var query1 = (from group_member in db.Group_Members
                         where group_member.GroupId == id
                         select group_member);

            foreach (var group_member in query1)
            {
                ArrayList data = new ArrayList();

                var query2 = (from userProfile in db.Profiles
                              where userProfile.Id == group_member.UserId
                              select userProfile.ProfileUsername).SingleOrDefault();

                data.Add(group_member.UserId);
                data.Add(query2).ToString();
                data.Add(group_member.Role);

                groupMembers.Add(data);
            }

            return groupMembers;
        }

        ArrayList getMessages(int id)    //group id
        {
            ArrayList messages = new ArrayList();

            var query1 = (from group_messages in db.Group_Messages
                          where group_messages.GroupId == id
                          select group_messages);

            foreach (var group_message in query1)
            {
                ArrayList data = new ArrayList();

                var query2 = (from userProfile in db.Profiles
                              where userProfile.Id == group_message.CreatorId
                              select userProfile.ProfileUsername).SingleOrDefault();

                data.Add(group_message.CreatorId);
                data.Add(query2).ToString();
                data.Add(group_message.Message);
                data.Add(group_message.DatePosted);
                data.Add(group_message.Status);
                data.Add(group_message.Id);

                messages.Add(data);
            }

            return messages;
        }
    }
}
