using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class Enemy : CharacterBody2D
{    // The speed of the enemy
    [Export] protected float _speed;
    // Get access to the sprite for flipping
    [Export] protected AnimatedSprite2D _sprite;
    // The amount of health
    [Export] protected int _maxHealth;
    protected int _currentHealth;
    // Hold the start position
    private Vector2 _startPosition;
    // Get access to the particles for blood
    [Export] GpuParticles2D _bloodParticles;

    public override void _Ready()
    {
        // Initialise the health
        _currentHealth = _maxHealth;
        // Store the start position of the enemy (to avoid wandering too far)
        _startPosition = GlobalPosition;
    }
    public void TakeDamage(int damage = 0)
    {
        // If we have specified particles
        if (_bloodParticles != null)
        {
            _bloodParticles.Emitting = true;
        }
        // Reduce the amount of health
        _currentHealth -= damage;
        if (_currentHealth < 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
    }
}

