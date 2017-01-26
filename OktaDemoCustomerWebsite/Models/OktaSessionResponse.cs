using System;
using System.Runtime.Serialization;

namespace OktaDemoCustomerWebsite.Models {
    [DataContract]
    public class OktaSessionResponse {

        [DataMember(Name = "id")]
        public String Id { get; set; }

        [DataMember(Name = "userId")]
        public String UserId { get; set; }
    }
}