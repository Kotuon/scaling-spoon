shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform sampler2D noise_1 : repeat_enable;
uniform sampler2D noise_2 : repeat_enable;

uniform vec2 scroll_1 = vec2(0.05, 0.05);
uniform vec2 scroll_2 = vec2(-0.05, -0.05);

uniform float distortion_strength : hint_range(-1,1) = 0.2;
uniform vec4 tint_color : source_color;

uniform vec4 top_color : source_color;
uniform float light_start : hint_range(0.0, 1.0) = 0.275;
uniform float light_end : hint_range(0.0, 1.0) = 0.4;

uniform sampler2D gradiant;
uniform sampler2D flash_value;

uniform bool is_running = false;
uniform float input_value : hint_range(0.0, 1.0) = 0.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
    vec4 grad_color = texture(gradiant, vec2(input_value, 0.5));
    vec4 flash_color = texture(flash_value, vec2(input_value, 0.5));
    if (is_running) {
        float depth = texture(noise_1, UV + scroll_1 * TIME).r * texture(noise_2, UV + scroll_2 * TIME).r;
        vec4 screen_col = texture(SCREEN_TEXTURE, SCREEN_UV + distortion_strength * vec2(depth)) * 0.8;
        vec4 top_light = smoothstep(light_start, light_end, depth) * top_color * (flash_color.a * 2.0);

        COLOR = screen_col * tint_color + top_light;// * tint_color + top_light;
        ///COLOR.rgb += COLOR.rgb * (1.0 - input_value);

        float dist = distance(UV, vec2(0.5,0.5));

        if (dist > 0.5) {
            COLOR.a = 0.0;
        }
        else if (dist < 0.4999) {
            COLOR.a = grad_color.a * (1.0 - (dist / 0.5));
        }
        else {
            COLOR.a = grad_color.a * (1.0 - (dist / 0.5));
        }
    }
    else {
        COLOR.a = 0.0;
    }
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
