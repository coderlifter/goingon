﻿// ****************************************************************************
// <copyright file="RouteConfig.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Frontend
{
    [ExcludeFromCodeCoverage]
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var configuration = new HttpConfiguration();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: null
            );
        }
    }
}