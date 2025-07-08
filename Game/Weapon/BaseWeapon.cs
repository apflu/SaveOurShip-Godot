using Godot;
using System;

public partial class BaseWeapon : Node2D
{
    [Export]
    public Timer CameraShakeTimer;

    [Export(PropertyHint.Range, "0.0,1.0")]
    public float CameraShakeTrauma = 0.5f;

    protected CameraShakeController _cameraShakeController;

    public override void _Ready()
    {
        // 查找 Timer 和 CameraShakeController
        if (CameraShakeTimer == null)
        {
            CameraShakeTimer = GetNode<Timer>("Timers/CameraShakeStartTimer");
            if (CameraShakeTimer == null)
            {
                GD.PushError($"{Name}: CameraShakeTimer not found!");
            }
        }

        // Camera2D 及其 CameraShakeController 位于 PlayerCharacter 下
        _cameraShakeController = GetNode<CameraShakeController>("../../Camera2D");
        if (_cameraShakeController == null)
        {
            GD.PushError($"{Name}: CameraShakeController not found!");
        }

        // 连接 Timer 信号 (如果Timer不是武器子节点，则需要确保只连接一次)
        // 可以在 PlayerCharacter 层面连接，或者确保BaseWeapon只连接一次
        // 如果CameraShakeTimer是武器自身的子节点，就直接连接到自己
        if (CameraShakeTimer != null && !CameraShakeTimer.IsConnected("timeout", Callable.From(_OnCameraShakeTimerTimeout)))
        {
            CameraShakeTimer.Connect("timeout", Callable.From(_OnCameraShakeTimerTimeout));
        }
    }

    public virtual void Fire()
    {
        if (CameraShakeTimer != null)
        {
            CameraShakeTimer.Start();
        }
    }

    protected virtual void _OnCameraShakeTimerTimeout()
    {
        if (_cameraShakeController != null)
        {
            _cameraShakeController.AddTrauma(CameraShakeTrauma);
        }
    }
}
