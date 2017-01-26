using System;
using System.Runtime.Serialization;

namespace OktaDemoAPIServer.Models {
    [DataContract]
    public class AuthenticationResponse {

        [DataMember(Name = "sessionToken")]
        public String SessionToken { get; set; }

    }
}