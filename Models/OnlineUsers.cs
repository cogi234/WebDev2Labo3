using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MoviesDBManager.Models
{
    public static class OnlineUsers
    {
        public static List<int> ConnectedUsersId
        {
            get
            {
                if (HttpRuntime.Cache["OnlineUsers"] == null)
                    HttpRuntime.Cache["OnlineUsers"] = new List<int>();
                return (List<int>)HttpRuntime.Cache["OnlineUsers"];
            }
        }

        public static void AddSessionUser(int userId)
        {
            if (!ConnectedUsersId.Contains(userId))
            {
                HttpContext.Current.Session["UserId"] = userId;
                ConnectedUsersId.Add(userId);
            }
        }
        public static void RemoveSessionUser()
        {
            HttpContext.Current?.Session.Abandon();
            if (IsOnline((int)HttpContext.Current.Session["UserId"]))
                ConnectedUsersId.Remove((int)HttpContext.Current.Session["UserId"]);
        }
        public static User GetSessionUser()
        {
            if (HttpContext.Current.Session["UserId"] != null)
            {
                User currentUser = DB.Users.Get((int)HttpContext.Current.Session["UserId"]);
                return currentUser;
            }
            return null;
        }
        public static bool Write_Access()
        {
            User sessionUser = OnlineUsers.GetSessionUser();
            if (sessionUser != null)
            {
                return sessionUser.IsPowerUser || sessionUser.IsAdmin;
            }
            return false;
        }
        public static bool IsOnline(int userId) => ConnectedUsersId.Contains(userId);

        public class UserAccess : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                User sessionUser = OnlineUsers.GetSessionUser();
                if (sessionUser != null)
                {
                    if (sessionUser.Blocked)
                    {
                        RemoveSessionUser();
                        httpContext.Response.Redirect("~/Accounts/Login?message=Compte bloqué!");
                        return false;
                    }
                    return true;
                }
                httpContext.Response.Redirect("~/Accounts/Login?message=Accès non autorisé!");
                return false;

            }
        }
        public class PowerUserAccess : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                User sessionUser = GetSessionUser();
                if (sessionUser != null && (sessionUser.IsPowerUser || sessionUser.IsAdmin))
                    return true;
                else
                {
                    if (sessionUser != null)
                        RemoveSessionUser();
                    httpContext.Response.Redirect("~/Accounts/Login?message=Accès non autorisé!", true);
                }
                return false;
            }
        }
        public class AdminAccess : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                User sessionUser = GetSessionUser();
                if (sessionUser != null && sessionUser.IsAdmin)
                    return true;
                else
                {
                    if (sessionUser != null)
                        RemoveSessionUser();
                    httpContext.Response.Redirect("~/Accounts/Login?message=Accès non autorisé!");
                }
                return true;
            }
        }
    }
}