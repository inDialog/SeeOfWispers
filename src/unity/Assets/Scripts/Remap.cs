using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using System;
using AsImpL;
using UnityEngine.UI;
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
        int a, b, c, d, f, g;
        string[] _key;
        _key = key.Split('-');
        if (_key.Length == 6)
        {
            a = int.Parse((_key[0]).Replace(" ", ""));
            b = int.Parse((_key[1]).Replace(" ", ""));
            c = int.Parse((_key[2]).Replace(" ", ""));
            d = int.Parse((_key[3]).Replace(" ", ""));
            f = int.Parse((_key[4]).Replace(" ", ""));
            g = int.Parse((_key[5]).Replace(" ", ""));
            var floor = ((a * b) + (c / d)) + a * d;
            return floor == 1992 & g + f > 92;
        }
        else return false;
    }
    public static string ComposeString(TMP_InputField[] inputFields)
    {
        return inputFields[0].text + "\n" + inputFields[1].text + "\n" + inputFields[2].text + "\n" + inputFields[3].text + "\n" + inputFields[4].text;
    }

    public static bool CheckURl(string url)
    {
        if (url == "")
            return false;
        if (url.Substring(url.Length - 3, 3) != "obj")
            return false;
        if (!url.Contains("http"))
            return false;
        Uri uriResult;
        return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    public static bool[] ConcertToBool(string input)
    {
        string[] temp = input.Split('/');
        bool[] output = new bool[temp.Length - 1];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = (temp[i] == "True");
        }
        return output;
    }
    public static Color32 RandomColor()
    {
        return new Color32(
        (byte)UnityEngine.Random.Range(0, 200),
        (byte)UnityEngine.Random.Range(0, 200),
        (byte)UnityEngine.Random.Range(0, 200),
        225);
    }
    public static string StringToCollor(Color32 Color)
    {
        return
        Color.r + "\t" +
        Color.g + "\t" +
        Color.b;
    }

    public static Vector3 Trip (Vector3 Original, Vector3 target,Transform space)
    {
       return  space.InverseTransformPoint(Original) -target;

    }
    public static void ConvertConvexObjects(Dictionary<string, InfoArtwork> infoArwork, out List<string> converted)
    {
        converted = new List<string>();
        foreach (var item in infoArwork)
        {
            if (item.Key == ArtistInfo.artistKey) continue;
            if (!ExtensionMethods.ConcertToBool(item.Value.uploadOptions)[4])
            {
                if (item.Value.@object.transform.childCount > 1)
                {
                    converted.Add(item.Value.@object.transform.root.name);
                    MeshCollider[] meshFilters = item.Value.@object.transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
                    foreach (var item2 in meshFilters)
                    {
                        item2.convex = true;
                        item2.isTrigger = true;
                    }
                }
            }
        }
    }
    public static bool RestitureConvertedObjects(List<string> wasConverted)
    {
        AssetManager _as = GameObject.FindObjectOfType<AssetManager>();
        int i =0;
            while (i < wasConverted.Count)
            {
                if (_as.infoArwork.ContainsKey(wasConverted[i]))
                {
                    if (_as.infoArwork[wasConverted[i]].@object)
                    {
                        MeshCollider[] meshFilters = _as.infoArwork[wasConverted[i]].@object.transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
                        foreach (var item2 in meshFilters)
                        {
                        item2.isTrigger = false;
                        item2.convex = false;
                        }
                    }
                }
                i++;
            }
        return true;
    }
    
}

