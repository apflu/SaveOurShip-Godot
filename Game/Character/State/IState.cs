namespace SaveOurShip.Game.Character
{
    public interface IState
    {
        GameCharacter GameCharacter { get; }
        void Enter(object data = null);
        void Exit();

        void Update(double delta);
    }
}
