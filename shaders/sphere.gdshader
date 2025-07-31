shader_type spatial;
render_mode unshaded;

varying vec3 world_camera;
varying vec3 world_position;

uniform sampler3D volume_texture;

const int MAX_STEPS = 100;
const float MAX_DIST = 100.0;
const float SURF_DIST = 1e-3;

float GetDist(vec3 p){
	float d = length(p) - .3; //Sphere
	//d = length(vec2(length(p.xz) - .5, p.y)) - .05; //torus
	return d;
}

float RayMarch(vec3 origin, vec3 direction) {
	float dO = 0.0;
	float dS;
	
	for (int i = 0; i < MAX_STEPS; i++)
	{
		vec3 p = origin + dO * direction;
		dS = GetDist(p);
		dO += dS;
		
		if (dS < SURF_DIST || dO > MAX_DIST)
			break;
	}
	return dO;
}

void vertex() {
	world_position = VERTEX;
	world_camera = (inverse(MODELVIEW_MATRIX) * vec4(0, 0, 0, 1)).xyz; //object space
}

void fragment() {
	
	vec3 ro = world_camera;
	vec3 rd =  normalize(world_position - ro);
	
	vec3 col = vec3(1.0);
	
	float d = RayMarch(ro, rd);

	if (d >= MAX_DIST)
		discard;
	
	ALBEDO = col;
}