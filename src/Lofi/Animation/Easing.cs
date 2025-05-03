/**************************************************************************/
/*  EasingEquations.cs                                                    */
/**************************************************************************/
/*                         This file is based on:                         */
/*                             GODOT ENGINE                               */
/*                        https://godotengine.org                         */
/*                                                                        */
/* Original work derived from Robert Penner's easing equations:           */
/* http://robertpenner.com/easing/                                         */
/*                                                                        */
/* Adapted and translated to C# with the assistance of ChatGPT,           */
/* based on the original Godot Engine source code.                        */
/*                                                                        */
/* Copyright (c) 2001 Robert Penner                                        */
/* Copyright (c) 2014-present Godot Engine contributors (see AUTHORS.md)  */
/* Copyright (c) 2007-2014 Juan Linietsky, Ariel Manzur                    */
/*                                                                        */
/* Permission is hereby granted, free of charge, to any person obtaining  */
/* a copy of this software and associated documentation files (the        */
/* "Software"), to deal in the Software without restriction, including    */
/* without limitation the rights to use, copy, modify, merge, publish,    */
/* distribute, sublicense, and/or sell copies of the Software, and to     */
/* permit persons to whom the Software is furnished to do so, subject to  */
/* the following conditions:                                              */
/*                                                                        */
/* The above copyright notice and this permission notice shall be         */
/* included in all copies or substantial portions of the Software.        */
/*                                                                        */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. */
/* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE      */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 */
/**************************************************************************/

namespace Lofi.Animation;

public static class Easing
{
    /// <summary>
    /// Delegate for easing interpolation functions.
    /// </summary>
    /// <param name="t">Current time (in seconds or frames), typically ranging from 0 to d.</param>
    /// <param name="b">Starting value (initial position or value at the beginning of the animation).</param>
    /// <param name="c">Change in value (end value minus start value).</param>
    /// <param name="d">Total duration of the animation (in seconds or frames).</param>
    /// <returns>Interpolated value at time t.</returns>
    public delegate float EasingEquation(float t, float b, float c, float d);

    public enum TransitionType
    {
        Linear,
        Sine,
        Quint,
        Quart,
        Quad,
        Expo,
        Elastic,
        Cubic,
        Circ,
        Bounce,
        Back,
        Spring,
    }

    public enum EaseType
    {
        In,
        Out,
        InOut,
        OutIn,
    }
    
    private static readonly EasingEquation[,] EasingEquations =
    {
        { Linear.In, Linear.In, Linear.In, Linear.In }, // Linear is the same for each easing.
        { Sine.In, Sine.Out, Sine.InOut, Sine.OutIn },
        { Quint.In, Quint.Out, Quint.InOut, Quint.OutIn },
        { Quart.In, Quart.Out, Quart.InOut, Quart.OutIn },
        { Quad.In, Quad.Out, Quad.InOut, Quad.OutIn },
        { Expo.In, Expo.Out, Expo.InOut, Expo.OutIn },
        { Elastic.In, Elastic.Out, Elastic.InOut, Elastic.OutIn },
        { Cubic.In, Cubic.Out, Cubic.InOut, Cubic.OutIn },
        { Circ.In, Circ.Out, Circ.InOut, Circ.OutIn },
        { Bounce.In, Bounce.Out, Bounce.InOut, Bounce.OutIn },
        { Back.In, Back.Out, Back.InOut, Back.OutIn },
        { Spring.In, Spring.Out, Spring.InOut, Spring.OutIn }
    };

    public static float Ease(TransitionType transitionType, EaseType easeType, float t, float b, float c, float d)
    {
        var func = EasingEquations[(int)transitionType, (int)easeType];
        return func(t, b, c, d);
    }
    
    public static class Linear
    {
        public static float In(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
    }

    public static class Sine
    {
        public static float In(float t, float b, float c, float d)
        {
            return -c * (float)System.Math.Cos(t / d * (System.Math.PI / 2)) + c + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            return c * (float)System.Math.Sin(t / d * (System.Math.PI / 2)) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            return -c / 2 * ((float)System.Math.Cos(System.Math.PI * t / d) - 1) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Quint
    {
        public static float In(float t, float b, float c, float d)
        {
            return c * (float)System.Math.Pow(t / d, 5) + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            return c * ((float)System.Math.Pow(t / d - 1, 5) + 1) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            t = t / d * 2;
            if (t < 1)
                return c / 2 * (float)System.Math.Pow(t, 5) + b;
            return c / 2 * ((float)System.Math.Pow(t - 2, 5) + 2) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Quart
    {
        public static float In(float t, float b, float c, float d)
        {
            return c * (float)System.Math.Pow(t / d, 4) + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            return -c * ((float)System.Math.Pow(t / d - 1, 4) - 1) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            t = t / d * 2;
            if (t < 1)
                return c / 2 * (float)System.Math.Pow(t, 4) + b;
            return -c / 2 * ((float)System.Math.Pow(t - 2, 4) - 2) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Quad
    {
        public static float In(float t, float b, float c, float d)
        {
            return c * (float)System.Math.Pow(t / d, 2) + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            t /= d;
            return -c * t * (t - 2) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            t = t / d * 2;
            if (t < 1)
                return c / 2 * (float)System.Math.Pow(t, 2) + b;
            return -c / 2 * ((t - 1) * (t - 3) - 1) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Expo
    {
        public static float In(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            return c * (float)System.Math.Pow(2, 10 * (t / d - 1)) + b - c * 0.001f;
        }

        public static float Out(float t, float b, float c, float d)
        {
            if (t == d) return b + c;
            return c * 1.001f * (-(float)System.Math.Pow(2, -10 * t / d) + 1) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            if (t == d) return b + c;

            t = t / d * 2;
            if (t < 1)
                return c / 2 * (float)System.Math.Pow(2, 10 * (t - 1)) + b - c * 0.0005f;
            return c / 2 * 1.0005f * (-(float)System.Math.Pow(2, -10 * (t - 1)) + 2) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Elastic
    {
        public static float In(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            t /= d;
            if (t == 1) return b + c;

            t -= 1;
            float p = d * 0.3f;
            float a = c * (float)System.Math.Pow(2, 10 * t);
            float s = p / 4;
            return -(a * (float)System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p)) + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            t /= d;
            if (t == 1) return b + c;

            float p = d * 0.3f;
            float s = p / 4;
            return c * (float)System.Math.Pow(2, -10 * t) *
                (float)System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p) + c + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            t /= d / 2;
            if (t == 2) return b + c;

            float p = d * (0.3f * 1.5f);
            float s = p / 4;
            if (t < 1)
            {
                t -= 1;
                return -0.5f * (c * (float)System.Math.Pow(2, 10 * t) *
                                (float)System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p)) + b;
            }

            t -= 1;
            return c * (float)System.Math.Pow(2, -10 * t) *
                (float)System.Math.Sin((t * d - s) * (2 * System.Math.PI) / p) * 0.5f + c + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Cubic
    {
        public static float In(float t, float b, float c, float d)
        {
            t /= d;
            return c * t * t * t + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            t = t / d - 1;
            return c * (t * t * t + 1) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1)
                return c / 2 * t * t * t + b;
            t -= 2;
            return c / 2 * (t * t * t + 2) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Circ
    {
        public static float In(float t, float b, float c, float d)
        {
            t /= d;
            return -c * ((float)System.Math.Sqrt(1 - t * t) - 1) + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            t = t / d - 1;
            return c * (float)System.Math.Sqrt(1 - t * t) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            t /= d / 2;
            if (t < 1)
                return -c / 2 * ((float)System.Math.Sqrt(1 - t * t) - 1) + b;
            t -= 2;
            return c / 2 * ((float)System.Math.Sqrt(1 - t * t) + 1) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Bounce
    {
        public static float Out(float t, float b, float c, float d)
        {
            t /= d;
            if (t < (1 / 2.75f))
                return c * (7.5625f * t * t) + b;
            if (t < (2 / 2.75f))
            {
                t -= 1.5f / 2.75f;
                return c * (7.5625f * t * t + 0.75f) + b;
            }

            if (t < (2.5f / 2.75f))
            {
                t -= 2.25f / 2.75f;
                return c * (7.5625f * t * t + 0.9375f) + b;
            }

            t -= 2.625f / 2.75f;
            return c * (7.5625f * t * t + 0.984375f) + b;
        }

        public static float In(float t, float b, float c, float d)
        {
            return c - Out(d - t, 0, c, d) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return In(t * 2, b, c / 2, d);
            float h = c / 2;
            return Out(t * 2 - d, b + h, h, d);
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Back
    {
        public static float In(float t, float b, float c, float d)
        {
            const float s = 1.70158f;
            t /= d;
            return c * t * t * ((s + 1) * t - s) + b;
        }

        public static float Out(float t, float b, float c, float d)
        {
            const float s = 1.70158f;
            t = t / d - 1;
            return c * (t * t * ((s + 1) * t + s) + 1) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            const float s = 1.70158f * 1.525f;
            t /= d / 2;
            if (t < 1)
                return c / 2 * (t * t * ((s + 1) * t - s)) + b;
            t -= 2;
            return c / 2 * (t * t * ((s + 1) * t + s) + 2) + b;
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }

    public static class Spring
    {
        public static float Out(float t, float b, float c, float d)
        {
            t /= d;
            float s = 1.0f - t;
            t = ((float)System.Math.Sin(t * System.Math.PI * (0.2 + 2.5 * t * t * t)) * (float)System.Math.Pow(s, 2.2) +
                 t) *
                (1.0f + (1.2f * s));
            return c * t + b;
        }

        public static float In(float t, float b, float c, float d)
        {
            return c - Out(d - t, 0, c, d) + b;
        }

        public static float InOut(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return In(t * 2, b, c / 2, d);
            float h = c / 2;
            return Out(t * 2 - d, b + h, h, d);
        }

        public static float OutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return Out(t * 2, b, c / 2, d);
            float h = c / 2;
            return In(t * 2 - d, b + h, h, d);
        }
    }
}