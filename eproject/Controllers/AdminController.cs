using eproject.Models;
using eproject.Security;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace eproject.Controllers
{
    public class AdminController : Controller
    {
        private context db = new context();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult index([Bind(Include = "email,pass")] User ouser)
        {
            if (ouser.email == null || ouser.pass == null)
            {
                return View(ouser);
            }
            var user = db.user.Where(m => m.email == ouser.email).SingleOrDefault();
            if (user != null && pass.VerifyHashedPassword(user.pass, ouser.pass))
            {
                Session["userId"] = user.id;
                Session["email"] = user.email;
                return RedirectToAction("user", "admin");
            }
            ModelState.AddModelError("", "email or password error");
            return View(ouser);
        }
        //custom authorize
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult user(string search)
        {
            if (search != null && search != "")
            {
                return View(db.user.Where(c => c.role == "ROLE_USER" && (c.email.ToLower().Contains(search.ToLower()) || c.name.ToLower().Contains(search.ToLower()))));
            }
            return View(db.user.Where(c => c.role == "ROLE_USER"));
        }
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult edituser(Guid id, bool enabled)
        {
            var user = db.user.Find(id);
            user.enabled = enabled;
            db.SaveChanges();
            return RedirectToAction("user");
        }
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult recipe(string search)
        {
            if (search != null && search != "")
            {
                return View(db.recipe.Where(c => c.name.ToLower().Contains(search.ToLower()) || c.category.ToLower().Contains(search.ToLower())));
            }
            return View(db.recipe);
        }
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult editRecipe(Guid id, bool enabled)
        {
            var recipe = db.recipe.Find(id);
            recipe.enabled = enabled;
            db.SaveChanges();
            return RedirectToAction("recipe");
        }

        // GET: Contest/Sorting/Searching/Paging 
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult contestIndex(string sortOrder, string currentFilter, string search, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name" : "";
            ViewBag.CreateDateSort = sortOrder == "datecreate" ? "createdate" : "datecreate";
            ViewBag.StartDateSort = sortOrder == "datestart" ? "stardate" : "datestart";
            ViewBag.EndDateSort = sortOrder == "dateend" ? "enddate" : "dateend";

            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }

            ViewBag.CurrentFilter = search;

            var contests = from c in db.contest
                           select c;

            // Searching
            if (!String.IsNullOrEmpty(search))
            {
                contests = contests.Where(c => c.name.Contains(search));
            }

            //Sorting
            switch (sortOrder)
            {
                case "datecreate":
                    contests = contests.OrderBy(c => c.createAt);
                    break;
                case "name":
                    contests = contests.OrderBy(c => c.name);
                    break;
                case "":
                    contests = contests.OrderByDescending(c => c.name);
                    break;
                case "datestart":
                    contests = contests.OrderBy(c => c.startDate);
                    break;
                case "stardate":
                    contests = contests.OrderByDescending(c => c.startDate);
                    break;
                case "dateend":
                    contests = contests.OrderBy(c => c.endDate);
                    break;
                case "enddate":
                    contests = contests.OrderByDescending(c => c.endDate);
                    break;
                default:
                    contests = contests.OrderByDescending(c => c.createAt);
                    break;
            }

            //Paging
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(contests.ToPagedList(pageNumber, pageSize));
        }

        // GET: Contest/Create
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult contestCreate()
        {
            return View();
        }

        // POST: Contest/Create
        [AuthorizeUser(role = "ROLE_ADMIN")]
        [HttpPost]
        public ActionResult contestCreate(Contest newcontest)
        {
            //Check contest name exist
            if (db.contest.Any(x => x.name == newcontest.name))
            {
                ModelState.AddModelError("name", "Contest name already exist");
                return View(newcontest);
            }
            //Check start date
            if (newcontest.startDate < DateTime.Today)
            {
                ModelState.AddModelError("startDate", "Start date cannot be less than current date");
                return View(newcontest);
            }
            //Check end date
            if (newcontest.endDate <= newcontest.startDate)
            {
                ModelState.AddModelError("endDate", "End date cannot be equal or less than start date");
                return View(newcontest);
            }
            if (ModelState.IsValid)
            {
                db.contest.Add(newcontest);
                newcontest.createAt = DateTime.Today;
                db.SaveChanges();
                return RedirectToAction("contestIndex", "admin");
            }
            return View(newcontest);
        }


        // GET: Contest/Details
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult contestDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = db.contest.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // GET: Contest/Edit
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult contestEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = db.contest.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // POST: Contest/Edit
        [AuthorizeUser(role = "ROLE_ADMIN")]
        [HttpPost]
        public ActionResult contestEdit([Bind(Include = "id,name,content,createAt,startDate,endDate")] Contest editcontest)
        {
            //Check contest name exist          
            if (db.contest.Any(x => x.name == editcontest.name && x.id != editcontest.id))
            {
                ModelState.AddModelError("name", "Contest name already exist");
                return View(editcontest);
            }
            //Check start date
            if (editcontest.startDate < editcontest.createAt)
            {
                ModelState.AddModelError("startDate", "Start date cannot be less than create date");
                return View(editcontest);
            }
            //Check end date
            if (editcontest.endDate <= editcontest.startDate)
            {
                ModelState.AddModelError("endDate", "End date cannot be equal or less than start date");
                return View(editcontest);
            }
            if (ModelState.IsValid)
            {
                db.Entry(editcontest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("contestIndex", "admin");
            }
            return View(editcontest);
        }

        // GET: Contests/Delete
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult contestDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = db.contest.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // POST: Contests/Delete
        [AuthorizeUser(role = "ROLE_ADMIN")]
        [HttpPost, ActionName("contestDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult contestDeleteConfirmed(Guid id)
        {
            Contest contest = db.contest.Find(id);
            db.contest.Remove(contest);
            db.SaveChanges();
            return RedirectToAction("contestIndex", "admin");
        }

        // GET: ContestRecipe/Sorting/Searching/Paging 
        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult contestRecipe(string sortOrder, string currentFilter, string search, int? page, Guid? id, DateTime? endDate, Guid? winner)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name" : "";
            ViewBag.CreateDateSort = sortOrder == "datecreate" ? "createdate" : "datecreate";
            ViewBag.StartDateSort = sortOrder == "category" ? "recipecategory" : "category";
            ViewBag.endDate = endDate;
                    
            ViewBag.winner = winner;

            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }

            ViewBag.CurrentFilter = search;

            var contestRecipes = from cr in db.recipe
                                 where cr.contest_id == id
                                 select cr;

            //Searching
            if (!String.IsNullOrEmpty(search))
            {
                contestRecipes = contestRecipes.Where(cr => cr.name.Contains(search));
            }

            //Sorting
            switch (sortOrder)
            {
                case "datecreate":
                    contestRecipes = contestRecipes.OrderBy(cr => cr.createAt);
                    break;
                case "name":
                    contestRecipes = contestRecipes.OrderBy(cr => cr.name);
                    break;
                case "":
                    contestRecipes = contestRecipes.OrderByDescending(cr => cr.name);
                    break;
                case "category":
                    contestRecipes = contestRecipes.OrderBy(cr => cr.category);
                    break;
                case "recipecategory":
                    contestRecipes = contestRecipes.OrderByDescending(cr => cr.category);
                    break;
                default:
                    contestRecipes = contestRecipes.OrderByDescending(cr => cr.createAt);
                    break;
            }

            //Paging
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(contestRecipes.ToPagedList(pageNumber, pageSize));
        }

        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult setWinner(Guid? id, Guid? winner, DateTime? enddate)
        {
            Contest contest = db.contest.Find(id);
            contest.winner = winner;
            db.SaveChanges();
            return RedirectToAction("contestRecipe", "Admin", new { id = id, winner = winner, enddate = enddate });
        }

        [AuthorizeUser(role = "ROLE_ADMIN")]
        public ActionResult report()
        {
            var topViewRecipe = db.recipe.OrderByDescending(m => m.viewCount).Take(3).ToList();
            ViewBag.toprecipe = topViewRecipe;
            var topRate = (from Recipe
                           in db.recipe
                           join rate in db.ratting on Recipe.id equals rate.recipe_id
                           group Recipe by Recipe.name into Recipe
                           select new RateCount
                           {
                               name = Recipe.Key,
                               count = Recipe.Count()
                           }).Take(3).ToList();
            ViewBag.topRate = topRate;

            return View();
        }
    }

    public class RateCount
    {
        public string name { get; set; }
        public int count { get; set; }
    }
}