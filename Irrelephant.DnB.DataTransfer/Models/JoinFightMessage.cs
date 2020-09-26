using Irrelephant.DnB.Core.Data;

namespace Irrelephant.DnB.DataTransfer.Models
{
    public class JoinFightMessage
    {
        public int Position { get; set; }

        public JoinedSide Side { get; set; }

        public CharacterSnapshot Character { get; set; }
    }
}
