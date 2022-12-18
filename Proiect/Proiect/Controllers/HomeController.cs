using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect.Data;
using Proiect.Models;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace Proiect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public HomeController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Feed()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string id = _userManager.GetUserId(User);         
                ViewBag.Authenticated = true;
                ViewBag.Id = id;
                ViewBag.Friends = getFriendshipsData(id);
                ViewBag.Groups = getGroups(id);
            }
            else
                ViewBag.Authenticated = false;

            return View();
        }

        public IActionResult Warning()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //Methods

        ArrayList getFriendshipsData(string id)
        {
            ArrayList friendsId = new ArrayList();
            ArrayList friendsData = new ArrayList();      //id, profile username

            var query1 = (from friendship in db.Friendships
                          where friendship.Friend_1_Key == id
                          select friendship.Friend_2_Key);

            var query2 = (from friendship in db.Friendships
                          where friendship.Friend_2_Key == id
                          select friendship.Friend_1_Key);

            foreach (var friend in query1)
                friendsId.Add(friend);

            foreach (var friend in query2)
                friendsId.Add(friend);

            for(int i = 0; i < friendsId.Count; i++)
            {
                var username = (from profile in db.Profiles
                                where profile.Id == friendsId[i]
                                select profile.ProfileUsername).SingleOrDefault();

                ArrayList data = new ArrayList();
                data.Add(friendsId[i]);
                data.Add(username.ToString());
                friendsData.Add(data);
            }

            return friendsData;
        }

        ArrayList getGroups(string id)
        {
            ArrayList groupsData = new ArrayList();

            var query1 = (from group_member in db.Group_Members
                          where group_member.UserId == id
                          select group_member);

            foreach(var group_member in query1)
            {
                ArrayList data = new ArrayList();

                var query2 = (from group2 in db.Groups
                              where group2.Id == group_member.GroupId
                              select group2.Name).SingleOrDefault();

                data.Add(group_member.GroupId);
                data.Add(query2).ToString();
                data.Add(group_member.Role);

                groupsData.Add(data);
            }

            return groupsData;
        }
    }
}