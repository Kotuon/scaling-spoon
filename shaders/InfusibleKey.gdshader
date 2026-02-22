shader_type canvas_item;

uniform vec4 color : source_color;

uniform sampler2D noise : repeat_enable, filter_nearest;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
    COLOR = color;
    
    float dist = distance(UV, vec2(0.5));
    if (dist >= 0.5)
        COLOR.a = 0.0f;
    else
        COLOR.a = texture(noise, UV).r;
    
    COLOR.a *= 1.0 - smoothstep(0.3, 0.5, dist);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
