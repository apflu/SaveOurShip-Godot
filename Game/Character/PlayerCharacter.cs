using Godot;
using SaveOurShip.Game.Character;
using SaveOurShip.Game.Character.State;
using System;
using System.Collections.Generic;

public partial class PlayerCharacter : GameCharacter
{
    [Export]
    public float Push_Force = 500f;
    [Export]
    public float InputBufferWindow = 0.2f; // 输入缓冲窗口，单位为秒
    [Export]
    public string[] ActionsToBuffer = ["attack", "jump"]; // 默认缓冲攻击和跳跃

    private Vector2 _velocity = Vector2.Zero;
    private Dictionary<StringName, float> _bufferedActions = [];

    public new static class States
    {
        public static readonly string Dashing = "Player_Dashing";
        public static readonly string Interacting = "Player_Interacting";
        public static readonly string Aiming = "Player_Aiming";
        public static readonly string UsingSkill = "Player_UsingSkill";
    }

    public override void _Ready()
    {
        base._Ready(); // 确保调用基类的_Ready()来初始化_stateMachine


        // TODO: 添加其他 PlayerCharacter.States 中的状态

        // 设置玩家的初始状态
        State.SetInitialState(GameCharacter.States.Idle);
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta); // 调用基类的物理处理方法

        float currentTime = Time.GetTicksMsec() / 1000.0f; // 获取当前时间（秒）
        List<StringName> actionsToRemove = [];

        // 检查缓冲的动作是否超时，并移除超时的动作
        foreach (var entry in _bufferedActions)
        {
            if (currentTime - entry.Value > InputBufferWindow)
            {
                actionsToRemove.Add(entry.Key);
            }
        }

        // 从缓冲列表中移除超时的动作
        foreach (var actionName in actionsToRemove)
        {
            _bufferedActions.Remove(actionName);
        }

        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        _velocity = inputDirection * Speed;

        if (inputDirection.LengthSquared() > 0.1f) // 如果有输入方向，则更新 LastMovementDirection
        {
            LastMovementDirection = inputDirection.Normalized();
        }

        // 如果当前状态是移动状态，则更新 LastMovementDirection
        if (State.CurrentStateName == GameCharacter.States.Moving || State.CurrentStateName == GameCharacter.States.Idle)
        {
            LastMovementDirection = _velocity.Normalized(); // 更新最后的移动方向

            // TODO: 将外部推力叠加到 _velocity 上

            Velocity = _velocity;
        }

        // 更新状态机
        State.Update(delta);

        MoveAndSlide();

        // 对怪物施加推力
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            if (collision.GetCollider() is Monster monster) // 检查碰撞到的是否是怪物
            {
                // 计算推力方向 (从玩家到怪物)
                Vector2 pushDirection = (monster.GlobalPosition - GlobalPosition).Normalized();

                // 将推力施加给怪物
                monster.ApplyPush(pushDirection * Push_Force);
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event); // 调用基类的输入处理方法

        // 遍历ActionsToBuffer中的每个动作
        foreach (var actionName in ActionsToBuffer)
        {
            if (@event.IsActionPressed(actionName))
            {
                _bufferedActions[actionName] = Time.GetTicksMsec() / 1000.0f; // 记录动作按下的时间
                return; // 一旦捕获到一个动作，就不再继续处理其他动作
            }
        }

        // 捕获攻击输入
        /*if (@event.IsActionPressed("attack") && State.CurrentStateName != GameCharacter.States.Attacking)
        {
            State.TransitionTo(GameCharacter.States.Attacking);
        }*/
    }

    public void EnableAttackArea()
    {
        // 假设你有一个名为 "AttackArea" 的 Area2D 子节点用于攻击判定
        // 你也可以根据需要有多个攻击区域
        Area2D attackArea = GetNodeOrNull<Area2D>("AttackArea");
        if (attackArea != null)
        {
            attackArea.Monitoring = true; // 激活区域检测
            attackArea.Monitorable = true; // 允许其他对象检测到此区域
            GD.Print("Attack Area Enabled");
        }
    }

    public void DisableAttackArea()
    {
        Area2D attackArea = GetNodeOrNull<Area2D>("AttackArea");
        if (attackArea != null)
        {
            attackArea.Monitoring = false; // 禁用区域检测
            attackArea.Monitorable = false;
            GD.Print("Attack Area Disabled");
        }
    }

    public bool HasBufferedAction(StringName actionName)
    {
        return _bufferedActions.ContainsKey(actionName);
    }

    public void ConsumeBufferedAction(StringName actionName)
    {
        _bufferedActions.Remove(actionName);
    }
}
