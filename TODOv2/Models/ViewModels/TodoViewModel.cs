using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODOv2.Models
{
    public class TodoViewModel
    {
        public TodoViewModel()
        {
            Items = new List<TodoItem>();
        }

        public  List<TodoItem> Items { get; set; }
    }
}