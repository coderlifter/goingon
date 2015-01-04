﻿// ****************************************************************************
// <copyright file="IdentityBasicAuthenticationAttribute.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// TODO: write a summary
// </summary>
// ****************************************************************************

using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Frontend.Entities;
using GoingOn.Authentication;
using MemoryStorage;

namespace Frontend.Authentication
{
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string nickname, string password, CancellationToken cancellationToken)
        {
            IUserStorage storage = UserMemoryStorage.GetInstance();

            var containsUserTask = await storage.ContainsUser(User.ToUserBll(new User(nickname, password)));

            if (containsUserTask)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return new GenericPrincipal(new GenericIdentity(nickname), null);
            }

            // No user with userName/password exists.
            return null;
        }
    }
}