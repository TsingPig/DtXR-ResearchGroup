using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathExtend
{
    public static double GetAtan(float x, float y)
    {
        double radian = Math.Atan2(y, x);
        double radianToAngle = radian / Math.PI * 180;
        return radianToAngle;
    }
}
