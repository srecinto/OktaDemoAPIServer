using System;
using System.Runtime.Serialization;

namespace OktaDemoCustomerWebsite.Models {
    [DataContract]
    public class OIDCTokenResponse {

        [DataMember(Name = "access_token")]
        public String AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public String TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public Int32 ExpiresIn { get; set; }

        [DataMember(Name = "scope")]
        public String Scope { get; set; }

    }
}