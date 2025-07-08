using Godot;
using System;

public partial class BaseCombo : Resource
{
    [Export] public string AnimationName { get; set; } = "";
    [Export] public float Duration { get; set; } = 0.5f; // 动画持续时间
    [Export] public float Damage { get; set; } = 10f;
    [Export] public float KnockbackForce { get; set; } = 700f;
    [Export] public float ComboInputWindow { get; set; } = 0.2f; // 连招输入窗口
    // 如果 Area2D 属性需要变化，可以考虑这里包含数据，由 BaseWeapon 的 Area2D 动态调整
    // [Export] public Vector2 AttackAreaOffset { get; set; } = Vector2.Zero;
    // [Export] public Vector2 AttackAreaSize { get; set; } = new Vector2(32, 32);
}
