using NourStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace NourStore.Controllers
{
    public class AccountController : Controller
    {
        private NourStoreProject6Entities1 db = new NourStoreProject6Entities1();
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Check if username or email already exists
                var existingUser = db.Users.FirstOrDefault(u => u.Username == model.Username || u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username or Email already exists.");
                    return View(model);
                }

                model.CreatedAt = DateTime.Now;
                model.Password = HashPassword(model.Password); // Hash the password
                db.Users.Add(model);
                db.SaveChanges();

                // Set session data
                Session["UserID"] = model.UserID;
                Session["Username"] = model.Username;
                Session["Email"] = model.Email;

                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        // Utility method for hashing passwords
        private string HashPassword(string password)
        {
            // Simple hash method; use a better one in production
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Password));
                var user = db.Users.FirstOrDefault(u => u.Username == Username && u.Password == hashedPassword);

                if (user != null)
                {
                    // Set session data
                    Session["UserID"] = user.UserID;
                    Session["Username"] = user.Username;
                    Session["Email"] = user.Email;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();  // Clear all session data
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UserProfile()
        {
            if (Session["UserID"] == null)
            {
                // Redirect to login if the session is null
                return RedirectToAction("Login", "Account");
            }

            var userProfile = new UserProfile
            {
                UserId = (int)Session["UserID"],
                UserName = Session["Username"].ToString(),
                Email = Session["Email"].ToString()
            };

            return View(userProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserID"];
                var user = db.Users.Find(userId);

                if (user != null)
                {
                    // Update username and email
                    user.Username = model.UserName;
                    user.Email = model.Email;

                    // Handle password change
                    if (!string.IsNullOrEmpty(model.CurrentPassword) &&
                        !string.IsNullOrEmpty(model.NewPassword) &&
                        !string.IsNullOrEmpty(model.ConfirmNewPassword))
                    {
                        string hashedCurrentPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.CurrentPassword));
                        if (user.Password == hashedCurrentPassword)
                        {
                            user.Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.NewPassword));
                        }
                        else
                        {
                            ModelState.AddModelError("", "The current password is incorrect.");
                            return View(model);
                        }
                    }

                    db.SaveChanges();

                    // Update session data
                    Session["Username"] = user.Username;
                    Session["Email"] = user.Email;

                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("UserProfile");
                }
            }

            return View(model);
        }
    }
}