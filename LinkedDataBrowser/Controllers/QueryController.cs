using LinkedDataBrowser.Models.DataModels;
using LinkedDataBrowser.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LinkedDataBrowser.Controllers
{
    public class QueryController : Controller
    {
        public ActionResult AsQuery()
        {
            return View();
        }
                
        public ActionResult AsSubjectNoSparql()
        {
            return View();
        }
                
        public ActionResult AsSubjectOnlyFive()
        {
            return View();
        }

        public ActionResult AsSubjectMultipleEndpoints()
        {
            return View();
        }
    }
}