using System;
using System.Collections.Generic;
using System.Web.Http;

using OktaDemoAPIServer.Models;
using OktaDemoAPIServer.Utils;


namespace OktaDemoAPIServer.Controllers
{
    public class CustomerController : ApiController {

        // GET: api/Customer
        public IEnumerable<Customer> Get() {
            return RESTUtil.GetAllCustomers();
        }

        // GET: api/Customer/5
        public Customer Get(String id)
        {
            return RESTUtil.GetCustomerById(id);
        }

        // POST: api/Customer
        public Customer Post([FromBody]Customer customer)
        {
            return RESTUtil.AddNewCustomer(customer);
        }

        // PUT: api/Customer/5
        public Customer Put(String id, [FromBody]Customer customer)
        {
            return RESTUtil.UpdateCustomer(id, customer);
        }

        // DELETE: api/Customer/5
        public void Delete(String id)
        {
            // Not Implementing
        }
    }
}
