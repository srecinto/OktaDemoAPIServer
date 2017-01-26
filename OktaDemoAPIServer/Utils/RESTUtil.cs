using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;

using OktaDemoAPIServer.Models;

namespace OktaDemoAPIServer.Utils {
    public class RESTUtil {
        public static readonly String OKTA_SESSION = "OKTA_SESSION";

        private static String oktaOrg;
        private static String oktaApiKey;
        private static String oktaOAuthIssuerId;
        private static String oktaOAuthClientId;
        private static String oktaOAuthHeaderAuth;
        private static HttpClient client = new HttpClient();

        static RESTUtil() {
            oktaOrg = WebConfigurationManager.AppSettings["okta:Org"];
            oktaApiKey = WebConfigurationManager.AppSettings["okta:ApiKey"];
            oktaOAuthIssuerId = WebConfigurationManager.AppSettings["okta:OAuthIssuerId"];
            oktaOAuthClientId = WebConfigurationManager.AppSettings["okta:OAuthClientId"];

            String oktaOAuthSecret = WebConfigurationManager.AppSettings["okta:OAuthSecret"];

            oktaOAuthHeaderAuth = Base64Encode(String.Format("{0}:{1}", oktaOAuthClientId, oktaOAuthSecret));

        }

        /// <summary>
        /// This method will return all customers from Okta via API call
        /// TODO: Need to add behaviors like paging and filters
        /// </summary>
        /// <returns>Array of Customer objects</returns>
        public static Customer[] GetAllCustomers() {
            return GetObjectsFromAPI<Customer>(HttpMethod.Get, 
                String.Format("https://{0}/api/v1/users?filter=status eq \"ACTIVE\"&limit=100", oktaOrg));
        }

        public static Customer GetCustomerById(String customerId) {
            return GetObjectFromAPI<Customer>(HttpMethod.Get, 
                String.Format("https://{0}/api/v1/users/{1}", oktaOrg, customerId));
        }

        public static Customer AddNewCustomer(Customer newCustomer) {
            Customer customer = GetObjectFromAPI<Customer>(HttpMethod.Post, 
                String.Format("https://{0}/api/v1/users?activate=true", oktaOrg), newCustomer);

            return customer;
        }

        public static Customer UpdateCustomer(String Id, Customer customer) {
            //NOTE: a partial update from Okta requires a POST not a Put... a full update would require a Put
            Customer result = GetObjectFromAPI<Customer>(HttpMethod.Post, 
                String.Format("https://{0}/api/v1/users/{1}", oktaOrg, Id), customer);

            return result;
        }

        public static OktaSessionResponse GetSession(Authentication authentication) {
            AuthenticationResponse authResponse = GetObjectFromAPI<AuthenticationResponse>(HttpMethod.Post, String.Format("https://{0}/api/v1/authn", oktaOrg), authentication);
            OktaSessionResponse sess = GetObjectFromAPI<OktaSessionResponse>(HttpMethod.Post, 
                String.Format("https://{0}/api/v1/sessions?additionalFields=cookieToken", oktaOrg), 
                authResponse);

            return sess;
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

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri, object model, String authHeader) {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(uri);
            request.Method = method;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if(authHeader != null) {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            } else {
                request.Headers.Authorization = new AuthenticationHeaderValue("SSWS", oktaApiKey);
            }
            

            if(model != null) {
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
                if (method == HttpMethod.Post || method == HttpMethod.Put) {
                    request.Content = new StringContent(
                        "",
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded"
                    );
                }
            }

            return request;
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri) {
            return CreateBaseRequest(method, uri, null, null);
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri, String authHeader) {
            return CreateBaseRequest(method, uri, null, authHeader);
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, String uri, object model) {
            return GetObjectFromAPI<T>(method, uri, model, null);
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, String uri, String authHeader) {
            return GetObjectFromAPI<T>(method, uri, null, authHeader);
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, String uri, object model, String authHeader) {
            T result = default(T);
            HttpRequestMessage request = CreateBaseRequest(method, uri, model, authHeader);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode) {
                if("text/html".Equals(response.Content.Headers.ContentType.MediaType)) {
                    var results = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(results);
                } else {
                    result = response.Content.ReadAsAsync<T>().Result;
                }
            }

            return result;
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, String uri) {
            return GetObjectFromAPI<T>(method, uri, null);
        }

        private static T[] GetObjectsFromAPI<T>(HttpMethod method, String uri) {
            T[] results = { };
            HttpRequestMessage request = CreateBaseRequest(method, uri);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode) {
                var apiObjects = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                results = apiObjects.ToArray<T>();
            }

            return results;
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}