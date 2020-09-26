using System;
using System.Linq;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.Cards;
using Irrelephant.DnB.Core.Characters;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Exceptions;
using Irrelephant.DnB.Core.Utils;
using Irrelephant.DnB.DataTransfer.Models;

namespace Irrelephant.DnB.Client.Networking
{
    public class ClientCard : Card
    {
        public string RemoteText { get; set; }

        public override string Text => RemoteText;

        public async override Task Play(PlayerCharacter player, ITargetProvider targetProvider)
        {
            if (!CanPlay(player))
            {
                throw new NotEnoughActionsException();
            }

            var targets = new CardTargets {
                CardId = Id,
                EffectTargets = await Effects.ToDictionaryAsync(
                    e => e.Id, 
                    async e => (await targetProvider.PickTarget(e)).Select(c => c.Id).ToArray())
            };
            var clientCharacter = (ClientPlayerCharacter)player;
            Console.WriteLine(clientCharacter.ClientCombat == null);
            await clientCharacter.ClientCombat.PlayCard(targets);
        }
    }
}
