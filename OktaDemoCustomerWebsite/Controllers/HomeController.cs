using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OktaDemoCustomerWebsite.Models;
using OktaDemoCustomerWebsite.Utils;

namespace OktaDemoCustomerWebsite.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index() {
            Login response = new Login();
            OktaSessionResponse oktaSession = (OktaSessionResponse)Session[RESTUtil.OKTA_SESSION];
            CheckSession(oktaSession, response);

            if(response.IsAuthenticated) {
                String oAuthToken = (String)Session[RESTUtil.OKTA_OIDC_TOKEN];
                if (oAuthToken != null) {
                    ViewBag.CurrentUser = RESTUtil.GetCurrentUser(oktaSession.UserId, oAuthToken);
                }
            }
            
            return View(response);
        }

        [HttpPost]
        public ActionResult Index(Login loginModel) {
            OktaSessionResponse sessionResponse = RESTUtil.GetSession(loginModel);
            CheckSession(sessionResponse, loginModel);

            if (loginModel.IsAuthenticated) {
                return Redirect("https://recinto.oktapreview.com/oauth2/aus9d9b7z6YAegbPh0h7/v1/authorize?response_type=code&client_id=4KsoT6lnBL9bOPqBFhMv&redirect_uri=http://localhost:59142/Home/AuthCode&scope=Read&state=af0ifjsldkj&nonce=n-0S6_WzA2Mj&sessionToken=" + sessionResponse.Id);
            } else {
                return View(loginModel);
            }

        }

        public ActionResult AuthCode() {
            String oidcCode = Request.QueryString["code"];
            Login response = new Login();
            OktaSessionResponse oktaSession = (OktaSessionResponse)Session[RESTUtil.OKTA_SESSION];
            CheckSession(oktaSession, response);

            String oAuthTokenUrl = "https://recinto.oktapreview.com/oauth2/aus9d9b7z6YAegbPh0h7/v1/token?grant_type=authorization_code&code=" + oidcCode + "&redirect_uri=http://localhost:59142/Home/AuthCode";

            ViewBag.OidcRedirectUrl = oAuthTokenUrl;

            OIDCTokenResponse tokenResponse = RESTUtil.GetOIDCToken(oAuthTokenUrl);
            if(tokenResponse != null) {
                if(tokenResponse.AccessToken != null) {
                    Session[RESTUtil.OKTA_OIDC_TOKEN] = tokenResponse.AccessToken;
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private void CheckSession(OktaSessionResponse sessionResponse, Login login) {
            if (sessionResponse != null) {
                if (sessionResponse.Id != null) {
                    login.IsAuthenticated = true;
                    this.Session[RESTUtil.OKTA_SESSION] = sessionResponse;
                }
            }
        }
    }
}