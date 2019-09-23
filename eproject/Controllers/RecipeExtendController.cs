using eproject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eproject.Helper;
using eproject.Security;

namespace eproject.Controllers
{
    public class RecipeExtendController : Controller
    {
        private context db = new context();
        // GET: RecipeExtend
        [HttpPost]

        public ActionResult rate(int ratting, string user, string recipe)
        {
            if (recipe == "" || user == "")
            {
                return Json(new { success = false, responseText = "you must log in before ratting!" }, JsonRequestBehavior.AllowGet);
            }
            Guid userId = Guid.Parse(user);
            Guid recipeId = Guid.Parse(recipe);
            var orate = db.ratting.Where(c => c.recipe_id == recipeId && c.own == userId);

            if (orate.Count() != 0)
            {
                return Json(new { success = false, responseText = "You already rate this recipe!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Ratting nrate = new Ratting();
                nrate.rate = ratting;
                nrate.own = userId;
                nrate.recipe_id = recipeId;
                db.ratting.Add(nrate);
                db.SaveChanges();
                return Json(new { success = true, responseText = "You have rate this recipe " + ratting + " star!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult recipeDetail(Guid id)
        {
            double rate = 0;
            int rateCount = 0;
            var recipe = db.recipe.Find(id);
            if (recipe.enabled == false)
            {
                return RedirectToAction("index", "home");
            }
            var type = recipe.type;
            if (type != "Free")
            {
                if (Session["userId"] != null)
                {
                    var u = db.user.Find(Session["userId"]);
                    if (u.expireDate < DateTime.Now && u.role != "ROLE_ADMIN")
                    {
                        return RedirectToAction("error", "home", new { msg = "Only vip user can see this recipe, go to profile to buy vip access!" });
                    }
                }
                else
                    return RedirectToAction("error", "home", new { msg = "Only vip user can see this recipe, go to profile to buy vip access!" });

            }
            ViewBag.manager = db.user.Find(recipe.manager).email;
            var srate = db.ratting.Where(m => m.recipe_id == recipe.id).Select(m => m.rate);
            if (srate.Count() > 0)
            {
                rate = srate.Average();
                rateCount = srate.Count();
            }
            //random 3 recipe with same category
            var relate = db.recipe.Where(m => m.category == recipe.category).OrderBy(x => Guid.NewGuid()).Take(3);

            var comment = db.feedBack.Where(m => m.recipe_id == recipe.id).ToArray();

            foreach (var item in comment)
            {
                item.ago = DateTimeConvert.TimeAgo(item.createAt);
                item.name = db.user.Find(item.own).name;

            }
            ViewBag.comment = comment;
            ViewBag.rateCount = rateCount;
            ViewBag.rate = rate;
            ViewBag.recipeId = id;
            ViewBag.relate = relate;
            //view count
            if (Request.Cookies["ViewedPage"] != null)
            {
                if (Request.Cookies["ViewedPage"][string.Format("pId_{0}", id)] == null)
                {
                    HttpCookie cookie = (HttpCookie)Request.Cookies["ViewedPage"];
                    cookie[string.Format("pId_{0}", id)] = "1";
                    cookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(cookie);
                    recipe.viewCount = recipe.viewCount + 1;
                    db.SaveChanges();
                }
            }
            else
            {
                HttpCookie cookie = new HttpCookie("ViewedPage");
                cookie[string.Format("pId_{0}", id)] = "1";
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(cookie);

                recipe.viewCount = recipe.viewCount + 1;
                db.SaveChanges();
            }
            return View(recipe);
        }
        [HttpPost]
        public ActionResult createComment(string content, string user, string recipe)
        {
            if (recipe == "" || user == "")
            {
                return Json(new { success = false, responseText = "you must log in before ratting!" }, JsonRequestBehavior.AllowGet);
            }
            if (content.Length < 20 || content.Length > 150)
            {
                return Json(new { success = false, responseText = "comment must at 20-150 character" }, JsonRequestBehavior.AllowGet);
            }
            Guid userId = Guid.Parse(user);
            Guid recipeId = Guid.Parse(recipe);
            var fb = new FeedBack();
            fb.own = userId;
            fb.recipe_id = recipeId;
            fb.content = content;
            fb.createAt = DateTime.Now;
            // if(ModelState.IsValid)
            db.feedBack.Add(fb);
            db.SaveChanges();

            return Json(new { success = true, responseText = "Success" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult deleteComment(Guid id, Guid recipeId)
        {
            var com = db.feedBack.Find(id);
            if (com != null)
            {
                db.feedBack.Remove(com);
                db.SaveChanges();
            }
            return RedirectToAction("recipeDetail", new { id = recipeId });
        }
    }
}