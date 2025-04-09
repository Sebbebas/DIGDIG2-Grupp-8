// Vertex Shader
#version 330 core
layout(location = 0) in vec3 position;
layout(location = 1) in vec2 texCoord;

out vec2 TexCoord;

void main()
{
    gl_Position = vec4(position, 1.0);
    TexCoord = texCoord;
}

// Fragment Shader
#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform vec3 lightPos;
uniform vec3 lightColor;
uniform float fogDensity;

float noise(vec3 p) {
    // Simple noise function
    return fract(sin(dot(p, vec3(12.9898, 78.233, 37.719))) * 43758.5453);
}

vec3 rayMarch(vec3 ro, vec3 rd) {
    vec3 col = vec3(0.0);
    float totalAlpha = 0.0;
    for (int i = 0; i < 100; i++) {
        vec3 p = ro + rd * float(i) * 0.1;
        float density = noise(p) * fogDensity;
        float alpha = density * 0.1;
        vec3 lightDir = normalize(lightPos - p);
        float lightDist = length(lightPos - p);
        vec3 light = lightColor * (1.0 / (1.0 + lightDist * lightDist));
        col += vec3(1.0 - totalAlpha) * alpha * light;
        totalAlpha += alpha;
        if (totalAlpha >= 1.0) break;
    }
    return col;
}

void main()
{
    vec3 ro = vec3(0.0, 0.0, -5.0); // Camera position
    vec3 rd = normalize(vec3(TexCoord, 1.0)); // Ray direction
    vec3 col = rayMarch(ro, rd);
    FragColor = vec4(col, 1.0);
}
