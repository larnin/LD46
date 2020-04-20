using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MathEx
{
    public static bool Intersect(Vector2 posA, Vector2 posB, Vector2 posC, Vector2 posD, out Vector2 point)
    {
        Vector2 sPos = posB - posA;
        Vector2 sSeg = posD - posC;

        float denom = sPos.x * sSeg.y - sPos.y * sSeg.x;

        if(denom == 0)
        {
            point = Vector2.zero;
            return false;
        }

        float u = (posA.x* sSeg.y - posC.x * sSeg.y - sSeg.x * posA.y + sSeg.x * posC.y)/ denom;
        float v = (-sPos.x * posA.y + sPos.x * posC.y + sPos.y * posA.x - sPos.y * posC.x)/ denom;

        if(u >= -1 && u <= 0 && v >= -1 && v < 0)
        {
            point = posA - sPos * u;
            return true;
        }
        point = Vector2.zero;
        return false;
    }
}
