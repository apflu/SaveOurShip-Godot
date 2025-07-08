shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform float center_x = 0.5; // 冲击波中心X (0.0-1.0)
uniform float center_y = 0.5; // 冲击波中心Y (0.0-1.0)
uniform float radius : hint_range(0.0, 1.0) = 0.0; // 冲击波半径
uniform float strength : hint_range(0.0, 0.1) = 0.05; // 冲击波强度
uniform float speed = 5.0; // 冲击波扩散速度

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
    // 计算当前像素相对于冲击波中心的向量
    vec2 center = vec2(center_x, center_y);
    vec2 uv = SCREEN_UV;
    vec2 diff = uv - center;

    // 计算距离中心点的距离
    float dist = length(diff);

    // 计算冲击波效果的强度衰减
    float shockwave_effect = max(0.0, 1.0 - abs(dist - radius) / strength);

    // 计算扭曲方向（从中心点向外）
    vec2 normalized_diff = normalize(diff);

    // 应用扭曲
    vec2 offset = normalized_diff * shockwave_effect * strength * 0.1; // 调整0.1来控制整体扭曲量
    COLOR = texture(SCREEN_TEXTURE, SCREEN_UV + offset);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
