using System.Threading.Tasks;
using Irrelephant.DnB.Client.Networking;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.DataTransfer.Models;
using Moq;

namespace Irrelephant.DnB.Client.Tests.Mocks
{
    public class RemoteCombatListenerMock : Mock<IRemoteCombatListener>
    {
        private bool _isMyTurn;

        public const int NetworkDelay = 300;

        public RemoteCombatListenerMock SetupMock()
        {
            SetupNotifyJoin();
            SetupNotifyEndTurn();
            SetupPlayCard();
            return this;
        }

        private void SetupPlayCard()
        {
            Setup(l => l.PlayCard(It.IsAny<CardTargets>()))
                .Callback<CardTargets>(async cardTargets => {
                    if (!_isMyTurn)
                    {
                        return;
                    }

                    await Task.Delay(2 * NetworkDelay);
                    Raise(l => l.OnCardPlayed += null, cardTargets.CardId);
                    Raise(l => l.OnDiscardCard += null, cardTargets.CardId);
                    var update = FakeCombat.CharacterUpdate;
                    update.Health = FakeCombat.Attacker.Health;
                    update.Actions = 4 - FakeCombat.Deck.Card3.ActionCost;
                    Raise(l => l.OnCharacterUpdated += null, update);
                })
                .Returns(Task.Delay(NetworkDelay));
        }

        private void SetupNotifyEndTurn()
        {
            Setup(l => l.NotifyEndTurnAsync())
                .Callback(async () =>
                {
                    if (!_isMyTurn)
                    {
                        return;
                    }

                    await Task.Delay(NetworkDelay);
                    _isMyTurn = false;
                    await Task.Delay(2 * NetworkDelay);
                    _isMyTurn = true;
                    Raise(l => l.OnMyTurn += null);
                })
                .Returns(async () =>
                {
                    if (!_isMyTurn)
                    {
                        throw new NotMyTurnException();
                    }

                    await Task.Delay(NetworkDelay / 2);
                    return true;
                });
        }

        private void SetupNotifyJoin()
        {
            Setup(l => l.NotifyJoinedAsync())
                .Callback(async () =>
                {
                    await Task.Delay(NetworkDelay);
                    Raise(l => l.OnJoinedCombat += null, FakeCombat.Object);
                    await Task.Delay(2 * NetworkDelay);
                    _isMyTurn = true;
                    Raise(l => l.OnMyTurn += null);
                })
                .Returns(Task.Delay(NetworkDelay / 2));
        }
    }
}
