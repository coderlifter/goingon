﻿// ****************************************************************************
// <copyright file="ApiValidationChecks.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Input validation
// </summary>
// ****************************************************************************

namespace GoingOn.Frontend.Validation
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using GoingOn.Common;
    using GoingOn.Frontend.Common;
    using Microsoft.Ajax.Utilities;

    using GoingOn.Frontend.Entities;

    public class ApiInputValidationChecks : IApiInputValidationChecks
    {
        private readonly IApiInputValidationChecks inputValidation;

        public ApiInputValidationChecks()
        {
        }

        public ApiInputValidationChecks(IApiInputValidationChecks inputValidation)
        {
            this.inputValidation = inputValidation;
        }

        public bool IsValidUser(User user)
        {
            return 
                user != null &&
                this.inputValidation.IsValidNickName(user.Nickname) &&
                this.inputValidation.IsValidPassword(user.Password) &&
                this.inputValidation.IsValidName(user.Name) &&
                this.inputValidation.IsValidEmail(user.Email) &&
                ((user.BirthDate == null) || this.inputValidation.IsValidBirthDate((DateTime)user.BirthDate));
        }

        public bool IsValidNickName(string nickName)
        {
            return !string.IsNullOrWhiteSpace(nickName);
        }

        public bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password);
        }

        public bool IsValidCity(string cityString)
        {
            if (!string.IsNullOrWhiteSpace(cityString))
            {
                City city;
                
                return Enum.TryParse(cityString, out city);
            }

            return false;
        }

        public bool IsValidName(string name)
        {
            return name == null || !name.IsNullOrWhiteSpace();
        }

        /// <summary>
        /// Validates an e-mail address according to the example in MSDN
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx"/>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", this.DomainMapper,
                                        RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(email,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public bool IsValidBirthDate(DateTime birthDate)
        {
            return true;
        }

        public bool IsValidNews(News news)
        {
            return
                news != null &&
                news.Title != null && this.IsValidTitle(news.Title) &&
                news.Content != null && this.IsValidContent(news.Content);
        }

        public bool IsValidVote(Vote vote)
        {
            // TODO: add tests
            return vote.Value > 0 && vote.Value <= 5;
        }

        public bool IsValidNewsId(string id)
        {
            Guid guid;
            return Guid.TryParse(id, out guid);
        }

        public bool IsValidNewsDate(string dateString)
        {
            DateTime date;

            return DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        public void ValidateDiaryEntryParameters(string city, string date)
        {
            if (!this.inputValidation.IsValidCity(city))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, string.Format("'{0}' is not a valid city.", city));
            }

            if (!this.inputValidation.IsValidNewsDate(date))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, string.Format("'{0}' is not a valid date.", date));
            }
        }

        public void ValidateNewsParameters(string city, string date, string newsId)
        {
            this.ValidateDiaryEntryParameters(city, date);

            if (!this.inputValidation.IsValidNewsId(newsId))
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, string.Format("'{0}' is not a valid identifier for news.", newsId));
            }
        }

        public void ValidateImage(byte[] imageBytes, MediaTypeHeaderValue contentType)
        {
            try
            {
                ImageFormat format = MediaTypeHelper.ConvertToImageFormat(contentType);

                MemoryStream memoryStream = new MemoryStream(imageBytes);
                var image = Image.FromStream(memoryStream);

                if (!format.Equals(image.RawFormat))
                {
                    throw new ArgumentException("The image format does not match the content type.");
                }
            }
            catch (ArgumentException formatException)
            {
                throw new InputValidationException(HttpStatusCode.BadRequest, formatException.Message);
            }
        }

        public bool IsValidTitle(string title)
        {
            return !string.IsNullOrWhiteSpace(title);
        }

        public bool IsValidContent(string content)
        {
            return !string.IsNullOrWhiteSpace(content);
        }

        #region Helper methods

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;

            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
            }

            return match.Groups[1].Value + domainName;
        }

        #endregion
    }
}