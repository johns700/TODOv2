using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace TODO_Version_1.Models
{
    public class TodoDatabase : DbContext
    {
        public DbSet<MyUser> MyUsers { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}