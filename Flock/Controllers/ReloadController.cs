using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using Flock.DTO;
using Flock.Facade.Interfaces;

namespace Flock.Controllers
{
    [RoutePrefix("api/Reload")]
    public class ReloadController : ApiController
    {
        private readonly IQuackFacade _quackFacade;

        public ReloadController(IQuackFacade quackFacade)
        {
            _quackFacade = quackFacade;
        }
        [GET("")]
        public QuackDto GetUpdatedQuack(int id)
        {
            return _quackFacade.ReloadQuack(id);
        }
    }
}
