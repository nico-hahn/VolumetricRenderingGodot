shader_type spatial;
render_mode blend_mix;

uniform sampler3D volume_texture;
uniform vec3 volume_shape = vec3(1.0);
uniform float density_threshold = 0.1;
uniform float modulation_factor = 1.0;
uniform float max_steps = 256.0;

varying vec3 world_position;
varying mat4 model_matrix;

void vertex() {
    world_position = VERTEX;
	model_matrix = MODEL_MATRIX;
}

// Converts the camera position into object space
vec3 get_camera_pos_object_space(vec3 campos) {
    mat3 rot = mat3(model_matrix);
    vec3 translation = model_matrix[3].xyz;
    mat3 inv_rot = transpose(rot);
    return inv_rot * (campos - translation);
}

float findBoxOffset(vec3 ro, vec3 rd) {
    vec3 box_min = -0.5 * volume_shape;
    vec3 box_max =  0.5 * volume_shape;

    vec3 inv_dir = 1.0 / rd;
    vec3 tmin_tmp = (box_min - ro) * inv_dir;
    vec3 tmax_tmp = (box_max - ro) * inv_dir;

    vec3 tmin = min(tmin_tmp, tmax_tmp);
    vec3 tmax = max(tmin_tmp, tmax_tmp);

    float t0 = max(max(tmin.x, tmin.y), tmin.z);
    float t1 = min(min(tmax.x, tmax.y), tmax.z);

    if (t1 < 0.0 || t0 > t1) discard;

    return max(t0, 0.0);
}

void fragment() {
	/*
	  Apparently, CAMERA_POSITION_WORLD is the correct one for an XR
	  environment because it takes the eye offset into account. 
	*/
    vec3 ro = get_camera_pos_object_space(CAMERA_POSITION_WORLD);
    vec3 rd = normalize(world_position - ro);

    vec4 final_color = vec4(0.0);
    vec3 pos;

    float step_size = 1.0 / max_steps;
    float t = findBoxOffset(ro, rd);

    for (int i = 0; i < int(max_steps); i++) {
    	// Gets current object space position of the raymarch
        pos = ro + rd * t;
        t += step_size;

		// Turns the raymarching position into texture coordinates [0..1]
		// 0.5 offset because origin is in the middle
        vec3 uvw = (pos / volume_shape) + 0.5;

		// Check if point is outside of the volume texture
        if (any(lessThan(uvw, vec3(0.0))) || any(greaterThan(uvw, vec3(1.0))))
            break;

        float density = texture(volume_texture, uvw).r;
        if (density > density_threshold) {
        	// I think, this could depend on step size -> otherwise an area will
            // 'light up' faster, the more often it is sampled. But idk how.
            final_color.rgb += vec3(density);
            final_color.a += density * modulation_factor;
        }

        if (final_color.a >= 1.0)
            break;
    }

    ALBEDO = final_color.rgb;
    ALPHA = final_color.a;
}
