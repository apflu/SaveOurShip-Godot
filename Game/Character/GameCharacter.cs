using Godot;
using SaveOurShip.Game.Character.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character
{
    /// <summary>
    /// 表示游戏中的角色基类
    /// 在编辑器中，记得添加AnimationPlayer节点，命名为"AnimationPlayer"
    /// </summary>
    public abstract partial class GameCharacter : CharacterBody2D
    {
        [Export]
        public float Speed = 300.0f;
        [Export]
        public float Push_Resistance = 0.5f; // 推力抵抗力，值越大，抵抗力越强

        // 最后一次移动的方向
        public Vector2 LastMovementDirection { get; protected set; } = Vector2.Zero;


        public static class States
        {
            public static readonly string Idle = "Idle";
            public static readonly string Moving = "Moving";
            public static readonly string Attacking = "Attacking";
            public static readonly string TakingDamage = "TakingDamage";
            public static readonly string Dead = "Dead";
        }
        public StateMachine State { get; protected set; }

        // 获取AnimationPlayer节点（必然存在）
        public AnimationPlayer AnimationPlayer { get; protected set; } = null!;

        public override void _Ready()
        {
            base._Ready();
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            State = GetNode<StateMachine>("StateMachine");
            State.AddState(States.Idle, new IdleState(this));
            State.AddState(States.Moving, new MovingState(this));
            State.AddState(States.Attacking, new AttackState(this));
            // 添加其他状态

            State.SetInitialState(States.Idle); // 设置初始状态为Idle

        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
            MoveAndSlide();
        }

        public virtual string GetDirectionSuffix(Vector2 direction)
        {
            if (direction.LengthSquared() < 0.1f) // 如果速度太小，认为是空闲方向
            {
                // 默认返回南方
                return "_S";
            }

            if (Mathf.Abs(direction.X) >= Mathf.Abs(direction.Y))
            {
                // 水平方向为主
                if (direction.X > 0) return "_E";
                else return "_W";
            }
            else
            {
                // 垂直方向为主
                if (direction.Y > 0) return "_S";
                else return "_N";
            }
        }
    }
}
