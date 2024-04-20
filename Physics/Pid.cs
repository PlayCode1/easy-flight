using Godot;

namespace App.Physics;

public partial class Pid : RefCounted
{
    private readonly float _p;
    private readonly float _i;
    private readonly float _d;

    private Vector3 _prevError;
    private Vector3 _errorIntegral;


    public Pid(float p, float i, float d)
    {
        _p = p;
        _i = i;
        _d = d;
    }

    public Vector3 Update(Vector3 error, float delta)
    {
        _errorIntegral += error * delta;
        var errorDerivative = (error - _prevError) / delta;
        _prevError = error;

        return _p * error + _i * _errorIntegral + _d * errorDerivative;
    }
}