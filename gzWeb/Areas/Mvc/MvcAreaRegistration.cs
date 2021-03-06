﻿using System.Web.Mvc;

namespace gzWeb.Areas.Mvc
{
    public class MvcAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Mvc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            context.MapRoute(
                    "MvcRoute",
                    "Mvc/{controller}/{action}/{id}",
                    new {action = "Index", id = UrlParameter.Optional},
                    new[] { "gzWeb.Areas.Mvc.Controllers" });
        }
    }
}