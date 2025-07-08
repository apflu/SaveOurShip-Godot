using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character
{
    public partial class Monster : MonsterBase, IMonsterPushable
    {
        public Vector2 MovementTarget
        {
            get { return _navigationAgent.TargetPosition; }
            set { _navigationAgent.TargetPosition = value; }
        }

        private NavigationAgent2D _navigationAgent = null!;

        public override void _Ready()
        {
            base._Ready();

            _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

            LinearDamp = Force_Magnitude / (Speed * Mass); // 线性阻尼，模拟摩擦力

            // 可以使用 Timer 节点或者在 _PhysicsProcess 中手动计时
            Callable.From(RequestPathUpdate).CallDeferred(); // 初始请求一次
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            // 怪物AI移动逻辑 - 现在基于寻路路径
            UpdatePathFollowing(delta);
/*
            // 临时：怪物永远瞄准玩家为目标移动
            Vector2 targetPosition = GetParent().GetNode<PlayerCharacter>("Player").GlobalPosition;
            Vector2 direction = (targetPosition - GlobalPosition).Normalized();

            ConstantForce = direction * Force_Magnitude; // 施加一个恒定的力*/
        }

        public void ApplyPush(Vector2 force)
        {
            float clampedResistance = Mathf.Clamp(Push_Resistance, 0.0f, 1.0f);
            // 计算实际施加的力乘数
            // 当 clampedResistance 为 0.0 时，multiplier = 1.0 (完全接受力)
            // 当 clampedResistance 为 1.0 时，multiplier = 0.0 (完全抵消力)
            float forceMultiplier = 1.0f - clampedResistance;
            // 施加力，乘以计算出的乘数
            // 只有当 forceMultiplier 大于一个很小的值时才施加力，避免不必要的计算和浮点误差
            if (forceMultiplier > 0.001f) // 使用一个小的 epsilon 值来避免浮点数比较问题
            {
                // RigidBody2D 使用 ApplyCentralImpulse 或 ApplyImpulse 来施加瞬时力
                // ApplyCentralImpulse 在物体的质心施加力
                ApplyCentralImpulse(force * forceMultiplier);
            }
        }

        private async void RequestPathUpdate()
        {
            // 获取玩家位置作为目标
            PlayerCharacter player = GetParent().GetNodeOrNull<PlayerCharacter>("Player");
            if (player == null)
            {
                GD.PrintErr("PlayerCharacter not found in parent node.");
                return; // 如果没有找到玩家，退出
            }

            Vector2 targetPosition = player.GlobalPosition;

            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame); // 等待一个物理帧，确保物理引擎更新完成
            MovementTarget = targetPosition; // 设置目标位置
        }

        private void UpdatePathFollowing(double delta)
        {
            Vector2 forceToApply = Vector2.Zero;

            if (_navigationAgent.IsNavigationFinished())
            {
                // 没有路径或路径已完成，停止移动
                ConstantForce = Vector2.Zero;
                return;
            }

            // NavigationAgent2D 已经处理了路径计算，我们只需获取下一个路径点
            Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();

            // 计算指向下一个路径点的方向
            Vector2 direction = (nextPathPosition - GlobalPosition).Normalized();

            // 施加力来引导 RigidBody2D 沿着路径移动
            ConstantForce = direction * Force_Magnitude;
        }

        // 辅助方法：如果需要重新计算路径，可以在玩家移动后调用此方法
        // (例如，在PlayerCharacter中连接信号或手动调用)
        public void ReCalculatePath()
        {
            RequestPathUpdate();
        }
    }
}
