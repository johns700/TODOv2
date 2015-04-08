using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODO_Version_1.Models
{
    public class MyUser
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Passwork { get; set; }
        List<TodoItem> TodoItems { get; set; }
    }
}