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
                ViewBag.Posts = getPosts(id);
            }
            else
            {
                ViewBag.Authenticated = false;
                ViewBag.Posts = getRandomPosts(new ArrayList());
            }

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
                
                var profileImage = (from profile in db.Profiles
                                    where profile.Id == friendsId[i]
                                    select profile.ProfileImage).SingleOrDefault();

                ArrayList data = new ArrayList();
                data.Add(friendsId[i]);
                data.Add(username.ToString());

                if (profileImage != null)
                    data.Add("/UserAddedImages/profile/" + friendsId[i] + ".jpg");
                else
                    data.Add(ViewBag.ProfileImage = "/UserAddedImages/profile/default.jpg");

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


                var groupImage = (from group2 in db.Groups
                              where group2.Id == group_member.GroupId
                              select group2.GroupImage).SingleOrDefault();

                data.Add(group_member.GroupId);
                data.Add(query2).ToString();

                if (groupImage != null)
                    data.Add("/UserAddedImages/group/" + group_member.GroupId + ".jpg");
                else
                    data.Add("/UserAddedImages/group/default.jpg");

                groupsData.Add(data);
            }

            return groupsData;
        }

        ArrayList getPosts(string id)          //the id of the user (logged user); it gets also the comments for every post
        {
            ArrayList posts = new ArrayList();
            ArrayList friendsIds = getFriendIds(id);

            //from here we take current user posts
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

                foreach (var comment in query2)
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

            //from here we take user's friends posts

            foreach (var friendId in friendsIds)
            {
                var query11 = (from post in db.Posts
                             where post.CreatorId == friendId
                             select post);

                foreach (var post in query11)
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

                    foreach (var comment in query2)
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
            }

            foreach (var post in getRandomPosts(friendsIds))
                posts.Add(post);

            return posts;
        }

        ArrayList getFriendIds(string id)  //id of the user whom friends we want;  this returns only the friends ids, the method above also friends details
        {
            ArrayList friends = new ArrayList();

            var queryFriends1 = (from friendship in db.Friendships
                                 where friendship.Friend_1_Key == id
                                 select friendship.Friend_2_Key);

            var queryFriends2 = (from friendship in db.Friendships
                                 where friendship.Friend_2_Key == id
                                 select friendship.Friend_1_Key);

            foreach(var friendId in queryFriends1)
                friends.Add(friendId);

            foreach (var friendId in queryFriends2)
                friends.Add(friendId);

            return friends;
        }

        ArrayList getRandomPosts(ArrayList exceptionUsers)  //gets random posts; used for guests, but also for logged users; for logged it takes users that arent in exceptionUsers(friends)
        {
            ArrayList posts = new ArrayList();
            var query = (from post in db.Posts
                         join profile in db.Profiles on post.CreatorId equals profile.Id
                         where profile.Privacy == "Public"
                         select post); 

            foreach(var post in query)
            {
                bool inExceptionUsers = false;
                for (int i = 0; i < exceptionUsers.Count; i++)
                    if (post.CreatorId.Equals(exceptionUsers[i]))
                    {
                        inExceptionUsers = true;
                        break;
                    }

                  
                if(inExceptionUsers == false)
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

                    foreach (var comment in query2)
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
            }

            return posts;
        }
    }
}