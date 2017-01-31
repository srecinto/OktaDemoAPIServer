using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Web;

using OktaDemoCustomerWebsite.Models;

namespace OktaDemoCustomerWebsite.Utils {
    public class RESTUtil {
        private static String apiUrlBase;
        private static String oktaOrg;
        private static String oktaOAuthHeaderAuth;
        private static String oktaOAuthIssuerId;
        private static String oktaOAuthClientId;
        private static String oktaOAuthRedirectUri;
        private static HttpClient client = new HttpClient();

        static RESTUtil() {
            apiUrlBase = WebConfigurationManager.AppSettings["apiBaseUri"];
            oktaOrg = WebConfigurationManager.AppSettings["okta:Org"];
            oktaOAuthIssuerId = WebConfigurationManager.AppSettings["okta:OAuthIssuerId"];
            oktaOAuthClientId = WebConfigurationManager.AppSettings["okta:OAuthClientId"];
            oktaOAuthRedirectUri = WebConfigurationManager.AppSettings["okta:OAuthRedirectUri"];
            String oktaOAuthSecret = WebConfigurationManager.AppSettings["okta:OAuthSecret"];

            oktaOAuthHeaderAuth = Base64Encode(String.Format("{0}:{1}", oktaOAuthClientId, oktaOAuthSecret));
        }

        
        public static OktaSessionResponse GetSession(Login login) {
            OktaSessionResponse sessResponse = GetObjectFromAPI<OktaSessionResponse>(HttpMethod.Post, apiUrlBase + "/api/oktasession/", login, null);
            return sessResponse;
        }

        public static OIDCTokenResponse GetOIDCToken(String url) {
            OIDCTokenResponse tokenResponse = GetObjectFromAPI<OIDCTokenResponse>(HttpMethod.Post, url, oktaOAuthHeaderAuth);
            return tokenResponse;
        }

        public static Customer GetCurrentUser(String customerId, String oAuthToken) {
            Customer result = null;
            result = GetObjectFromAPI<Customer>(HttpMethod.Get, String.Format("{0}/api/customer/{1}", apiUrlBase, customerId), oAuthToken);

            return result;
        }

        public static String GetAuthorizationURL(String oktaSessionId) {

            return String.Format("https://{0}/oauth2/{1}/v1/authorize?response_type=code&client_id={2}&redirect_uri={3}/Home/AuthCode&scope=Read&state=af0ifjsldkj&nonce=n-0S6_WzA2Mj&sessionToken={4}",
                                oktaOrg,
                                oktaOAuthIssuerId,
                                oktaOAuthClientId,
                                oktaOAuthRedirectUri,
                                oktaSessionId);
        }

        public static String GetTokenURL(String oidcCode) {
            return String.Format("https://{0}/oauth2/{1}/v1/token?grant_type=authorization_code&code={2}&redirect_uri={3}/Home/AuthCode",
                                oktaOrg,
                                oktaOAuthIssuerId,
                                oidcCode,
                                oktaOAuthRedirectUri);
        }

        public static TokenIntrospectionResponse IntrospectToken(String token) {
            TokenIntrospectionResponse introspectionResponse = GetObjectFromAPI<TokenIntrospectionResponse>(
                HttpMethod.Post,
                String.Format("https://{0}/oauth2/{1}/v1/introspect?token={2}&token_type_hint=access_token",
                              oktaOrg,
                              oktaOAuthIssuerId,
                              token),
                oktaOAuthHeaderAuth);
            return introspectionResponse;
        }

        public static void RevokeToken(String token) {
            GetObjectFromAPI<Object>(
                HttpMethod.Post,
                String.Format("https://{0}/oauth2/{1}/v1/revoke?token={2}&token_type_hint=access_token",
                              oktaOrg,
                              oktaOAuthIssuerId,
                              token),
                oktaOAuthHeaderAuth);
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri, object model, String authHeader) {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(uri);
            request.Method = method;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if(authHeader != null) {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            }

            if (model != null) {
                String json = null;

                using (MemoryStream ms = new MemoryStream()) {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(model.GetType());
                    serializer.WriteObject(ms, model);
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    json = sr.ReadToEnd();
                }

                request.Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );
            } else {
                if(method == HttpMethod.Post || method == HttpMethod.Put) {
                    request.Content = new StringContent(
                        "",
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded"
                    );
                }
                
            }

            return request;
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri, String authHeader) {
            return CreateBaseRequest(method, uri, null, authHeader);
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri) {
            return CreateBaseRequest(method, uri, null, null);
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri, object model) {
            return CreateBaseRequest(method, uri, model, null);
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, String uri, object model, String authHeader) {
            T result = default(T);
            HttpRequestMessage request = CreateBaseRequest(method, uri, model, authHeader);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode) {
                if (response.Content.Headers.ContentType != null) {
                    if ("text/html".Equals(response.Content.Headers.ContentType.MediaType)) {
                        var results = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(results);
                    } else {
                        result = response.Content.ReadAsAsync<T>().Result;
                    }
                }
            } else {
                var results = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(results);
            }

            return result;
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, String uri, String authHeader) {
            return GetObjectFromAPI<T>(method, uri, null, authHeader);
        }

        private static T[] GetObjectsFromAPI<T>(HttpMethod method, String uri, String authHeader) {
            T[] results = { };
            HttpRequestMessage request = CreateBaseRequest(method, uri, authHeader);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode) {
                var apiObjects = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                results = apiObjects.ToArray<T>();
            }

            return results;
        }

        public static String Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static String GetOAuthTokenFromHeader(HttpRequestBase request) {
            String rawToken = (String)request.Headers["Authorization"];
            return GetTokenFromRawString(rawToken);
        }

        public static String GetOAuthTokenFromHeader(HttpResponseBase response) {
            String rawToken = (String)response.Headers["Authorization"];
            return GetTokenFromRawString(rawToken);
        }

        private static String GetTokenFromRawString(String rawToken) {
            String result = null;
            if (!String.IsNullOrEmpty(rawToken)) {
                String[] splitToken = { " " };
                String[] resultParts = rawToken.Split(splitToken, StringSplitOptions.RemoveEmptyEntries);
                if (resultParts.Length == 2) {
                    result = resultParts[1];
                }
            }

            return result;
        }

    }
}