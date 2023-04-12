#version 330 core

in vec3 aPosition;
in vec2 aTexCoord;
in vec3 aNormal;

out vec2 texCoord;

uniform mat4 model;
uniform mat4 proj;
uniform mat4 view;

void main() {
	texCoord = aTexCoord;
	
	gl_Position = proj * view * model * vec4(aPosition, 1.0);

}
