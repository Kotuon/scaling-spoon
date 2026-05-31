shader_type canvas_item;

uniform vec4 color : source_color;

uniform sampler2D noise : repeat_enable, filter_nearest;

uniform float speed = 0.5f;
uniform float alpha_scale : hint_range( 0.0, 2.0 ) = 1.0f;
uniform float pixelation_amount : hint_range( 1.0, 100.0 ) = 20.0;

float remap( float value, float inputA, float inputB, float outputA,
             float outputB ) {
    return ( value - inputA ) / ( inputB - inputA ) * ( outputB - outputA ) +
           outputA;
}

void vertex() {
    // Called for every vertex the material is visible on.
}

void fragment() {
    // Called for every pixel the material is visible on.
    COLOR = color;

    vec2 grid_uv = round( UV * pixelation_amount ) / pixelation_amount;

    float dist =
        distance( vec2( grid_uv.x * 1.0, grid_uv.y ), vec2( 0.5, 0.5 ) );

    vec2 uv =
        grid_uv * vec2( remap( -cos( TIME * speed ), -1.0, 1.0, 0.50, 1.0 ),
                        remap( -sin( TIME * speed ), -1.0, 1.0, 0.50, 1.0 ) );

    COLOR.a = texture( noise, uv ).r * alpha_scale;

    COLOR.a *= 1.0 - smoothstep( 0.01, 0.60, dist ) +
               remap( sin( TIME ), -1.0, 1.0, -0.350, 0.0025 );
}

// void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
// }
