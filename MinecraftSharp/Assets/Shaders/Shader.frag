#version 330 core

in vec2 texCoord;

out vec3 color;

uniform sampler2D tex;
uniform vec3 camPos;

void main() {
	color = vec3(texture(tex, texCoord));
}
