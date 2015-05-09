﻿// ****************************************************************************
// <copyright file="BusinessValidationException.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Common
{
    using System.Net;

    public class BusinessValidationException : FrontendException
    {
        public BusinessValidationException(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        {
        }
    }
}