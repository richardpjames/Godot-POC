using Godot;
using Godot.Collections;
using System;

public partial class Skeleton : Enemy
{
    // Reference to the player to drive behaviour
    private Player player;
    // The direction to walk
    private Vector2 _direction = Vector2.Zero;
    private Vector2 _facingDirection = Vector2.Zero;
    public override void _Ready()
    {
        // Grab the player based on the "Player" group
        player = (Player)GetTree().GetNodesInGroup("Player")[0];
        // Do standard setup
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Find the direction, based on the player 
        _direction = DetermineDirection();
        // This stores the last direction we were facing before stopping
        if (_direction != Vector2.Zero)
        {
            _facingDirection = _direction;
        }
        // Set our velocity and then move
        Velocity = _direction * _speed;
        MoveAndSlide();
        Animate();
    }

    private Vector2 DetermineDirection()
    {
        // Get access to 2D space
        var spaceState = GetWorld2D().DirectSpaceState;
        // Fire a raycast towards the player to determine if we can see them (2 represents the collision mask)
        PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, player.GlobalPosition, 2);
        // The result holds what we have hit in the query
        Dictionary result = spaceState.IntersectRay(query);
        if (result != null)
        {
            // Get the collider and determine if it is in the player group (was the raycast uninterrupted)
            Node2D collider = (Node2D)result["collider"];
            if (collider.IsInGroup("Player"))
            {
                return (player.GlobalPosition - GlobalPosition).Normalized();
            }
        }
        // If we didn't hit the player then stand still
        return Vector2.Zero;
    }

    private void Animate()
    {
        // If the direction is zero then we are idle
        if (_direction == Vector2.Zero)
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
        // Otherwise we are running
        else
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
    }
}
