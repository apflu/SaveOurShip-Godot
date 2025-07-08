using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character.State
{
    public partial class IdleState(GameCharacter character) : StateBase(character)
    {
        public override void Enter(object data = null)
        {
            base.Enter(data);

            GD.Print("Entering Idle State");
            // 空闲时停止移动，根据玩家上次移动的方向播放空闲动画
            GameCharacter.Velocity = Vector2.Zero;
            string directionSuffix = GameCharacter.GetDirectionSuffix(GameCharacter.LastMovementDirection);

            // debug: 输出GameCharacter的AnimationPlayer
            GD.Print($"IdleState: {GameCharacter.AnimationPlayer}");
            // debug: 同时输出一个null的GameCharacter
            GD.Print($"IdleState: {null}");
            GameCharacter.AnimationPlayer.Play($"character_idle{directionSuffix}");
        }

        public override void Update(double delta)
        {
            base.Update(delta);

            // 如果玩家开始移动，则切换到移动状态
            if (GameCharacter.Velocity.LengthSquared() > 0.1f)
            {
                GameCharacter.State.TransitionTo(GameCharacter.States.Moving);
                return;
            }
        }

        public override void Exit()
        {
            GD.Print("Exiting Idle State");
        }
    }
}
