using System;
using System.Runtime.Serialization;

namespace OktaDemoCustomerWebsite.Models {
    [DataContract]
    public class Profile {

        [DataMember(Name = "firstName")]
        public String FirstName { get; set; }

        [DataMember(Name = "lastName")]
        public String LastName { get; set; }

        [DataMember(Name = "email")]
        public String Email { get; set; }

        [DataMember(Name = "login")]
        public String Login { get; set; }

        [DataMember(Name = "customerType")]
        public Customer.CustomerType Type { get; set; }

        [DataMember(Name = "loyaltyPoints")]
        public int LoyaltyPoints { get; set; }

    }
}