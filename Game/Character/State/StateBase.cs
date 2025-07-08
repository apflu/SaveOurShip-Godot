using Godot;

namespace SaveOurShip.Game.Character
{
    public abstract partial class StateBase(GameCharacter character) : Node, IState
    {
        public GameCharacter GameCharacter { get; } = character;

        public virtual void Enter(object data = null) { }
        public virtual void Exit() { }
        public virtual void Update(double delta) { }
        public virtual void HandleInput(InputEvent @event) { }
    }
}
