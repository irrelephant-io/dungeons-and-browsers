using System;
using System.Threading.Tasks;
using Irrelephant.DnB.Core.GameFlow;

namespace Irrelephant.DnB.Core.Characters.Controller
{
    public abstract class CharacterController
    {
        public Character Character { get; }

        protected CharacterController(Character character)
        {
            Character = character;
            Character.OnUpdate += OnUpdate;
            Character.OnDeath += Die;
        }

        public virtual Task Act(Combat combat)
        {
            return Task.CompletedTask;
        }

        public event Action OnAction;

        public event Action OnDeath;

        protected virtual void Die()
        {
            OnDeath?.Invoke();
        }

        protected virtual void OnUpdate()
        {
            InvokeOnAction();
        }

        public virtual Task LeaveCombat()
        {
            OnAction?.Invoke();
            return Task.CompletedTask;
        }

        public void InvokeOnAction()
        {
            OnAction?.Invoke();
        }
    }
}
