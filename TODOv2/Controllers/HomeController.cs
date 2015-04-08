using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TODOv2.Models;

namespace TODOv2.Controllers
{
    public class HomeController : Controller
    {

        // Get
        public ActionResult Index()
        {
            if (Session["UserJustLoggedIn"] == null)
            {
                Session["UserJustLoggedIn"] = false;
            }
            if (Session["UserJustLoggedOut"] == null)
            {
                Session["UserJustLoggedOut"] = false;
            }

            if ((bool)Session["UserJustLoggedIn"] == true)
            {
                // Add the current TODO list to their list
                AddItemsToDatabase();
                PopulateListFromDatabase();

                // After a local copy of the user's todo list has been created we can delete
                // all item records in the database & just work with the local copy. When
                // the user logs out the local copy will be added back into the database. AddItemsToDatabase()
                DeleteListFromDatabase(); 
 
                Session["UserJustLoggedIn"] = false;
            }

            if ((bool)Session["UserJustLoggedOut"] == true)
            {
                AddItemsToDatabase();
                Session["UserJustLoggedOut"] = false;
            }

            // Grab the view model
            TodoViewModel model = new TodoViewModel();
            if (Session["TODO"] != null)
            {
                model = (TodoViewModel)Session["TODO"];
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(TodoViewModel model, string TodoItem)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            // Add new items to list
            if (TodoItem != "")
            {
                model.Items.Add(new TodoItem { Task = TodoItem, Complete = false });
            }

            // Remove items from local list
            model.Items.RemoveAll(item => item.Complete == true);

            // Save list into Session state until we add it to the database
            Session["TODO"] = model;
            Session["Update"] = true;

            // Copy values to new model & clear the viewmodel
            // This is done because there were issues with the ViewModel retaining its state between requests
            TodoViewModel newModel = model;
            ModelState.Clear();

            return View(newModel);
        }

        private void AddItemsToDatabase()
        {
            // Get references
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Find the user
            ApplicationUser user = userManager.FindById(User.Identity.GetUserId());

            // Get the current TODO list
            TodoViewModel sessionModel = new TodoViewModel();
            if (Session["TODO"] != null)
            {
                sessionModel = (TodoViewModel)Session["TODO"];
            }

            // Add todo list to database
            foreach (TodoItem item in sessionModel.Items)
            {
                user.Items.Add(item);
            }
            context.SaveChanges();
        }

        private void PopulateListFromDatabase()
        {
            // Get reference
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            // Clear the list stored in the session
            Session.Remove("TODO");

            // Re-poplulate the list grabbing all items (old & new)
            string userID = User.Identity.GetUserId();
            TodoViewModel newModel = new TodoViewModel { Items = context.Users.Find(userID).Items.ToList() };

            // Create new list in Session
            Session["TODO"] = newModel;
        }

        private void DeleteListFromDatabase()
        {
            // Get references
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Clear all entries from the current user
            ApplicationUser user = userManager.FindById(User.Identity.GetUserId());

            //context.TodoItems.
            //// Get the record that needs to be deleted
            //var x = (from n in context.TodoItems
            //         where n. == userID
            //         select n).First();

            // This method doesn't work - It just removes the ApplicationUserID from the TodoItems table
            context.Users.Find(User.Identity.GetUserId()).Items.Clear();
            context.SaveChanges();
        }

            /*
        [HttpPost]
        public ActionResult Index(TodoViewModel model, string TodoItem)
        {
            // Get references
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Find the user
            ApplicationUser user = userManager.FindById(User.Identity.GetUserId());

            // If the user has typed a todo item
            if (TodoItem != "")
            {
                // Add todo
                if (user != null)
                {
                    user.Items.Add(new TodoItem { Task = TodoItem, Complete = false });
                    context.SaveChanges();
                }
            }

            // Get current user's ID
            string userID = User.Identity.GetUserId();

            // If the user has deleted any flag them as complete in the database
            foreach (TodoItem item in model.Items)
            {
                if (item.Complete == true)
                {
                    // Get the record that needs to be deleted
                    var x = (from n in context.TodoItems
                             where n.ID == item.ID
                             select n).First();

                    context.TodoItems.Remove(x);
                    context.SaveChanges();
                }  
            }

            ModelState.Clear();
            // Retrieve the user's todo list

            model = new TodoViewModel { Items = context.Users.Find(userID).Items.ToList() };

            return View(model);
        }
             * */
    }
}