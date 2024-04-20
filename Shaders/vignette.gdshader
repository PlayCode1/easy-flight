shader_type canvas_item;
//render_mode unshaded, cull_front;

uniform vec4 color : source_color;
uniform float multiplier = 0.2;
uniform float softness = 3.0;

void vertex() {
//	float value = distance(UV, vec2(0.5));
//	COLOR = vec4(color.rgb, smoothstep(multiplier, softness, value));
//	VERTEX += normalize(VERTEX) * 0.1;
}

void fragment() {
	float value = distance(UV, vec2(0.5));
	COLOR = vec4(color.rgb, smoothstep(multiplier, softness, value));
//	ALBEDO = color.rgb;
}
