using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eproject.Models;
using eproject.Security;

namespace eproject.Controllers
{
    public class RecipesController : Controller
    {
        private context db = new context();

        // GET: Recipes
        public ActionResult Index(string name)
        {          
            var model = db.recipe.Where(c => c.enabled == true).ToList();
            foreach(var item in model)
            {
                double rate = 0;
        
                var srate = db.ratting.Where(m => m.recipe_id == item.id).Select(m => m.rate);
                if (srate.Count() > 0)
                {
                    rate = srate.Average();
                    item.rate = rate;
                }
                else item.rate = 0;
            }
            if (string.IsNullOrEmpty(name))
            {
                return View(model);
            }
            else
            {
                //var res = db.recipe.Where( e=>e.name.Equals(name)).ToList();
                var res = model.Where(e => e.name.ToLower().Contains(name.ToLower()) || e.name.ToUpper().Contains(name.ToUpper())).ToList();
                return View(res);
            }
        }

        // GET: Recipes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Recipes/Create
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult Create(Guid? id)
        {
           // ViewBag.manager = new SelectList(db.user, "id", "name");
            ViewBag.contestId = id;
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult Create(Recipe recipe, HttpPostedFileBase file, string contest_id)
        {
            var u = db.user.Find(Session["userId"]);
            //
            if (u.enabled == false)
            {
                return RedirectToAction("error", "home", new { msg = "your accoutn dont have permission to create recipe, contact admin for more infomation!" });
            }
            if (u.expireDate < DateTime.Now)
            {
                return RedirectToAction("error", "home", new { msg = "your accoutn has expired, please go to profile to buy vip access!" });
            }
            try
            {if(contest_id != null)
                recipe.contest_id =Guid.Parse(contest_id);
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        string ImageName = System.IO.Path.GetFileName(file.FileName);
                        string physicalPath = Server.MapPath("~/images/" + ImageName);

                        file.SaveAs(physicalPath);

                        //Recipe newRecord = new Recipe();
                        //newRecord.name = Request.Form["name"];
                        //newRecord.category = Request.Form["category"];
                        //newRecord.content = Request.Form["content"];
                        //newRecord.type = Request.Form["type"];
                        // recipe.contest_id = contest_id;
                        recipe.createAt = DateTime.Now.ToLocalTime();
                        recipe.manager = Guid.Parse(Session["userId"].ToString());
                        recipe.image = ImageName;
                        db.recipe.Add(recipe);
                        db.SaveChanges();
                        ModelState.AddModelError("", "Create recipe success");
                        return RedirectToAction("Index", "Recipes");
                    }
                    else
                    {
                        ModelState.AddModelError("", "plz select imgae");
                    }
                }
                else
                {
                    var errors = ModelState.SelectMany(m => m.Value.Errors).Select(e => e.ErrorMessage).ToList();
                    ModelState.AddModelError("", "Create fail!");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return View();
        }

        //        ([Bind(Include = "id,name,category,content,type,createAt,enabled,image,manager")] Recipe recipe)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                recipe.id = Guid.NewGuid();
        //                db.recipe.Add(recipe);
        //                db.SaveChanges();
        //                return RedirectToAction("Index");
        //    }

        //    ViewBag.manager = new SelectList(db.user, "id", "name", recipe.manager);
        //            return View(recipe);
        //}

        // GET: Recipes/Edit/5
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ViewBag.manager = new SelectList(db.user, "id", "name", recipe.manager);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult Edit(Recipe recipe, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string ImageName = System.IO.Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath("~/images/" + ImageName);

                    file.SaveAs(physicalPath);

                    db.Entry(recipe).State = EntityState.Modified;
                    //model.name = Request.Form["name"];
                    //model.category = Request.Form["category"];
                    //model.content = Request.Form["content"];
                    //model.type = Request.Form["type"];
                    recipe.image = ImageName;
                    db.SaveChanges();
                    return RedirectToAction("edit", "recipes");
                }
                else
                {
                    db.Entry(recipe).State = EntityState.Modified;
                    //model.name = Request.Form["name"];
                    //model.category = Request.Form["category"];
                    //model.content = Request.Form["content"];
                    //model.type = Request.Form["type"];

                    db.SaveChanges();
                    return RedirectToAction("recipe", "users");
                }

            }
            ViewBag.manager = new SelectList(db.user, "id", "name", recipe.manager);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(role = "ROLE_USER")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Recipe recipe = db.recipe.Find(id);
            db.recipe.Remove(recipe);
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

    }
}
