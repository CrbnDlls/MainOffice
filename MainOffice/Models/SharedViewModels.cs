using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainOffice.Models
{
    public class NotFoundViewModel
    {
        public string Title { get; set; }
        public string OriginalViewAction { get; set; }
        public string Message { get; set; }
    }
}