using System.Collections;
using System.DirectoryServices;
using System.Web;
using System.Web.Security;
using System.Web.Services.Description;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using Flock.DTO;
using Flock.DataAccess.EntityFramework;
using Flock.Facade;
using Flock.Facade.Interfaces;
using Flock.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http;

namespace Flock.Controllers
{
    [RoutePrefix("/api/quack")]
    public class QuackController : ApiController
    {
        private readonly IQuackFacade _quackFacade;

        public QuackController(IQuackFacade quackFacade)
        {
            _quackFacade = quackFacade;
        }

        [POST("save")]
        public HttpResponseMessage Post(Quack quack)
        {
            int createdQuackId = _quackFacade.SaveQuack(quack);
            return Request.CreateResponse(HttpStatusCode.Created, createdQuackId);
        }

        [PUT("updateQuack")]
        public HttpResponseMessage Put(int quackId, int status)
        {
            if (status == 0)
            {
                _quackFacade.DeleteQuack(quackId);
                return Request.CreateResponse(HttpStatusCode.Created, true);    
            }
            else if (status == 1)
            {
                _quackFacade.ActivateQuack(quackId);
                return Request.CreateResponse(HttpStatusCode.Created, true);    
            }
            return Request.CreateResponse(HttpStatusCode.Created, false);
        }
        
        [GET("activeQuacks")]
        public QuacksList GetAllActiveQuacks(int quackCount)
        {
           return _quackFacade.GetAllQuacks(quackCount);
        }

        [GET("quacksByHashtag")]
        public IList<QuackDto> GetAllQuacksByHashtag(string hashtag)
        {
            return _quackFacade.GetAllQuacksbyHashtag(hashtag);
        }

        [POST("likeOrUnlikeQuack")]
        public HttpResponseMessage Post(int quackId, int userId, bool isLike)
        {
            _quackFacade.LikeOrUnlikeQuack(quackId, userId, isLike);
            return Request.CreateResponse(HttpStatusCode.Created, true);
        }


        [GET("getQuackInfo")]
        public IList<QuackDto> GetQuackInfo(int conversationId)
        {
            return _quackFacade.GetQuacksInfo(conversationId );
        }

        [GET("getQuackByFirstAndLastName")]
        public IList<QuackDto> GetQuackByFirstAndLastName(string firstName, string lastName)
        {
            #region Verify if name information is correct
            if (lastName == null)
            {
                lastName = "";
            }
            else if (lastName.Equals("undefined"))
            {
                lastName = "";
            }

            if (firstName == null)
            {
                firstName = "";
            }
            else if (firstName.Equals("undefined"))
            {
                firstName = "";
            }
            #endregion
            return _quackFacade.GetQuacksByLastNameAndFirstName(lastName, firstName);
        }

    }
}
