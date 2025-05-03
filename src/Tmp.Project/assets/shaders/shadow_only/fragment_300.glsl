#version 300 es
precision mediump float;

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Output fragment color
out vec4 outColor;

// Input uniform values
uniform sampler2D texture0;

// NOTE: Add your custom variables here
uniform vec4 shadowColor;

void main()
{
    float alpha = texture(texture0, fragTexCoord).a;

    vec4 shadow = vec4(shadowColor.rgb, alpha);
    shadow = vec4(shadow.rgb, step(1.0, shadow.a) * shadowColor.a);

    outColor = shadow;
}