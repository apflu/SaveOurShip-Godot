using Godot;
using System;

public partial class ShockwaveEffect : TextureRect
{
    private ShaderMaterial _shaderMaterial;
    private bool _isShockwaveActive = false;
    private float _currentRadius = 0.0f;
    private float _maxRadius = 0.8f; // 冲击波最大半径
    private float _shockwaveDuration = 0.5f; // 冲击波持续时间（秒）
    private float _timeElapsed = 0.0f;

    public override void _Ready()
    {
        // 获取ShaderMaterial
        _shaderMaterial = Material as ShaderMaterial;
        if (_shaderMaterial == null)
        {
            GD.PushError("ShockwaveEffectController: Material is not a ShaderMaterial.");
            QueueFree(); // 如果没有ShaderMaterial，则销毁自身
            return;
        }

        // 初始隐藏效果
        Visible = false;
    }

    public override void _Process(double delta)
    {
        if (_isShockwaveActive)
        {
            _timeElapsed += (float)delta;
            if (_timeElapsed >= _shockwaveDuration)
            {
                _isShockwaveActive = false;
                Visible = false;
                _shaderMaterial.SetShaderParameter("radius", 0.0f); // 重置半径
                return;
            }

            // 根据时间插值计算当前半径
            _currentRadius = Mathf.Lerp(0.0f, _maxRadius, _timeElapsed / _shockwaveDuration);
            _shaderMaterial.SetShaderParameter("radius", _currentRadius);

            // 可以根据时间调整强度，使其逐渐减弱
            float currentStrength = Mathf.Lerp(0.05f, 0.0f, _timeElapsed / _shockwaveDuration);
            _shaderMaterial.SetShaderParameter("strength", currentStrength);
        }
    }

    /// <summary>
    /// 触发冲击波效果
    /// </summary>
    /// <param name="position">冲击波中心的世界坐标</param>
    public void TriggerShockwave(Vector2 position)
    {
        if (_shaderMaterial == null) return;

        // 将世界坐标转换为屏幕UV坐标
        Vector2 screenPosition = GetViewport().GetCanvasTransform().AffineInverse().BasisXform(position);
        Vector2 screenUV = screenPosition / GetViewportRect().Size;

        _shaderMaterial.SetShaderParameter("center_x", screenUV.X);
        _shaderMaterial.SetShaderParameter("center_y", screenUV.Y);

        _isShockwaveActive = true;
        _timeElapsed = 0.0f;
        _currentRadius = 0.0f;
        Visible = true;
    }
}
