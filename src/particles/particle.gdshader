shader_type canvas_item;
//render_mode blend_mix;

uniform float fade_width = 0.2;
uniform float speed = 1.0f;
uniform sampler2D noise : repeat_enable, filter_nearest;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
    //COLOR = vec4(1.0);
    float noise_value = texture(noise, UV + TIME * speed).r;
    float opacity = clamp(noise_value * 2.0 - 1.0, 0.0, 1.0);
    COLOR.a *= opacity;

    float dist_from_center = distance(UV, vec2(0.5));

    float amount = 0.6;

    if (dist_from_center > amount)
        COLOR.a = 0.0;
    else
        COLOR.a *= 1.0 - smoothstep(amount - fade_width, amount, dist_from_center);
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
