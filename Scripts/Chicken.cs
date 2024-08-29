using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Chicken : Enemy
{
    private Vector2 _direction;
    private List<Vector2> _raycastDirections;
    // Determine how often we will re-evaluate the directions we can travel
    [Export] private float _evaluationTime;
    // How far the chicken can see
    [Export] private float _viewDistance;
    // How many raycasts we will check
    [Export] private int _viewResolution;
    // Determine when we can next evaluate (for performance)
    private float _nextTimeToEvaluate = 0;

    public override void _Ready()
    {
        // Set up the list of directions to test with our raycasts
        _raycastDirections = new List<Vector2>();
        for (float rotation = 0; rotation < Math.PI * 2; rotation += ((float)Math.PI * 2) / _viewResolution)
        {
            _raycastDirections.Add(Vector2.Up.Rotated(rotation));
        }
        // Set initial evaluation time somewhat randomly
        Random rand = new Random();
        _nextTimeToEvaluate = Time.GetTicksMsec() + ((float)rand.NextDouble() * _evaluationTime * 1000);
        // Call the method in the Enemy class
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Determine the direction for the chicken if we are at time to reevaluate
        if (Time.GetTicksMsec() > _nextTimeToEvaluate)
        {
            _direction = DetermineDirection();
            // By default keep the sprite facing the right way
            _sprite.FlipH = false;
            // If we are heading in the right direction then flip the sprite
            if (_direction.X > 0)
            {
                _sprite.FlipH = true;
            }
        }
        // Set our velocity and then move
        Velocity = _direction * _speed;
        MoveAndSlide();
    }

    private Vector2 DetermineDirection()
    {
        // Determine when we should next evaluate
        _nextTimeToEvaluate = Time.GetTicksMsec() + (_evaluationTime * 1000);
        // Local variables for storing results of our evaluation
        Vector2 chosenDirection = Vector2.Zero;
        float maxDistance = 0;
        // Get access to 2D space
        var spaceState = GetWorld2D().DirectSpaceState;
        // Don't always evaluate the list in the same order
        Random rand = new Random();
        // Calls a function on each item which generates a random number, then sorts and converts back to a list
        _raycastDirections = _raycastDirections.OrderBy(_ => (rand.Next())).ToList();
        foreach (Vector2 direction in _raycastDirections)
        {
            // We are going to fire our raycast from the global position of the chicken, then expand out to the direction times the length
            PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + (direction * _viewDistance));
            // The result holds what we have hit in the query
            Dictionary result = spaceState.IntersectRay(query);
            // If we don't hit anything then just return as this is the most clear direction
            if (result.Count == 0)
                return direction;
            // Otherwise determine the distance to the object hit
            float distanceToObject = GlobalPosition.DistanceTo((Vector2)result["position"]);
            // If this is our furthest distance so far, then this is the chosen direction
            if (distanceToObject > maxDistance)
            {
                // Store the new distance and set this to the chosen direction
                maxDistance = distanceToObject;
                // In a tie this will be random as we randomised the list
                chosenDirection = direction;
            }
        }
        // Return the eventually chosen direction
        return chosenDirection;
    }

    protected override void Die()
    {
        return;
    }
}
