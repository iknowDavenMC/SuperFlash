﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StreakerLibrary;

namespace COMP476Proj
{

    class ParticleSpewer : EntityVisible
    {
        #region Particle structure
        private struct Particle
        {
            public Vector2 position;
            public Vector2 velocity;
            public int scale;
            public int age;
            public int lifespan;
            public Color color;
        }
        #endregion

        #region Members
        List<Particle> particles;
        private int maxParticles;
        private int emitters;
        private float minAngle, maxAngle, angleRange;
        private int minLifespan, maxLifespan;
        // By using HSV instead  of RGB it's easier to vary colours randomly without looking like crap
        private float minHue, maxHue, hueRange;
        private float minSat, maxSat, satRange;
        private float minVal, maxVal, valRange;
        private int size;
        private float speed;
        private bool fade;
        private float fadePercent;
        private Texture2D tex;
        private bool started;
        public bool IsStarted { get { return started; } }
        public bool Absolute = false; // If true, positions and sizes are absolute (ie: not relative to the camera);
        #endregion

        #region Contructor

        /// <summary>
        /// Construct a new particle spewer.
        /// </summary>
        /// <param name="startX">Starting X-Position of the particles</param>
        /// <param name="startY">Starting Y-Position of the particles</param>
        /// <param name="MaxParticles">Maximum number of particles</param>
        /// <param name="Emitters">Number of particle emitters (ie: max new particles per frame)</param>
        /// <param name="MinAngle">Minumum angle to shoot particles (radians)</param>
        /// <param name="MaxAngle">Maximum angle to shoot particles (radians)</param>
        /// <param name="MinLifespan">Minimum lifespan of a particle (milliseconds)</param>
        /// <param name="MaxLifespan">Maximum lifespan of a particle (milliseconds)</param>
        /// <param name="ParticleSize">Particle size in pixels</param>
        /// <param name="ParticleSpeed">Particle speed in pixels/second</param>
        /// <param name="MinHue">Minimum value for a particle's hue (0-360)</param>
        /// <param name="MaxHue">Maximum value for a particle's hue (0-360)</param>
        /// <param name="MinSat">Minimum value for a particle's saturation (0-1)</param>
        /// <param name="MaxSat">Minimum value for a particle's saturation (0-1)</param>
        /// <param name="MinVal">Minimum value for a particle's color value (0-1)</param>
        /// <param name="MaxVal">Maximum value for a particle's color value (0-1)</param>
        /// <param name="FadeOut">Whether or not to fade out at the end</param>
        /// <param name="FadePercent">Percent of the lifespan after which to start fading out (0-1)</param>
        public ParticleSpewer(
            float startX, float startY,
            int MaxParticles, int Emitters,
            float MinAngle, float MaxAngle,
            int MinLifespan, int MaxLifespan,
            int ParticleSize, float ParticleSpeed,
            float MinHue, float MaxHue,
            float MinSat, float MaxSat,
            float MinVal, float MaxVal,
            bool FadeOut, float FadePercent = 0)
        {
            Position = new Vector2(startX, startY);
            maxParticles = MaxParticles;
            emitters = Emitters;
            minAngle = MinAngle;
            maxAngle = MaxAngle;
            angleRange = MaxAngle - minAngle;
            minLifespan = MinLifespan;
            maxLifespan = MaxLifespan;
            if (minLifespan > maxLifespan)
                minLifespan = maxLifespan;
            size = ParticleSize;
            speed = ParticleSpeed;
            minHue = MinHue;
            maxHue = MaxHue;
            if (minHue < 0)
                minHue = 0;
            if (minHue > 360)
                minHue = 360;
            if (maxHue < 0)
                maxHue = 0;
            if (maxHue > 360)
                maxHue = 360;
            hueRange = maxHue - minHue;
            minSat = MinSat;
            maxSat = MaxSat;
            if (minSat < 0)
                minSat = 0;
            if (minSat > 1)
                minSat = 1;
            if (maxSat < 0)
                maxSat = 0;
            if (maxSat > 1)
                maxSat = 1;
            satRange = maxSat - minSat;
            minVal = MinVal;
            maxVal = MaxVal;
            if (minVal < 0)
                minVal = 0;
            if (minVal > 1)
                minVal = 1;
            if (maxVal < 0)
                maxVal = 0;
            if (maxVal > 1)
                maxVal = 1;
            valRange = maxVal - minVal;
            fade = FadeOut;
            fadePercent = FadePercent;
            if (fadePercent < 0)
                fadePercent = 0;
            if (fadePercent > 1)
                fadePercent = 1;
            started = false;

            tex = SpriteDatabase.GetAnimation("blank").Texture;
            particles = new List<Particle>();
        }

        #endregion

        #region Public Methods

        public override void ResolveCollision(Entity other) { }

        public void ChangeColor(int MinHue, int MaxHue, float MinSat, float MaxSat, float MinVal, float MaxVal) {
        minHue = MinHue;
            maxHue = MaxHue;
            if (minHue < 0)
                minHue = 0;
            if (minHue > 360)
                minHue = 360;
            if (maxHue < 0)
                maxHue = 0;
            if (maxHue > 360)
                maxHue = 360;
            hueRange = maxHue - minHue;
            minSat = MinSat;
            maxSat = MaxSat;
            if (minSat < 0)
                minSat = 0;
            if (minSat > 1)
                minSat = 1;
            if (maxSat < 0)
                maxSat = 0;
            if (maxSat > 1)
                maxSat = 1;
            satRange = maxSat - minSat;
            minVal = MinVal;
            maxVal = MaxVal;
            if (minVal < 0)
                minVal = 0;
            if (minVal > 1)
                minVal = 1;
            if (maxVal < 0)
                maxVal = 0;
            if (maxVal > 1)
                maxVal = 1;
            valRange = maxVal - minVal;
        }

        public override void Update(GameTime gameTime)
        {
            int count = particles.Count;
            // If particles are fewer than max and the emitter is on, make new particles
            if (started && count < maxParticles)
            {
                for (int i = 0; i != emitters; ++i)
                {
                    Particle p = new Particle();
                    float angle = (float)Game1.random.NextDouble() * angleRange + minAngle;
                    Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    if (velocity.LengthSquared() > 0)
                        velocity.Normalize();
                    velocity *= speed;
                    int lifespan = Game1.random.Next(minLifespan, maxLifespan);
                    float hue = (float)Game1.random.NextDouble() * hueRange + minHue;
                    float sat = (float)Game1.random.NextDouble() * satRange + minSat;
                    float val = (float)Game1.random.NextDouble() * valRange + minVal;
                    p.position = new Vector2(Position.X, Position.Y);
                    p.velocity = velocity;
                    p.scale = size;
                    p.age = 0;
                    p.lifespan = lifespan;
                    p.color = fromHSV(hue, sat, val);
                    particles.Add(p);
                    count++;
                }
            }

            int time = gameTime.ElapsedGameTime.Milliseconds;

            for (int i = 0; i != count; ++i)
            {
                Particle p = particles[i];
                // Kill old particles
                if (p.age >= p.lifespan)
                {
                    particles.RemoveAt(i);
                    --i;
                    --count;
                }
                else
                {
                    p.age += time;
                    Vector2 dp = p.velocity * (float)time / 1000f;
                    if (fade)
                    {
                        float agePct = (float)p.age / (float)p.lifespan;
                        if (agePct > 1)
                            agePct = 1;
                        if (agePct >= fadePercent)
                        {
                            float fadeDist = 100 - fadePercent;
                            p.color.A = (byte)((255 - (agePct - fadePercent) / (1 - fadePercent)) * 255);
                            dp *= (float)(p.color.A) / 255f;
                        }
                    }
                    p.position += dp;
                    particles[i] = p;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Particle p in particles)
            {
                float scale = 1f;
                Vector2 offset = new Vector2(0, 0);
                if (Absolute)
                {
                    scale = 1f / Camera.Scale;
                    offset = new Vector2(-Camera.Width / 2, -Camera.Height / 2);
                    offset *= scale;
                    offset.X += Camera.X;
                    offset.Y += Camera.Y;
                }
                Vector2 drawPos = new Vector2(p.position.X - p.scale / 2, p.position.Y - p.scale / 2);
                spriteBatch.Draw(tex, drawPos * scale + offset, null, p.color, 0f, Vector2.Zero, p.scale * scale, SpriteEffects.None, 0f);
            }
            base.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Start the particle spewer
        /// </summary>
        public void Start()
        {
            started = true;
        }

        /// <summary>
        /// Stop the particle spewer (extant particles will continue until they die)
        /// </summary>
        public void Stop()
        {
            started = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Convert a colour from HSV to a Color instance
        /// </summary>
        /// <param name="H">Hue</param>
        /// <param name="S">Saturation</param>
        /// <param name="V">Value</param>
        /// <param name="A">Alpha</param>
        /// <returns>The XNA Color instance</returns>
        private Color fromHSV(float H, float S, float V, float A = 1)
        {
            float C = V * S;
            float Hprime = H / 60f;
            float X = C * (1 - Math.Abs(Hprime % 2 - 1));
            float m = V - C;
            if (0 <= Hprime && Hprime < 1)
                return new Color(C + m, X + m, 0 + m, A);
            if (1 <= Hprime && Hprime < 2)
                return new Color(X + m, C + m, 0 + m, A);
            if (2 <= Hprime && Hprime < 3)
                return new Color(0 + m, C + m, X + m, A);
            if (3 <= Hprime && Hprime < 4)
                return new Color(0 + m, X + m, C + m, A);
            if (4 <= Hprime && Hprime < 5)
                return new Color(X + m, 0 + m, C + m, A);
            if (5 <= Hprime && Hprime < 6)
                return new Color(C + m, 0 + m, X + m, A);
            return new Color(0f, 0f, 0f, A);

        }

        #endregion
    }
}
