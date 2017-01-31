using System;
using System.Runtime.Serialization;

namespace OktaDemoAPIServer.Models {
    [DataContract]
    public class User {

        [DataMember(Name = "id")]
        public String Id { get; set; }
    }
}