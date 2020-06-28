using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using System;
using AsImpL;
using UnityEngine.UI;
using System.Linq;

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
        string temp = "";
        for (int i = 0; i < inputFields.Length-1; i++)
        {
            if (i == inputFields.Length - 2)
                temp += inputFields[i].text;
            else
                temp += inputFields[i].text + "§";
        }
        return temp;
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
        Color32 myColor;
        myColor = new Color32(
        (byte)UnityEngine.Random.Range(0, 200),
        (byte)UnityEngine.Random.Range(0, 200),
        (byte)UnityEngine.Random.Range(0, 200),
        225);
        myColor.a = 225;
        return myColor;
    }
    public static string StringToCollor(Color32 Color)
    {
        return
        Color.r + "\t" +
        Color.g + "\t" +
        Color.b;
    }

    public static Vector3 Trip(Vector3 Original, Vector3 target, Transform space)
    {
        return space.InverseTransformPoint(Original) - target;

    }
    public static void ConvertConvexObjects(Dictionary<string, InfoArtwork> InfoArtwork, out List<string> converted)
    {
        converted = new List<string>();
        foreach (var item in InfoArtwork)
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
        int i = 0;
        while (i < wasConverted.Count)
        {
            if (_as.InfoArtwork.ContainsKey(wasConverted[i]))
            {
                if (_as.InfoArtwork[wasConverted[i]].@object)
                {
                    MeshCollider[] meshFilters = _as.InfoArtwork[wasConverted[i]].@object.transform.GetChild(1).GetComponentsInChildren<MeshCollider>();
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
  
    public static Bounds ObjectBounds(Transform obj)
    {
        Bounds meshesBounds = new Bounds(obj.position, Vector3.zero);
        MeshRenderer[] mrs = obj.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < mrs.Length; i++)
        {
            if (i == 0) meshesBounds = mrs[i].bounds;
            else meshesBounds.Encapsulate(mrs[i].bounds);
        }
        return meshesBounds;
    }
    public static Vector3 LocationTeleport(string artKey, AssetManager astMan)
    {
        Vector3 pos;
        pos = Vector3.zero;
        if (artKey != "" & artKey != null)
        {
            GameObject artContainer;
            GameObject containerMesh;
            artContainer = astMan.InfoArtwork[artKey].@object;
            if (artContainer.transform.childCount < 2)
            {
                Debug.LogWarning("There is no artwork at this key");
                return pos;
            }
            containerMesh = artContainer.transform.GetChild(1).gameObject;
            Bounds tmp_mesh_bounds;
            tmp_mesh_bounds = ExtensionMethods.ObjectBounds(containerMesh.transform);
            float biggest_side = Check_Sides_Values(tmp_mesh_bounds.size);
            pos = tmp_mesh_bounds.center - new Vector3(0, 0, biggest_side);
        }
        else
            Debug.LogWarning("They key for teleporting object its missing");
        return pos;
    }
    public static float Check_Sides_Values(Vector3 vec3)
    {
        float tmp;
        tmp = 0;
        if (vec3.x > vec3.y && vec3.x > vec3.z)
            tmp = vec3.x;
        else if (vec3.y > vec3.x && vec3.y > vec3.z)
            tmp = vec3.y;
        else if (vec3.z > vec3.y && vec3.z > vec3.x)
            tmp = vec3.z;
        return (tmp);
    }
}