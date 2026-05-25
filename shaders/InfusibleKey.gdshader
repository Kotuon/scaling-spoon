shader_type canvas_item;

uniform vec4 color : source_color;

uniform sampler2D noise : repeat_enable, filter_nearest;

uniform float speed = 0.5f;
uniform float alpha_scale = 1.0f;

float remap(float value, float inputA, float inputB, float outputA, float outputB) {
	return (value - inputA) / (inputB - inputA) * (outputB - outputA) + outputA;
}

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
    COLOR = color;
    
    float dist = distance(UV, vec2(0.5));
    if (dist >= 0.5)
        COLOR.a = 0.0f;
    else {
		vec2 uv = UV * vec2(remap(-cos(TIME * speed), -1.0, 1.0, 0.50, 1.0), 
			remap(-sin(TIME * speed), -1.0, 1.0, 0.50, 1.0));
        COLOR.a = texture(noise, uv).r * alpha_scale;
		
	}
    
    COLOR.a *= 1.0 - smoothstep(0.3, 0.5, dist) + remap(sin(TIME), -1.0, 1.0, -0.550, 0.025);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
