using System;
using System.Runtime.Serialization;

namespace OktaDemoAPIServer.Models {
    [DataContract]
    public class OktaSessionResponse {

        [DataMember(Name = "sessionToken")]
        public String SessionToken { get; set; }

        [DataMember(Name = "userId")]
        public String UserId { get; set; }
    }
}