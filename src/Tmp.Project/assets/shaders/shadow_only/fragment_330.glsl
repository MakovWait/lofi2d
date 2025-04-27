#version 330 core

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Output fragment color
out vec4 FragColor;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 shadowColor;

void main()
{
    float alpha = texture(texture0, fragTexCoord).a;

    vec4 shadow = vec4(shadowColor.rgb, alpha);
    shadow = vec4(shadow.rgb, step(1.0, shadow.a) * shadowColor.a);

    FragColor = shadow;
}