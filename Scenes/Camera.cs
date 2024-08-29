using Godot;
using System;

public partial class Camera : Camera2D
{
    // Noise used for shaking the camera
    private FastNoiseLite _noise;
    [Export] private float _intensity;
    [Export] private float _duration;

    public override void _Ready()
    {
        // Initialise the noise used for camera shake
        _noise = new FastNoiseLite();
    }

    public void Shake()
    {
        // Create a tween to allow us to change over time
        Tween shakeTween = GetTree().CreateTween();
        // Call the shake method, but decrease intensity to zero over the timescale
        shakeTween.TweenMethod(Callable.From((float intensity) => StartShake(intensity)), _intensity, 0, _duration);
    }

    private void StartShake(float intensity)
    {
        // How much we will move the camera
        float cameraOffset = _noise.GetNoise1D(Time.GetTicksMsec()) * intensity;
        // Apply that offset to the camera in both the X and y
        Offset = new Vector2(cameraOffset, cameraOffset);
    }
}
