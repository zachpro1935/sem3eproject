using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eproject.Models;
using PagedList;



namespace eproject.Controllers
{
    public class ContestController : Controller
    {
        private context db = new context();
        // GET: Contest/Sorting/Searching/Paging 
        public ActionResult Index(string sortOrder, string currentFilter, string search, int? page)
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

            if (!String.IsNullOrEmpty(search))
            {
                contests = contests.Where(c => c.name.Contains(search));
            }

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

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(contests.ToPagedList(pageNumber, pageSize));
        }

        // GET: Contest/Details
        public ActionResult Readmore(Guid? id)
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

        // GET: ContestRecipe/Sorting/Searching/Paging 
        public ActionResult contestRecipe(string sortOrder, string currentFilter, string search, int? page, Guid? id, DateTime? endDate, Guid? winner)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name" : "";
            ViewBag.CreateDateSort = sortOrder == "datecreate" ? "createdate" : "datecreate";
            ViewBag.StartDateSort = sortOrder == "category" ? "recipecategory" : "category";
            ViewBag.endDate = endDate;
            if (winner != null)
            {
                var win = db.user.Find( db.recipe.Find(winner).manager).name ;
                ViewBag.winner = win;
            }

            ViewBag.w = winner;
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
    }
}