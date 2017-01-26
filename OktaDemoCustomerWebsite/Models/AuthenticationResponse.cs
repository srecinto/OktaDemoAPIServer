using System;
using System.Runtime.Serialization;

namespace OktaDemoCustomerWebsite.Models {
    [DataContract]
    public class AuthenticationResponse {

        [DataMember(Name = "sessionToken")]
        public String SessionToken { get; set; }

    }
}