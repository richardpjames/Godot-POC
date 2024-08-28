using Godot;

public partial class Player : CharacterBody2D
{
    [Export] private float _speed;
    [Export] private AnimatedSprite2D _sprite;

    public override void _PhysicsProcess(double delta)
    {
        // Get the direction from the inputs
        Vector2 direction = GetDirection();
        // Set the CharacterBody2D velocity to the direction (normalised to avoid fast diagonals) multiplied by the speed
        Velocity = direction * _speed;
        // Use move and slide to take account of delta and velocity
        MoveAndSlide();
        // Determine which animations should be playing
        Animate(direction);
    }

    // Control the 
    private void Animate(Vector2 direction)
    {
        // Determine whether there is any movement in the X axis
        if(direction.X != 0)
        {
            // This will occur if we are running left or right
            _sprite.Play("Run Side");
            _sprite.FlipH = false;
            // If we are running left then flip the sprite
            if(direction.X < 0)
            {
                _sprite.FlipH = true;
            }
        }
        // If there is negative movement in the Y axis we are moving upwards
        else if(direction.Y < 0)
        {
            _sprite.Play("Run Up");
        }
        // If there is positive movement then we are moving downwards
        else if(direction.Y > 0)
        {
            _sprite.Play("Run Down");
        }
        // If there is no movement then play the idle animation
        else
        {
            _sprite.Play("Idle Down");
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
}
