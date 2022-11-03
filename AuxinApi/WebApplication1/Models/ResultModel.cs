using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class ResultModel
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public String ErrorMessage { get; set; }
        public String TechDetails { get; set; }
    }
}