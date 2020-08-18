using System.Collections.Generic;

namespace Irrelephant.DnB.Core.Infrastructure
{
    public class GameLog
    {
        private readonly IList<string> _messages = new List<string>();

        public IEnumerable<string> Messages => _messages;

        public void LogMessage(string message)
        {
            _messages.Add(message);
        }
    }
}
