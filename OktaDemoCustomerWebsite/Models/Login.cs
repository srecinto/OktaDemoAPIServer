using System;
using System.Runtime.Serialization;

namespace OktaDemoCustomerWebsite.Models {
    public class Login {

        public Login() {
            IsAuthenticated = false;
        }

        public String UserName { get; set; }
        public String Password { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}