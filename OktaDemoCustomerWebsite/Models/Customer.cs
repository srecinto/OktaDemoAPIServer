using System;
using System.Runtime.Serialization;

namespace OktaDemoCustomerWebsite.Models {

    [DataContract]
    public class Customer {
        public enum CustomerType { Basic, Premium, Enterprise }

        [DataMember(Name ="id")]
        public String Id { get; set; }
        [DataMember(Name = "profile")]
        public Profile Profile { get; set; }

    }
}