using System;

namespace Irrelephant.DnB.Core.Exceptions
{
    public class NotMyTurnException : Exception
    {
        public NotMyTurnException() : base("Can't act as this is not your turn yet!")
        {

        }
    }
}
