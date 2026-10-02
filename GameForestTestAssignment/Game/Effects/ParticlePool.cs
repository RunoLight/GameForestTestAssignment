#region

using System;
using GameForestTestAssignment.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameForestTestAssignment.Game.Effects;

public struct Particle
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Life;
    public float MaxLife;
    public Color Color;
    public float Size;
    public bool Active;
}

public class ParticlePool
{
    private readonly Particle[] _particles;

    public ParticlePool(int maxParticles = 500)
    {
        _particles = new Particle[maxParticles];
        for (var i = 0; i < maxParticles; i++)
            _particles[i] = new Particle { Active = false };
    }

    public void CreateExplosion(Vector2 position, Color color, int count = 15)
    {
        for (var i = 0; i < count; i++)
        {
            var angle = (float)(i / (double)count * Math.PI * 2) + Random.Shared.NextSingle() * 0.3f;
            var speed = 50f + Random.Shared.NextSingle() * 100f;
            AddParticle(position, new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed),
                color, 0.4f + Random.Shared.NextSingle() * 0.3f, 3f + Random.Shared.NextSingle() * 3f);
        }
    }

    public void CreateBigExplosion(Vector2 position, Color color, int count = 40)
    {
        for (var i = 0; i < count; i++)
        {
            var angle = (float)(i / (double)count * Math.PI * 2) + Random.Shared.NextSingle() * 0.3f;
            var speed = 80f + Random.Shared.NextSingle() * 150f;
            AddParticle(position, new Vector2((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed),
                color, 0.6f + Random.Shared.NextSingle() * 0.4f, 4f + Random.Shared.NextSingle() * 4f);
        }
    }

    public void Update(float deltaTimeMs)
    {
        var deltaTime = deltaTimeMs / 1000f;

        for (var i = 0; i < _particles.Length; i++)
        {
            var p = _particles[i];
            if (!p.Active) continue;

            p.Position += p.Velocity * deltaTime;
            p.Life -= deltaTime;

            if (p.Life <= 0)
                _particles[i] = p with { Active = false };
            else
                _particles[i] = p;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var p in _particles)
        {
            if (!p.Active)
                continue;

            var alpha = p.Life / p.MaxLife;
            var size = p.Size * alpha;
            spriteBatch.Draw(PersistentResources.WhitePixel, p.Position, null, p.Color * alpha, 0f,
                Vector2.Zero, size, SpriteEffects.None, 0f);
        }
    }

    private void AddParticle(Vector2 position, Vector2 velocity, Color color, float life, float size)
    {
        for (var i = 0; i < _particles.Length; i++)
            if (!_particles[i].Active)
            {
                _particles[i] = new Particle
                {
                    Position = position,
                    Velocity = velocity,
                    Life = life,
                    MaxLife = life,
                    Color = color,
                    Size = size,
                    Active = true
                };
                return;
            }
    }
}