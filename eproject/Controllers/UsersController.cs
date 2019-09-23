using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using eproject.Models;
using eproject.Security;
using eproject.Helper;
using System.Text.RegularExpressions;

namespace eproject.Controllers
{
    public class UsersController : Controller
    {
        private context db = new context();
        //private pass passCode;

        //public UsersController(pass security)
        //{
        //    this.passCode = security;

        //}



        // GET: Users
        public ActionResult Index()
        {
            return View(db.user.ToList());
        }

        [HttpGet]
        public ActionResult login()
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("index", "home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult login([Bind(Include = "email,pass")] User ouser)
        {
            if (ouser.email == null || ouser.pass == null)
            {
                // ModelState.AddModelError("", "Filed can not blank!");
                return View(ouser);
            }
            var user = db.user.Where(m => m.email == ouser.email).SingleOrDefault();
            if (user != null && pass.VerifyHashedPassword(user.pass, ouser.pass))
            {
                Session["userId"] = user.id;
                Session["email"] = user.email;
                return RedirectToAction("index", "home");
            }
            ModelState.AddModelError("", "email or password error");
            return View(ouser);
        }

        public ActionResult logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("index", "home"); ;
        }

        // GET: Users/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.user.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,email,gender,phone,pass")] User user, string expired)
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("index", "home");
            }

            if (expired == "10")
            {
                user.expireDate = DateTime.Now.AddMonths(6);
            }
            if (expired == "50")
            {
                user.expireDate = DateTime.Now.AddMonths(12);
            }
            if (expired == "0")
            {
                user.expireDate = DateTime.Now;
            }
            //check exist email
            if (db.user.Any(x => x.email == user.email))
            {
                ModelState.AddModelError("email", "email existed!");
                return View(user);
            }
            var regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            if (!regex.IsMatch(user.pass))
            {
                ModelState.AddModelError("pass", "Contest name must have minimum eight characters, at least one letter and one number");
                return View(user);
            }
            if (ModelState.IsValid)
            {
                user.id = Guid.NewGuid();
                string a = pass.HashPassword(user.pass).ToString();
                user.pass = a;
                Guid theVerificationCode = Guid.NewGuid();
                user.emailVerify = theVerificationCode.ToString();
                db.user.Add(user);
                db.SaveChanges();
                var link = Request.Url.Scheme + "://" + Request.Url.Authority + @Url.Action("verifyEmail", "Users", new { id = user.id, code = theVerificationCode });
                new Mail().sendVerifyMail(link, user.email);
                Session["userId"] = user.id;
                Session["email"] = user.email;

                return RedirectToAction("error", "home", new { msg = "Create account success, please go to your email to verify account!" });
            }

            return View(user);
        }

        public ActionResult verifyEmail(Guid? id, Guid? code)
        {
            var user = db.user.Find(id);
            if (user == null || user.emailVerify != code.ToString() || id == null || code == null)
            {
                ViewBag.msg = "ERROR!";
            }
            else
            {
                ViewBag.msg = "Verify email success!";
                user.enabled = true;
                user.emailVerify = null;
                db.SaveChanges();
            }
            return View();
        }


        // GET: Users/Edit/5
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult Edit(Guid? id)
        {
            User cUser = db.user.Find(Session["userId"]);
            if (cUser.enabled == false)
            {

                return RedirectToAction("error", "home", new { msg = "Please verify your email fisrt!!" });
            }
            if (Session["userId"].ToString() != id.ToString())
            {
                return RedirectToAction("index", "home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.user.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult Edit(User nuser, string expired)
        {
            var t = nuser.expireDate;
            if (expired == "10")
            {
                nuser.expireDate = (t != null && t > DateTime.Now) ? t.AddMonths(6) : DateTime.Now.AddMonths(6);
            }
            if (expired == "50")
            {
                nuser.expireDate = (t != null && t > DateTime.Now) ? t.AddMonths(6) : DateTime.Now.AddMonths(12);
            }

            if (ModelState.IsValid)
            {
                db.Entry(nuser).State = EntityState.Modified;

                db.SaveChanges();
                return View(nuser);
            }
            return View(nuser);
        }

        // GET: Users/Delete/5

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.user.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            User user = db.user.Find(id);
            db.user.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult recipe()
        {
            var userid = Guid.Parse(Session["userId"].ToString());
            return View(db.recipe.Where(c => c.manager == userid).ToList());

        }
     
    }
}
