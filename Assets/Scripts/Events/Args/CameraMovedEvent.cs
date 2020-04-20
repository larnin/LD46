using UnityEngine;
using System.Collections;

public class CameraMovedEvent
{
    public float x;
    public float y;

    public CameraMovedEvent(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
}
