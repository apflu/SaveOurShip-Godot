using Godot;
using System;
using System.Threading;

public partial class CameraShakeController : Camera2D
{
    [Export]
    public float DecayRate = 0.8f; // 震动衰减速度 (0.0 - 1.0, 越大衰减越快)
    [Export]
    public Vector2 MaxOffset = new Vector2(10, 8); // 最大 X/Y 偏移量
    [Export]
    public float MaxRotation = 0.05f; // 最大旋转角度（弧度）

    private float _trauma = 0.0f; // 当前的震动值 (0.0 - 1.0)
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    private FastNoiseLite _noiseX;
    private FastNoiseLite _noiseY;
    private FastNoiseLite _noiseRotation;
    private float _noiseTime = 0.0f; // 用于遍历噪声的“时间”

    public override void _Ready()
    {
        _rng.Randomize(); // 初始化随机数生成器

        // 初始化噪声生成器
        _noiseX = new FastNoiseLite();
        _noiseX.Seed = (int)_rng.Randi(); // 将 uint 转换为 int
        _noiseX.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin; // Perlin 噪声通常看起来比较自然
        _noiseX.Frequency = 0.1f; // 调整为 Frequency

        _noiseY = new FastNoiseLite();
        _noiseY.Seed = (int)_rng.Randi(); // 使用不同的种子
        _noiseY.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        _noiseY.Frequency = 0.1f;

        _noiseRotation = new FastNoiseLite();
        _noiseRotation.Seed = (int)_rng.Randi();
        _noiseRotation.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        _noiseRotation.Frequency = 0.1f;
    }


    public override void _Process(double delta)
    {
        if (_trauma > 0)
        {
            _trauma = Mathf.Max(0, _trauma - DecayRate * (float)delta);
            float currentShakeAmount = Mathf.Pow(_trauma, 2);

            _noiseTime += (float)delta * 50f; // 控制噪声变化的频率

            Vector2 randomOffset = new Vector2(
                _noiseX.GetNoise2D(_noiseTime, 0) * MaxOffset.X * currentShakeAmount,
                _noiseY.GetNoise2D(_noiseTime, 1000) * MaxOffset.Y * currentShakeAmount // 使用不同的 Y 偏移避免 X/Y 轴同步
            );

            float randomRotation = _noiseRotation.GetNoise2D(_noiseTime, 2000) * MaxRotation * currentShakeAmount;

            Offset = randomOffset;
            Rotation = randomRotation;
        }
        else
        {
            // 当震动结束时，将偏移和旋转归零，避免小数值残留
            Offset = Vector2.Zero;
            Rotation = 0;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        // 如果输入debug_camera_shake，则添加创伤
        if (@event.IsActionPressed("debug_camera_shake"))
        {
            AddTrauma(0.4f); // 每次按下键时增加 0.1 的创伤值
        }
    }

    public void AddTrauma(float amount)
    {
        _trauma = Mathf.Min(1.0f, _trauma + amount); // 确保创伤值不超过 1.0
    }

}
