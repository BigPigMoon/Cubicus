#version 330 core

in vec2 texCoord;

out vec4 color;

uniform sampler2D tex;
uniform vec3 camPos;

void main() {
	vec4 texColor = texture(tex, texCoord);

	if (texColor.a < 0.1)
		discard;

	color = texColor;
}
