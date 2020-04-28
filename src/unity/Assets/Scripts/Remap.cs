using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
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

    public static bool CheckKey(string key)
    {
        int a, b, c, d;
        string[] _key;
        _key = key.Split('-');
        if (_key.Length == 4)
        {
            a = int.Parse((_key[0]).Replace(" ", ""));
            b = int.Parse((_key[1]).Replace(" ", ""));
            c = int.Parse((_key[2]).Replace(" ", ""));
            d = int.Parse((_key[3]).Replace(" ", ""));
            bool result = ((a * b) + (c / d)) + a * d == 1992;
            return result;
        }
        else return false;
        //Debug.Log(result + key);

    }
 
}

