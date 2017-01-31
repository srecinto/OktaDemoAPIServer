using System;
using System.Runtime.Serialization;

namespace OktaDemoAPIServer.Models {
    [DataContract]
    public class Embedded {

        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}