using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OktaDemoCustomerWebsite.Models;
using OktaDemoCustomerWebsite.Utils;

namespace OktaDemoCustomerWebsite.Controllers
{
    public class HomeController : Controller {
        private static readonly String COOKIES_TOKEN = "token";
        private static readonly String QUERY_STRING_CODE = "code";
        // GET: Home
        public ActionResult Index() {
            Login response = new Login();
            String oAuthToken = null;
            if (Request.Cookies[COOKIES_TOKEN] != null) {
                oAuthToken = Request.Cookies[COOKIES_TOKEN].Value;
            }

            if (!String.IsNullOrEmpty(oAuthToken)) {
                TokenIntrospectionResponse tokenResponse = RESTUtil.IntrospectToken(oAuthToken);
                if (tokenResponse != null) {
                    response.IsAuthenticated = true;
                    ViewBag.CurrentUser = RESTUtil.GetCurrentUser(tokenResponse.UID, oAuthToken);
                }
            }

            return View(response);
        }

        [HttpPost]
        public ActionResult Index(Login loginModel) {
            OktaSessionResponse sessionResponse = RESTUtil.GetSession(loginModel);

            if (sessionResponse != null) {
                return Redirect(RESTUtil.GetAuthorizationURL(sessionResponse.SessionToken));
            } else {
                return View(loginModel);
            }

        }

        public ActionResult AuthCode() {
            String oidcCode = Request.QueryString[QUERY_STRING_CODE];
            Login response = new Login();

            String oAuthTokenUrl = RESTUtil.GetTokenURL(oidcCode);

            ViewBag.OidcRedirectUrl = oAuthTokenUrl;

            OIDCTokenResponse tokenResponse = RESTUtil.GetOIDCToken(oAuthTokenUrl);
            if (tokenResponse != null) {
                if (tokenResponse.AccessToken != null) {
                    Response.Cookies.Add(new HttpCookie(COOKIES_TOKEN, tokenResponse.AccessToken));
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout() {
            if (Request.Cookies[COOKIES_TOKEN] != null) {
                RESTUtil.RevokeToken(Request.Cookies[COOKIES_TOKEN].Value);
            }
            Response.SetCookie(new HttpCookie(COOKIES_TOKEN, ""));
            return RedirectToAction("Index", "Home");
        }
    }
}