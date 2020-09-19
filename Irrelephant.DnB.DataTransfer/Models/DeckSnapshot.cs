using System.Runtime.Serialization;

namespace Irrelephant.DnB.DataTransfer.Models
{
    [DataContract]
    public class DeckSnapshot
    {
        [DataMember(Name = "drawPile")]
        public CardSnapshot[] DrawPile { get; set; }

        [DataMember(Name = "hand")]
        public CardSnapshot[] Hand { get; set; }

        [DataMember(Name = "discardPile")]
        public CardSnapshot[] DiscardPile { get; set; }
    }
}
