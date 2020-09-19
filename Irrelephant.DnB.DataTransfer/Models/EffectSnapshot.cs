using System;
using System.Runtime.Serialization;
using Irrelephant.DnB.Core.Data;

namespace Irrelephant.DnB.DataTransfer.Models
{
    [DataContract]
    public class EffectSnapshot
    {
        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "targets")]
        public Targets Targets { get; set; }
    }
}
