using System;
using System.Runtime.Serialization;

namespace Irrelephant.DnB.DataTransfer.Models
{
    [DataContract]
    public class CombatSnapshot
    {
        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Name = "attackers")]
        public CharacterSnapshot[] Attackers { get; set; }

        [DataMember(Name = "defenders")]
        public CharacterSnapshot[] Defenders { get; set; }

        [DataMember(Name = "activeCharacterId")]
        public Guid ActiveCharacterId { get; set; }

        [DataMember(Name = "turn", IsRequired = true)]
        public int Turn { get; set; }
    }
}
