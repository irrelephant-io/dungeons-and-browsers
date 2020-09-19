using System;
using System.Runtime.Serialization;

namespace Irrelephant.DnB.DataTransfer.Models
{
    [DataContract]
    public class CharacterSnapshot
    {
        [DataMember(Name = "id", IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(Name = "graphicId")]
        public string GraphicId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "health")]
        public int Health { get; set; }

        [DataMember(Name = "maxHealth")]
        public int MaxHealth { get; set; }

        [DataMember(Name = "armor")]
        public int Armor { get; set; }

        [DataMember(Name = "actions")]
        public int Actions { get; set; }

        [DataMember(Name = "actionsMax")]
        public int ActionsMax { get; set; }

        [DataMember(Name = "deck")]
        public DeckSnapshot Deck { get; set; }

        [DataMember(Name = "intent", IsRequired = false)]
        public string Intent { get; set; }
    }
}
