using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character.State
{
    public partial class MovingState(GameCharacter character) : StateBase(character)
    {
        private Vector2 lastMovementDirection;
        public override void Enter(object data = null)
        {
            GD.Print("Entering Moving State");
            lastMovementDirection = GameCharacter.LastMovementDirection; // 获取上次的移动方向

            string _lastDirectionSuffix = GameCharacter.GetDirectionSuffix(GameCharacter.LastMovementDirection);
            GameCharacter.AnimationPlayer.Play($"character_walk{_lastDirectionSuffix}");
        }
        public override void Exit()
        {
            GD.Print("Exiting Walk State");
        }

        public override void Update(double delta)
        {
            // 如果玩家没有移动，则切换到空闲状态
            if (GameCharacter.Velocity.LengthSquared() < 0.1f)
            {
                GameCharacter.State.TransitionTo(GameCharacter.States.Idle);
                return;
            }

            // 如果玩家的移动方向发生变化，则更新动画
            Vector2 currentMovementDirection = GameCharacter.LastMovementDirection;
            if (currentMovementDirection != lastMovementDirection)
            {
                lastMovementDirection = currentMovementDirection; // 更新最后的移动方向
                string directionSuffix = GameCharacter.GetDirectionSuffix(currentMovementDirection);
                GameCharacter.AnimationPlayer.Play($"character_walk{directionSuffix}");
            }   

        }
    }
}
