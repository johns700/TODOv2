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
            // Keep track of whether a user has just logged in or out
            if (Session["UserJustLoggedIn"] == null)
            {
                Session["UserJustLoggedIn"] = false;
            }
            if (Session["UserJustLoggedOut"] == null)
            {
                Session["UserJustLoggedOut"] = false;
            }

            // User just logged in
            if ((bool)Session["UserJustLoggedIn"] == true)
            {
                // Add the current TODO list to their list
                AddItemsToDatabase();
                PopulateListFromDatabase();

                Session["UserJustLoggedIn"] = false;
            }

            // User just logged out
            if ((bool)Session["UserJustLoggedOut"] == true)
            {
                Session.Remove("TODO");
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

            // If a user is logged in, make changes to the database aswell as the local copy
            bool bUserLoggedIn = User.Identity.IsAuthenticated;

            // Are there any items that need to be deleted?
            if (bUserLoggedIn)
            {
                DeleteItemsFromDatabase(model);
            }
            
            // Remove completed items from local list
            model.Items.RemoveAll(item => item.Complete == true);

            // Add new items to list
            TodoItem newItem = new TodoItem { Task = TodoItem, Complete = false };
            if (TodoItem != "")
            {
                model.Items.Add(newItem);
            }

            // Save local copy of the list
            Session["TODO"] = model;

            if (bUserLoggedIn && TodoItem != "")
            { 
                AddItemToDatabase(newItem);
            }

            // Copy values to new model & clear the viewmodel
            // This is done because there were issues with the ViewModel retaining its state between requests
            TodoViewModel newModel = model;
            ModelState.Clear();

            return View(newModel);
        }

        private void AddItemToDatabase(TodoItem item)
        {
            // Get references
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            ApplicationUser user = userManager.FindById(User.Identity.GetUserId());

            user.Items.Add(item);
            context.SaveChanges();
        }

        private void AddItemsToDatabase()
        {
            // Get references
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Get the current TODO list
            TodoViewModel sessionModel = new TodoViewModel();
            if (Session["TODO"] != null)
            {
                sessionModel = (TodoViewModel)Session["TODO"];
            }

            ApplicationUser user = userManager.FindById(User.Identity.GetUserId());

            // Add todo list to database
            foreach (TodoItem item in sessionModel.Items)
            {
                user.Items.Add(item);
            }
            context.SaveChanges();
        }

        private void DeleteItemsFromDatabase(TodoViewModel model)
        {
            // Get references
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            foreach (TodoItem item in model.Items)
            {
                if (item.Complete == true)
                {
                    // Get the record that needs to be deleted
                    var x = (from n in context.TodoItems
                             where n.ID == item.ID
                             select n).First();

                    context.TodoItems.Remove(x);
                }
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

            Session["TODO"] = newModel;
        }

    }
}