using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;

using OktaDemoAPIServer.Models;

namespace OktaDemoAPIServer.Utils {
    public class RESTUtil {
        private static String oktaOrg;
        private static String oktaApiKey;
        private static HttpClient client = new HttpClient();

        static RESTUtil() {
            oktaOrg = WebConfigurationManager.AppSettings["okta:Org"];
            oktaApiKey = WebConfigurationManager.AppSettings["okta:ApiKey"];
        }

        /// <summary>
        /// This method will return all customers from Okta via API call
        /// TODO: Need to add behaviors like paging and filters
        /// </summary>
        /// <returns>Array of Customer objects</returns>
        public static Customer[] GetAllCustomers() {
            return GetObjectsFromAPI<Customer>(HttpMethod.Get, "https://" + oktaOrg + "/api/v1/users?filter=status eq \"ACTIVE\"&limit=100");
        }

        public static Customer GetCustomerById(String customerId) {
            return GetObjectFromAPI<Customer>(HttpMethod.Get, "https://" + oktaOrg + "/api/v1/users/" + customerId);
        }

        public static Customer AddNewCustomer(Customer newCustomer) {
            Customer customer = GetObjectFromAPI<Customer>(HttpMethod.Post, "https://" + oktaOrg + "/api/v1/users?activate=true", newCustomer);

            return customer;
        }

        public static Customer UpdateCustomer(String Id, Customer customer) {
            //NOTE: a partial update from Okta requires a POST not a Put... a full update would require a Put
            Customer result = GetObjectFromAPI<Customer>(HttpMethod.Post, "https://" + oktaOrg + "/api/v1/users/" + Id, customer);

            return result;
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri, object model) {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(uri);
            request.Method = method;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("SSWS", oktaApiKey);

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
            }
            
            return request;
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, String uri) {
            return CreateBaseRequest(method, uri, null);
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, String uri, object model) {
            T result = default(T);
            HttpRequestMessage request = CreateBaseRequest(method, uri, model);
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode) {
                result = response.Content.ReadAsAsync<T>().Result;
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
                foreach (var customerObject in apiObjects) {

                }
                results = apiObjects.ToArray<T>();
            }

            return results;
        }

    }
}