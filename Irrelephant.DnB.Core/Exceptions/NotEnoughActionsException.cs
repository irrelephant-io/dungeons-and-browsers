using System;

namespace Irrelephant.DnB.Core.Exceptions
{
    public class NotEnoughActionsException : Exception
    {
        public NotEnoughActionsException()
            : base("Not enough actions to play the card.")
        {

        }
    }
}
