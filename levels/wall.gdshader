shader_type canvas_item;

uniform int x_or_y : hint_enum("X", "Y") = 0;

uniform float amp = 20.0;
uniform float freq = 5.0;
uniform float speed = 3.0;

uniform float range : hint_range(0.0, 1.0) = 0.0;
uniform float softness : hint_range(0.01, 0.5) = 0.5;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D noise : repeat_enable, filter_nearest;

uniform float distortion_str : hint_range(-1, 1) = 0.2;

uniform float blur_str : hint_range(0, 5.0) = 2.0;

void vertex() {
	// Called for every vertex the material is visible on.
	if (x_or_y == 0) {
		float wave = sin((VERTEX.x * freq) + (TIME * speed));
		VERTEX.y += wave * amp;
	} else if (x_or_y == 1) {
		float wave = sin((VERTEX.y * freq) + (TIME * speed));
		VERTEX.x += wave * amp;
	}
}

void fragment() {
	// Called for every pixel the material is visible on.
	//COLOR = vec4(1.0, 1.0, 1.0, 1.0);
	float depth = texture(noise, UV * TIME).r;
	vec4 screen_col = textureLod(SCREEN_TEXTURE, (SCREEN_UV) - distortion_str * vec2(depth), blur_str);
	
	COLOR = screen_col + 0.10;

	float center_short = x_or_y == 0 ? abs(UV.y - 0.5) : abs(UV.x - 0.5);
	float mask_short = 1.0 - smoothstep(range, range + softness, center_short);
	
	float mask_long = 1.0;
	if (x_or_y == 0) {
		if (UV.x < 0.1)
			mask_long = smoothstep(0.0, 0.1, UV.x);
		else if (UV.x > 0.9)
			mask_long = 1.0 - smoothstep(0.9, 1.0, UV.x);
	} else {
		if (UV.y < 0.1)
			mask_long = smoothstep(0.0, 0.1, UV.y);
		else if (UV.y > 0.9)
			mask_long = 1.0 - smoothstep(0.9, 1.0, UV.y);
	}
	
	COLOR.a = mask_short * mask_long;// * (1.2 - texture(noise, UV).r);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
