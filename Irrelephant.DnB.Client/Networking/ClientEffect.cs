using System;
using Irrelephant.DnB.Core.Data;
using Irrelephant.DnB.Core.Data.Effects;

namespace Irrelephant.DnB.Client.Networking
{
    public class ClientEffect : Effect
    {
        public override Guid Id { get; }

        private readonly string _name;
        public override string Name => _name;

        public override Targets ValidTargets { get; }

        public ClientEffect(Guid id, string name, Targets targets)
        {
            Id = id;
            _name = name;
            ValidTargets = targets;
        }
    }
}
