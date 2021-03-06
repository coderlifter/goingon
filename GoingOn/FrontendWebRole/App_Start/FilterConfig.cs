﻿// ****************************************************************************
// <copyright file="FilterConfig.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Filter config class
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole
{
    using System.CodeDom.Compiler;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("ASP.NET", "Visual Studio 2013")]
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
