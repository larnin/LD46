using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CameraTargetChangeEvent
{
    public float x;
    public float y;

    public CameraTargetChangeEvent(float _x, float _y)
    {
        x = _x;
        y = _y;
    }
}
