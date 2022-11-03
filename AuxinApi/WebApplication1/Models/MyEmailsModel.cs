using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class MyEmailsModel
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String EmailId { get; set; }
        public String Message { get; set; }
        public bool? IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}