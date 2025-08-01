shader_type spatial;
render_mode blend_mix;

varying vec3 world_camera;
varying vec3 world_position;

uniform sampler3D volume_texture;
uniform vec3 volume_shape = vec3(1.0);
uniform float density_threshold = 0.1;

// TODO: Scale STEP_SIZE with MAX_STEPS
const int MAX_STEPS = 100;
const float STEP_SIZE = 0.01;

const float MODULATION_FACTOR = 1.0;

void vertex() {
	world_position = VERTEX;
	world_camera = (inverse(MODELVIEW_MATRIX) * vec4(0, 0, 0, 1)).xyz; //object space
}

float findBoxOffset(vec3 ro, vec3 rd) {
	// Ray-box intersection
	vec3 box_min = -0.5 * volume_shape;
	vec3 box_max =  0.5 * volume_shape;

	// Avoid division by zero
	vec3 inv_dir = 1.0 / rd;

	vec3 tmin_tmp = (box_min - ro) * inv_dir;
	vec3 tmax_tmp = (box_max - ro) * inv_dir;

	vec3 tmin = min(tmin_tmp, tmax_tmp);
	vec3 tmax = max(tmin_tmp, tmax_tmp);

	float t0 = max(max(tmin.x, tmin.y), tmin.z);
	float t1 = min(min(tmax.x, tmax.y), tmax.z);

	if (t1 < 0.0 || t0 > t1) {
		discard; // Ray is not going to hit the box
	}

	return max(t0, 0.0);
}

void fragment() {
	vec3 ro = world_camera;
	vec3 rd =  normalize(world_position - ro);

	vec4 final_color = vec4(0.0);
	vec3 pos;

	float t = findBoxOffset(ro, rd);

	for(int i = 0; i < MAX_STEPS; i++) {
		// Gets current object space position of the raymarch
		pos = ro + rd * t;
		t += STEP_SIZE;

		// Turns the raymarching position into texture coordinates [0..1]
		// 0.5 offset because origin is in the middle
		vec3 uvw = (pos / volume_shape) + 0.5;

		// Check if point is outside of the volume texture
		if(any(lessThan(uvw, vec3(0.0))) || any(greaterThan(uvw, vec3(1.0))))
			// No need to further track the ray
			break;

		float density = texture(volume_texture, uvw).r;
		if(density > density_threshold) {
			final_color.rgb += vec3(density); // Set color to white
			final_color.a += density * MODULATION_FACTOR;
		}
		else {
			// Point is not bright enough
			continue;
		}
		if(final_color.a >= 1.0) {
			// alpha cannot get more than that, no need to march further
			break;
		}
	}

	ALBEDO = final_color.rgb;
	ALPHA = final_color.a;
}