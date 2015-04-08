using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODOv2.Models
{
    public class TodoItem
    {
        public int ID { get; set; }
        public string Task { get; set; }
        public bool Complete { get; set; }

        // It says that this field isn't necessary, but helpful. Maybe to use the relationship both ways?
        // public int UserID { get; set; }
    }
}