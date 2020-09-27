using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Irrelephant.DnB.DataTransfer.Models
{
    [DataContract]
    public class CombatInfo
    {
        [DataMember(Name = "combatId")]
        [JsonPropertyName("combatId")]
        public Guid CombatId { get; set; }

        [DataMember(Name = "attackers")]
        [JsonPropertyName("attackers")]
        public IEnumerable<string> Attackers { get; set; }

        [DataMember(Name = "defenders")]
        [JsonPropertyName("defenders")]
        public IEnumerable<string> Defenders { get; set; }

        [DataMember(Name = "round")]
        [JsonPropertyName("round")]
        public int Round { get; set; }
    }
}
