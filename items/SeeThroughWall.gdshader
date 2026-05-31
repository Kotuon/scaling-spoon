shader_type canvas_item;

uniform vec2 player_pos = vec2( 0, 0 );
uniform float max_distance = 125.0;

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
    if ( COLOR.a > 0.75 ) {
        float distance_to_player = distance( FRAGCOORD.xy, player_pos );
		//float dist = remap(distance_to_player - max_distance, 0.0, max_distance, 0.3, 1.0);
		//COLOR.a = dist;
        COLOR.a = clamp(
            ( ( distance_to_player - max_distance ) / max_distance * 5.0 ), 0.3,
            1 );
    }
}

// void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
// }
