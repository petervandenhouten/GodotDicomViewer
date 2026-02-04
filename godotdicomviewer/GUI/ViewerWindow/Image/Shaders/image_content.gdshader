shader_type canvas_item;

// INVERT
//void fragment() {
//    vec4 tex_color = texture(TEXTURE, UV);
//    tex_color.rgb = 1.0 - tex_color.rgb;
//    COLOR = tex_color;
//}

// Grey scale
//void fragment() {
//    vec4 tex = texture(TEXTURE, UV);
//    float gray = dot(tex.rgb, vec3(0.299, 0.587, 0.114));
//    COLOR = vec4(vec3(gray), tex.a);
//}

uniform vec4 tint_color : source_color = vec4(1.0, 0.0, 0.0, 1.0);


void fragment() {
    vec4 tex = texture(TEXTURE, UV);
    COLOR = tex * tint_color;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
