using System;
using System.Runtime.Serialization;

namespace OktaDemoAPIServer.Models {
    [DataContract]
    public class Authentication {

        [DataMember(Name = "username")]
        public String UserName { get; set; }

        [DataMember(Name = "password")]
        public String Password { get; set; }

    }
}