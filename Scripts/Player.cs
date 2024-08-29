using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    // The speed of the player
    [Export] private float _speed;
    // The direction the player is moving
    private Vector2 _direction = Vector2.Down;
    // The direction that the player is facing
    private Vector2 _facingDirection = Vector2.Down;
    // For playing animations
    [Export] private AnimatedSprite2D _sprite;
    // Determine how long to play the attack animation
    [Export] private float _attackCooldown;
    // The time that attacks can resume
    private float _attackEndTime = 0;
    // Hold the direction of the attack
    private Vector2 _attackDirection = Vector2.Zero;
    // Attack directions - shapes for detecting attack victims
    [Export] private Area2D _attackUp;
    [Export] private Area2D _attackDown;
    [Export] private Area2D _attackLeft;
    [Export] private Area2D _attackRight;

    // For keeping track of the current state of the player
    enum State { IDLE, WALKING, ATTACKING }
    private State _currentState = State.WALKING;

    public override void _PhysicsProcess(double delta)
    {
        // Get the direction from the inputs
        _direction = GetDirection();
        // Set the CharacterBody2D velocity to the direction (normalised to avoid fast diagonals) multiplied by the speed
        Velocity = _direction * _speed;
        // Use move and slide to take account of delta and velocity
        MoveAndSlide();
        // Deal with any attacking first - assuming the previous attack has completed
        if (Input.IsActionJustPressed("Attack") && _currentState != State.ATTACKING)
        {
            Attack();
        }
        // If the attack end time has complete then reset current state based on movement
        if (Time.GetTicksMsec() > _attackEndTime)
        {
            if (_direction != Vector2.Zero)
            {
                // If there is no attack, but movement then we are walking
                _currentState = State.WALKING;
                // If we are walking then update the facing direction
                _facingDirection = _direction;
            }
            else
            {
                // Otherwise we are idle (no walking or attack)
                _currentState = State.IDLE;
            }
        }
        // Determine which animations should be playing
        Animate();
    }

    // Control the 
    private void Animate()
    {
        // Deal with attacking animations
        if (_currentState == State.ATTACKING)
        {
            // Determine whether there is any movement in the X axis
            if (_attackDirection.X != 0)
            {
                // This will occur if we are attacking left or right
                _sprite.Play("Attack Side");
                _sprite.FlipH = false;
                // If we are attacking left then flip the sprite
                if (_attackDirection.X < 0)
                {
                    _sprite.FlipH = true;
                }
            }
            // If there is negative movement in the Y axis we are moving upwards
            else if (_attackDirection.Y < 0)
            {
                _sprite.Play("Attack Up");
            }
            // If there is positive movement then we are moving downwards
            else if (_attackDirection.Y > 0)
            {
                _sprite.Play("Attack Down");
            }
            // If there is no movement then play the attack down animation
            else
            {
                _sprite.Play("Attack Down");
            }
        }
        // Deal with walking/running animations
        if (_currentState == State.WALKING)
        {
            // Determine whether there is any movement in the X axis
            if (_direction.X != 0)
            {
                // This will occur if we are running left or right
                _sprite.Play("Run Side");
                _sprite.FlipH = false;
                // If we are running left then flip the sprite
                if (_direction.X < 0)
                {
                    _sprite.FlipH = true;
                }
            }
            // If there is negative movement in the Y axis we are moving upwards
            else if (_direction.Y < 0)
            {
                _sprite.Play("Run Up");
            }
            // If there is positive movement then we are moving downwards
            else if (_direction.Y > 0)
            {
                _sprite.Play("Run Down");
            }
        }
        // Deal with Idle animations
        if (_currentState == State.IDLE)
        {
            // Determine whether there is any movement in the X axis
            if (_facingDirection.X != 0)
            {
                // This will occur if we are running left or right
                _sprite.Play("Idle Side");
                _sprite.FlipH = false;
                // If we are running left then flip the sprite
                if (_facingDirection.X < 0)
                {
                    _sprite.FlipH = true;
                }
            }
            // If there is negative movement in the Y axis we are moving upwards
            else if (_facingDirection.Y < 0)
            {
                _sprite.Play("Idle Up");
            }
            // If there is positive movement then we are moving downwards
            else if (_facingDirection.Y > 0)
            {
                _sprite.Play("Idle Down");
            }
        }
    }

    // Take the inputs and return a direction as a normalised vector
    private Vector2 GetDirection()
    {
        Vector2 direction = Vector2.Zero;

        // Deal with input to set the velocity based on speed
        if (Input.IsActionPressed("Left"))
        {
            // Speed in the negative direction
            direction.X = -1;
        }
        if (Input.IsActionPressed("Right"))
        {
            // Speed in the positive direction
            direction.X = 1;
        }
        if (Input.IsActionPressed("Up"))
        {
            // Speed in the negative direction
            direction.Y = -1;
        }
        if (Input.IsActionPressed("Down"))
        {
            // Speed in the positive direction
            direction.Y = 1;
        }

        return direction.Normalized();
    }

    private void Attack()
    {
        // Set the end time for attacking
        _attackEndTime = Time.GetTicksMsec() + (_attackCooldown * 1000);
        // Set our current state for animation
        _currentState = State.ATTACKING;
        // Set the attack direction for animation
        _attackDirection = _facingDirection;

        Array<Node2D> bodies = new Array<Node2D>();
        // Determine which box to use and then get overlapping bodies
        if (_attackDirection == Vector2.Up)
        {
            bodies = _attackUp.GetOverlappingBodies();
        }
        else if (_attackDirection == Vector2.Down)
        {
            bodies = _attackDown.GetOverlappingBodies();
        }
        else if (_attackDirection == Vector2.Left)
        {
            bodies = _attackLeft.GetOverlappingBodies();
        }
        else if (_attackDirection == Vector2.Right)
        {
            bodies = _attackRight.GetOverlappingBodies();
        }

        // If we hit anything then cycle through and deal damage where we can
        for (int i = 0; i < bodies.Count; i++)
        {
            // Check if the thing we have hit is a chicken, then cast and give damage
            if (bodies[i] is Enemy)
            {
                Enemy enemy = (Enemy) bodies[i];
                enemy.TakeDamage(1);
            }
        }
    }
}
