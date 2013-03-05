using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAvail.MVC.Bootstrap.Models
{
    public interface INamesContainer
    {
        string FullName { get; set; }

        string ShortName { get; set; }
    }
}