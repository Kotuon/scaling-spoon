shader_type canvas_item;

uniform vec4 color : source_color = vec4(1.0);
uniform float speed = 0.35f;
uniform sampler2D noise : repeat_enable, filter_nearest;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform float brightness : hint_range(0.0, 2.0) = 1.0;
uniform float contrast : hint_range(0.0, 2.0) = 1.0;
uniform float saturation : hint_range(0.0, 2.0) = 1.0;

float remap(float value, float min_a, float max_a, float min_b, float max_b) {
    return (value - min_a) / (max_a - min_a) * (max_b - min_b) + min_b;
}

float random( vec2 p )
{
   // e^pi (Gelfond's constant)
   // 2^sqrt(2) (Gelfond–Schneider constant)
     vec2 K1 = vec2( 23.14069263277926, 2.665144142690225 );

   //return fract( cos( mod( 12345678., 256. * dot(p,K1) ) ) ); // ver1
   //return fract(cos(dot(p,K1)) * 123456.); // ver2
     return fract(cos(dot(p,K1)) * 12345.6789); // ver3
}

void vertex() {
	// Called for every vertex the material is visible on.
    //VERTEX += 10.0 * vec2(1.0 - random(UV + TIME * speed));
    //VERTEX *= vec2(clamp(random(UV), 0.9, 1.1));
    float speed_c = 10.0;
    vec2 v = VERTEX;
    float max_scale = 2.0f;
    //VERTEX = v * clamp(TIME, 0.0, max_scale);
    //VERTEX = v * (1.0 + (sin(TIME * speed_c) / 10.0));
    //VERTEX += vec2(cos(TIME * speed_c) * adjust, sin(TIME * speed_c) * adjust);
}

void fragment() {
	// Called for every pixel the material is visible on.
    COLOR.rgb = textureLod(screen_texture, SCREEN_UV, 0.0).rgb;
    COLOR.rgb = mix(vec3(0.0), COLOR.rgb, brightness);
    COLOR.rgb = mix(vec3(0.0), COLOR.rgb, contrast);
    COLOR.rgb = mix(vec3(dot(vec3(1.0), COLOR.rgb) * 0.33333), COLOR.rgb, saturation);
    //COLOR.rgb += color.rgb * color.a;
    
    float t_value = clamp(texture(noise, vec2(UV.x + TIME * speed, UV.y)).r * 2.0, 0.0, 1.0);
    
    //COLOR.a = t_value * ((0.3 - 0.1) + 0.1);
    COLOR.a = remap(t_value, 0.0, 1.0, 0.3, 0.7);
    //COLOR.a = 1.0;
    
    COLOR.rgb = color.rgb;
    float dist = distance(vec2(0.5), UV);
    COLOR.a *= 1.0 - smoothstep(0.1, 0.5, dist);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
