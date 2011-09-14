using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eepURL_MVC.Models
{
    public class Lists
    {
        public int total {get;set;}
        public List<ListData> data {get;set;}
    }

    public class ListData
    {
        public string id { get; set; }
        public int web_id { get; set; }
        public string name { get; set; }
        public string date_created { get; set; }
    }

    public class ListActivity
    {
        public string day { get; set; }
        public int emails_sent { get; set; }
        public int unique_opens { get; set; }
        public int recipient_clicks { get; set; }
        public int hard_bounce { get; set; }
        public int soft_bounce { get; set; }
        public int abuse_reports { get; set; }
        public int subs { get; set; }
        public int unsubs { get; set; }
        public int other_adds { get; set; }
        public int other_removes { get; set; }
    }
}