shader_type canvas_item;


// If you intend to read from mipmaps with `textureLod()` LOD values greater than `0.0`,
// use `filter_nearest_mipmap` instead. This shader doesn't require it.
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

uniform float saturation : hint_range(0.0, 1.0, 0.1) = 1.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
        vec3 c = textureLod(screen_texture, SCREEN_UV, 0.0).rgb;
		COLOR.rgb = mix(vec3(dot(vec3(1.0), c.rgb) * 0.33333), c.rgb, saturation);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
