using System;

namespace Irrelephant.DnB.Core.Exceptions
{
    public class NoCardsException : Exception
    {
        public NoCardsException()
            : base("Not enough cards to draw.")
        {

        }
    }
}
