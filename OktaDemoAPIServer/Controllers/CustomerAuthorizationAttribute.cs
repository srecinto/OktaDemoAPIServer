using System;
using System.Web.Http;

using OktaDemoAPIServer.Models;
using OktaDemoAPIServer.Utils;

namespace OktaDemoAPIServer.Controllers {

    public class CustomerAuthorizationAttribute : AuthorizeAttribute {

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext) {
            
            if (AuthorizeRequest(actionContext)) {

                return;
            }

            HandleUnauthorizedRequest(actionContext);
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext) {

            //Code to handle unauthorized request
            var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
            throw new HttpResponseException(challengeMessage);
        }

        private bool AuthorizeRequest(System.Web.Http.Controllers.HttpActionContext actionContext) {
            //Write your code here to perform authorization
            //Validate Okta Session
            bool response = false;
            if(actionContext.Request.Headers.Authorization != null) {
                String oAuthToken = actionContext.Request.Headers.Authorization.Parameter;
                TokenIntrospectionResponse introspection = RESTUtil.IntrospectToken(oAuthToken);

                if (introspection != null) {
                    response = introspection.Active;
                }
            }
            
            return response;
        }

    }
}
