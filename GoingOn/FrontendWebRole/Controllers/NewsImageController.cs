﻿// ****************************************************************************
// <copyright file="NewsImageController.cs" company="Universidad de Malaga">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Alberto Guerra Gonzalez</author>
// <summary>
// Controller for News Image
// </summary>
// ****************************************************************************

namespace GoingOn.FrontendWebRole.Controllers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GoingOn.Common;
    using GoingOn.Frontend.Authentication;
    using GoingOn.Frontend.Common;
    using GoingOn.Frontend.Validation;
    using GoingOn.Repository;

    public class NewsImageController : GoingOnApiController
    {
        private readonly INewsRepository newsRepository;
        private readonly IImageRepository imageRepository;
        private readonly IApiInputValidationChecks inputValidation;
        private readonly IApiBusinessLogicValidationChecks businessValidation;

        public NewsImageController(INewsRepository newsRepository, IImageRepository imageRepository, IApiInputValidationChecks inputValidation, IApiBusinessLogicValidationChecks businessValidation)
        {
            this.newsRepository = newsRepository;
            this.imageRepository = imageRepository;
            this.inputValidation = inputValidation;
            this.businessValidation = businessValidation;
        }

        /// <summary>
        /// Get the image of the news.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsImageTemplate)]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteGetAsync, city, date, newsId);
        }

        /// <summary>
        /// Creates the image of the news.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsImageTemplate)]
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecutePostAsync, city, date, newsId, this.User.Identity.Name);
        }

        /// <summary>
        /// Deletes the image of the news.
        /// </summary>
        /// <param name="city">The city of the news.</param>
        /// <param name="date">The date when the news was published.</param>
        /// <param name="newsId">The identifier of the news.</param>
        /// <returns></returns>
        [Route(GOUriBuilder.NewsImageTemplate)]
        [IdentityBasicAuthentication]
        [Authorize]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string city, string date, string newsId)
        {
            return await this.ValidateExecute(this.ExecuteDeletetAsync, city, date, newsId, this.User.Identity.Name);
        }

        #region Operations code

        private async Task<HttpResponseMessage> ExecuteGetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];

            await this.ValidateGetOperation(city, date, newsId);

            Image image = await this.imageRepository.GetNewsImage(city, DateTime.Parse(date), Guid.Parse(newsId));

            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StreamContent(memoryStream);
            response.Content.Headers.ContentType = MediaTypeHelper.ConvertFromImageFormat(image.RawFormat);

            return response;
        }

        private async Task<HttpResponseMessage> ExecutePostAsync(params object[] parameters)
        {
            var city = (string) parameters[0];
            var date = (string) parameters[1];
            var newsId = (string) parameters[2];
            var authenticatedUser = (string)parameters[3];

            byte[] imageBytes = await this.Request.Content.ReadAsByteArrayAsync();
            MediaTypeHeaderValue contentType = this.Request.Content.Headers.ContentType;

            await this.ValidatePostOperation(city, date, newsId, imageBytes, contentType, authenticatedUser);

            var memoryStream = new MemoryStream(imageBytes);

            Image image = ImageHelper.CreateFromStream(memoryStream);

            await this.imageRepository.CreateNewsImage(city, DateTime.Parse(date), Guid.Parse(newsId), image);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteDeletetAsync(params object[] parameters)
        {
            var city = (string)parameters[0];
            var date = (string)parameters[1];
            var newsId = (string)parameters[2];
            var authenticatedUser = (string)parameters[3];

            await this.ValidateDeleteOperation(city, date, newsId, authenticatedUser);

            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        #endregion

        #region Validation helpers

        private async Task ValidateGetOperation(string city, string date, string id)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (!(await this.businessValidation.IsValidGetImageNews(this.imageRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The image news is not in the database");
            }
        }

        private async Task ValidatePostOperation(string city, string date, string id, byte[] imageBytes, MediaTypeHeaderValue contentType, string authenticatedUser)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            this.inputValidation.ValidateImage(imageBytes, contentType);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (!(await this.businessValidation.IsValidModifyNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id), authenticatedUser)))
            {
                throw new BusinessValidationException(HttpStatusCode.Unauthorized, "The user is not authorized to create the image");
            }

            if (await this.businessValidation.IsValidGetImageNews(this.imageRepository, city, DateTime.Parse(date), Guid.Parse(id)))
            {
                throw new BusinessValidationException(HttpStatusCode.BadRequest, "The image news already exists.");
            }
        }

        private async Task ValidateDeleteOperation(string city, string date, string id, string authenticatedUser)
        {
            this.inputValidation.ValidateNewsParameters(city, date, id);

            if (!(await this.businessValidation.IsValidGetNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The news is not in the database");
            }

            if (!(await this.businessValidation.IsValidModifyNews(this.newsRepository, city, DateTime.Parse(date), Guid.Parse(id), authenticatedUser)))
            {
                throw new BusinessValidationException(HttpStatusCode.Unauthorized, "The user is not authorized to delete the image");
            }

            if (!(await this.businessValidation.IsValidGetImageNews(this.imageRepository, city, DateTime.Parse(date), Guid.Parse(id))))
            {
                throw new BusinessValidationException(HttpStatusCode.NotFound, "The image news is not in the database");
            }
        }

        #endregion
    }
}