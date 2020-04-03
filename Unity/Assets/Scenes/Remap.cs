using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{

    public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    public static List<string> GetPortNames()
    {
        int p = (int)System.Environment.OSVersion.Platform;
        List<string> ports = new List<string>();
        // Are we on Unix?
        if (p == 4 || p == 128 || p == 6)
        {
            string[] ttys = System.IO.Directory.GetFiles("/dev/", "cu.*");
            foreach (string dev in ttys)
            {
                if (dev.StartsWith("/dev/cu.usbmodem"))
                {
                    if (!ports.Contains(dev))
                        ports.Add(dev);
                }
            }
        }


        return ports;
    }
}

