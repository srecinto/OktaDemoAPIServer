using System;
using System.Runtime.Serialization;

namespace OktaDemoCustomerWebsite.Models {
    [DataContract]
    public class TokenIntrospectionResponse {

        [DataMember(Name = "active")]
        public Boolean Active { get; set; }

        [DataMember(Name = "scope")]
        public String Scope { get; set; }

        [DataMember(Name = "username")]
        public String UserName { get; set; }

        [DataMember(Name = "exp")]
        public String Exp { get; set; }

        [DataMember(Name = "iat")]
        public String IAT { get; set; }

        [DataMember(Name = "sub")]
        public String Sub { get; set; }

        [DataMember(Name = "aud")]
        public String Aud { get; set; }

        [DataMember(Name = "iss")]
        public String ISS { get; set; }

        [DataMember(Name = "jti")]
        public String JTI { get; set; }

        [DataMember(Name = "token_type")]
        public String TokenType { get; set; }

        [DataMember(Name = "client_id")]
        public String ClientId { get; set; }

        [DataMember(Name = "uid")]
        public String UID { get; set; }


    }
}
 