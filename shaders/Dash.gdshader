shader_type particles;
render_mode disable_velocity;

group_uniforms Direction;
uniform vec2 direction;
uniform float spread;

group_uniforms Velocity;
uniform float initial_velocity_min;
uniform float initial_velocity_max;

group_uniforms Scale;
uniform float initial_scale_min;
uniform float initial_scale_max;

void process() {
}
