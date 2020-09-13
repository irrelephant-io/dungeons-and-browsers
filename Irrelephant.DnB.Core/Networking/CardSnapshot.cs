using System;
using System.Runtime.Serialization;

namespace Irrelephant.DnB.Core.Networking
{
    [DataContract]
    public class CardSnapshot
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "graphicId")]
        public string GraphicId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "actionCost")]
        public int ActionCost { get; set; }
    }
}
