using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AccountController()
        {
            _context = new ApplicationDBContext();
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(UserRegistrationViewModel model, HttpPostedFileBase ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Pass = model.Pass
                };

                if (ProfilePicture != null && ProfilePicture.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ProfilePicture.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/ProfilePictures/"), fileName);
                    ProfilePicture.SaveAs(path);
                    user.Img = "~/Content/ProfilePictures/" + fileName;
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("RegisterSuccess");
            }

            return View(model);
        }

        // GET: /Account/RegisterSuccess
        public ActionResult RegisterSuccess()
        {
            return View();
        }

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.EmailOrMobile || u.Mobile == model.EmailOrMobile);

                if (user != null && user.Pass == model.Pass)
                {
                    if (user.IsActive) // Check if the user is active
                    {
                        Session["UserId"] = user.ID;
                        Session["ProfilePicturePath"] = user.Img;

                        // Redirect to appropriate dashboard based on admin status
                        if (user.IsAdmin)
                        {
                            return RedirectToAction("AdminDashboard", "Account");
                        }
                        else
                        {
                            return RedirectToAction("UserDashboard", "Account");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your account is currently inactive. Please contact the admin.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return View(model);
        }



        // GET: /Account/UserDashboard
        public ActionResult UserDashboard()
        {
            var userId = (int)Session["UserId"];
            var userDetails = _context.Users.FirstOrDefault(u => u.ID == userId);

            if (userDetails == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(userDetails);
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Account/EditProfilePicture
        public ActionResult EditProfilePicture()
        {
            var userId = (int)Session["UserId"];
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new EditProfilePictureViewModel
            {
                UserID = user.ID,
                Img = user.Img
            };

            return View(viewModel);
        }

        // POST: /Account/EditProfilePicture
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfilePicture(EditProfilePictureViewModel model, HttpPostedFileBase ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.UserID);

                if (user != null)
                {
                    if (ProfilePicture != null && ProfilePicture.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(ProfilePicture.FileName);
                        string path = Path.Combine(Server.MapPath("~/Content/ProfilePictures/"), fileName);
                        ProfilePicture.SaveAs(path);
                        user.Img = "~/Content/ProfilePictures/" + fileName;
                        _context.SaveChanges();

                        Session["ProfilePicturePath"] = user.Img;
                        return RedirectToAction("UserDashboard");
                    }
                }
            }

            return View(model);
        }

        // GET: /Account/EditUser
        public ActionResult EditUser()
        {
            var userId = (int)Session["UserId"];
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new EditUserViewModel
            {
                ID = user.ID,
                UserName = user.UserName,
                Email = user.Email,
                Mobile = user.Mobile
            };

            return View(model);
        }

        // POST: /Account/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.ID);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.Mobile = model.Mobile;

                    _context.Entry(user).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("UserDashboard");
                }
            }

            return View(model);
        }

        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            var userId = (int)Session["UserId"];
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new ChangePasswordViewModel
            {
                UserID = user.ID
            };

            return View(viewModel);
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.UserID);

                if (user != null && user.Pass == model.CurrentPassword)
                {
                    user.Pass = model.NewPassword;
                    _context.SaveChanges();

                    return RedirectToAction("UserDashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid current password.");
                }
            }

            return View(model);
        }


        // GET: /Account/AdminDashboard
        public ActionResult AdminDashboard(int page=1, int pageSize = 5)
        {
            var users = _context.Users.OrderBy(u => u.ID)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            var totalUsers = _context.Users.Count();
            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            var viewModel = new AdminDashboardViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // POST: /Account/ActivateUser
        [HttpPost]
        public ActionResult ActivateUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.IsActive = true;
                _context.SaveChanges();
            }
            return RedirectToAction("AdminDashboard");
        }

        // POST: /Account/DeactivateUser
        [HttpPost]
        public ActionResult DeactivateUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.IsActive = false;
                _context.SaveChanges();
            }
            return RedirectToAction("AdminDashboard");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}