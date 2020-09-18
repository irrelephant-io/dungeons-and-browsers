using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Irrelephant.DnB.Core.Networking
{
    [DataContract]
    public class CardTargets
    {
        [DataMember(Name = "CardId")]
        public Guid CardId { get; set; }

        [DataMember(Name = "EffectTargets")]
        public Dictionary<Guid, Guid[]> EffectTargets { get; set; }
    }
}
