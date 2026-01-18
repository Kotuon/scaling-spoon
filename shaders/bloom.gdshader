shader_type canvas_item;

group_uniforms OUTLINE_VALUES;
uniform vec4 color : source_color = vec4(0.0, 0.0, 1.0, 1.0);
uniform float width : hint_range(0.0, 10.0) = 2.0;
uniform int pattern : hint_range(0, 3) = 0;
uniform bool inside = false;
uniform bool add_margins = true;

group_uniforms TEXTURE_COORDINATES;
uniform int hframes = 1;
uniform int vframes = 1;
uniform vec2 idx = vec2(0.5, 0.5);

group_uniforms NOISE_VALUES;
uniform float density = 0.25f;
uniform vec2 speed = vec2(0.02, 0.01);
uniform sampler2D noise : repeat_enable, filter_nearest;


void vertex() {
	// Called for every vertex the material is visible on.
    if (add_margins)
    {
        VERTEX += (UV * 2.0 - 1.0) * width;
    }
}

bool hasContraryNeighbour(vec2 uv, vec2 texture_pixel_size, sampler2D texture) {
	for (float i = -ceil(width); i <= ceil(width); i++) {
		float x = abs(i) > width ? width * sign(i) : i;
		float offset;

		if (pattern == 0) {
			offset = width - abs(x);
		} else if (pattern == 1) {
			offset = floor(sqrt(pow(width + 0.5, 2) - x * x));
		} else if (pattern == 2) {
			offset = width;
		}

		for (float j = -ceil(offset - width); j <= ceil(offset); j++) {
			float y = abs(j) > offset ? offset * sign(j) : j;
			vec2 xy = uv + texture_pixel_size * vec2(x, y);

			if ((xy != clamp(xy, vec2(0.0), vec2(1.0)) || texture(texture, xy).a <= 0.0) == inside) {
				return true;
			}
		}
	}

	return false;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 uv = UV;

	if (add_margins) {
		vec2 texture_pixel_size = vec2(1.0) / (vec2(1.0) / TEXTURE_PIXEL_SIZE + vec2(width * 2.0));

		uv = (uv - texture_pixel_size * width) * TEXTURE_PIXEL_SIZE / texture_pixel_size;

		if (uv != clamp(uv, vec2(0.0), vec2(1.0))) {
			COLOR.a = 0.0;
		} else {
			COLOR = texture(TEXTURE, uv);
		}
	} else {
		COLOR = texture(TEXTURE, uv);
	}

	if ((COLOR.a > 0.0) == inside && hasContraryNeighbour(uv, TEXTURE_PIXEL_SIZE, TEXTURE)) {
		COLOR.rgb = inside ? mix(COLOR.rgb, color.rgb, color.a) : color.rgb;
		COLOR.a += (1.0 - COLOR.a) * (color.a);

        //float noise_value = texture(noise, (UV * vec2(float(hframes), float(vframes))) + speed).r;
        float noise_value = texture(noise, uv * idx).r;
        //float noise_value = texture(noise, uv * vec2(float(hframes), float(vframes))).r;
        float opacity = clamp(noise_value * 2.0 - 1.0, 0.0, 1.0);
//
        COLOR.a *= opacity * density;
	}

}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
