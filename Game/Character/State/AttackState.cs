using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character.State
{
    public partial class AttackState(GameCharacter character) : StateBase(character)
    {
        private Area2D _attackArea; // 攻击判定区域
        private Timer _attackDurationTimer; // 用于控制攻击持续时间或动画结束

        public override void Enter(object data = null)
        {
            _attackArea = GameCharacter.GetNode("WeaponMountPoint").GetNode("CurrentWeapon").GetNode<Area2D>("AttackArea"); // 获取攻击判定区域

            GD.Print("Entering Attack State");
            GameCharacter.Velocity = Vector2.Zero; // 攻击时停止移动
            var direction = GameCharacter.GetDirectionSuffix(GameCharacter.LastMovementDirection); // 获取角色的朝向
            GameCharacter.AnimationPlayer.Play($"character_attack{direction}"); // 播放攻击动画

            // 震动摄像机：已经移动到BaseWeapon类中处理
            // 获取父节点下PlayerCharacter的Camera2D节点
            //var camera = GameCharacter.GetParent().GetNode<CameraShakeController>("Player/Camera2D");
            //camera.AddTrauma(0.5f); // 添加震动效果，参数可以根据需要调整

            // 启用攻击判定区域：计划迁移到BaseWeapon中处理
            _attackArea.Monitoring = true;
            _attackArea.Monitorable = true; // 如果需要敌人检测到攻击区域

            // 设置一个计时器，当动画播放完毕或攻击持续时间结束时退出攻击状态
            _attackDurationTimer = new Timer();
            _attackDurationTimer.WaitTime = GameCharacter.AnimationPlayer.GetAnimation($"character_attack{direction}").Length; // 或者一个固定的攻击持续时间
            _attackDurationTimer.OneShot = true;
            GameCharacter.AddChild(_attackDurationTimer); // 将计时器添加到角色作为子节点
            _attackDurationTimer.Timeout += OnAttackTimerTimeout;
            _attackDurationTimer.Start();

            // 连接攻击区域的 body_entered 信号，用于处理碰撞
            _attackArea.BodyEntered += OnAttackAreaBodyEntered;
        }

        public override void Update(double delta)
        {
            
        }

        public override void Exit()
        {
            GD.Print("Exiting Attack State");
            GameCharacter.AnimationPlayer.Stop(); // 停止攻击动画
                                     // 禁用攻击判定区域
            _attackArea.Monitoring = false;
            _attackArea.Monitorable = false;

            // 断开信号连接并释放计时器
            if (_attackDurationTimer != null)
            {
                _attackDurationTimer.Stop();
                _attackDurationTimer.Timeout -= OnAttackTimerTimeout;
                _attackDurationTimer.QueueFree();
                _attackDurationTimer = null;
            }
            _attackArea.BodyEntered -= OnAttackAreaBodyEntered;
        }

        private void OnAttackTimerTimeout()
        {
            // 动画播放完毕或攻击持续时间结束，转换回空闲或移动状态
            if (GameCharacter is PlayerCharacter player)
            {
                if (player.Velocity.LengthSquared() > 0)
                {
                    player.State.TransitionTo(GameCharacter.States.Moving);
                }
                else
                {
                    player.State.TransitionTo(GameCharacter.States.Idle);
                }
            }
        }

        private void OnAttackAreaBodyEntered(Node2D body)
        {
            if (body is Monster monster)
            {
                // 对怪物造成伤害
                // monster.TakeDamage(10); // 假设Monster有一个TakeDamage方法

                // 施加攻击击退
                Vector2 knockbackDirection = (monster.GlobalPosition - GameCharacter.GlobalPosition).Normalized();
                monster.ApplyPush(knockbackDirection * 700f); // 攻击击退力
            }
        }
    }
}
