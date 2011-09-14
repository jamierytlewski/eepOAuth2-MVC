using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using eepURL_MVC.Models;

namespace eepURL_MVC.Controllers
{
    public class HomeController : Controller
    {

        Dictionary<string, string> _configuration = new Dictionary<string, string>(){
        {"client_id", "YourClientID"},
        {"client_secret", "YourSecret"},
        {"redirect_uri", "http://127.0.0.1:49660/redirect"},
        {"authorize_uri", "https://login.mailchimp.com/oauth2/authorize"},
        {"access_token_uri", "https://login.mailchimp.com/oauth2/token"},
        {"base_uri", "https://login.mailchimp.com/oauth2/"}};
        //
        // GET: /Home/
        public ActionResult Index()
        {
            eepURL eep = new eepURL(_configuration);
            return View(eep);
        }

        //
        // GET: /Home/ListActivity/{id}
        public ActionResult ListActivity(string id)
        {
            eepURL eep = new eepURL(_configuration);
            var session = eep.getSession();
            var rest_info = (eep.getMetaData());
            var api_key = session + "-" + rest_info.dc;
            var api = new MCAPI(api_key);
            var serializer = new JavaScriptSerializer();
            var listActivity = serializer.Deserialize<List<ListActivity>>(api.listActivity(id));
            return View(listActivity);
        }

        //
        // GET: /Home/Redirect/{id}
        public ActionResult Redirect()
        {
            if (string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                RedirectToAction("Index");
            }
            _configuration.Add("code", Request.QueryString["code"]);
            eepURL eep = new eepURL(_configuration);
            var session = eep.getSession();
            var rest_info = (eep.getMetaData());
            var api_key = session + "-" + rest_info.dc;
            var api = new MCAPI(api_key);
            var serializer = new JavaScriptSerializer();
            var r = api.lists("", "0", "5");
            var error = serializer.Deserialize<Errors>(r);
            if (error.code != null)
            {
                return View("MC_Error", error);
            }
            var lists = serializer.Deserialize<Lists>(r);
            return View(lists);
        }

    }
}
