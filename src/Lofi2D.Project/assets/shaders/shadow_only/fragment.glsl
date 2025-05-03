#version 100

precision mediump float;

// Input vertex attributes (from vertex shader)
varying vec2 fragTexCoord;
varying vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;

// NOTE: Add your custom variables here

uniform vec4 shadowColor;

void main()
{
    float alpha = texture2D(texture0, fragTexCoord).a;

    vec4 shadow = vec4(shadowColor.rgb, alpha);
    shadow = vec4(shadow.rgb, step(1.0, shadow.a) * shadowColor.a);

    gl_FragColor = shadow;
}