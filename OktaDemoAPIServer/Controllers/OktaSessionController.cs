using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using OktaDemoAPIServer.Models;
using OktaDemoAPIServer.Utils;

namespace OktaDemoAPIServer.Controllers
{
    public class OktaSessionController : ApiController
    {
        public OktaSessionResponse Post([FromBody]Authentication authentication) {

            return RESTUtil.GetSession(authentication);
        }
    }
}
