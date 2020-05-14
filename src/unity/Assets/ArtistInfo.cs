using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArtistInfo 
{
    public static string artistKey;
    public static bool activ;
    public static bool keepInPlace;

    public static string urlArt;
    public static string description;

    public static bool hasArt = false;
    public static bool busy;
    public static Vector3 colderSize;
    public static Vector3 MeshSizes;

    public static string uploadOptionsA = "";
}
public static class GeneralState
{
    public static bool AceptAssets;
    public static bool InRangeOfArtWork;
    public static bool colided;
    public static Vector3 maxColideSize = new Vector3(1000, 1000, 1000);
    public static int artworkLayer =12;
    public static string artworkTag = "Base";

}
public static class ArtWorkFunctions
{
}

