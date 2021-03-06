﻿// ****************************************************************************
// <copyright file="WebApiApplication.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Web service global class
// </summary>
// ****************************************************************************

using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GoingOn.FrontendWebRole
{
    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}