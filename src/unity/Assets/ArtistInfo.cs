using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArtistInfo 
{
    public static string artistKey = "";
    public static bool activ= false;
    public static bool keepInPlace = false;

    public static string urlArt ="";
    public static string description="";

    public static bool hasArt = false;
    public static bool busy = false;
    public static Vector3 colderSize = new Vector3(0,0,0);
    public static Vector3 MeshSizes = new Vector3(0, 0, 0);

    public static string uploadOptionsA = "";
}
public static class GeneralState
{
    public static bool AceptAssets = false;
    public static bool InRangeOfArtWork = false;
    public static bool colided = false;
    public static Vector3 maxColideSize = Vector3.one * 20;
    public static float maxDistance =20;

    public static int artworkLayer =12;
    public static string artworkTag = "Base";

}



