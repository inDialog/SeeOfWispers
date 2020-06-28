using UnityEngine;
using System;
using System.Collections;

public class ArtistTextManager : MonoBehaviour
{
    
    private string keyCollision;
    public Action<string, MonoBehaviour> Colided;
    private static bool insideBox;
    private IEnumerator coroutine;
    private bool executeCor;


    // Start is called before the first frame update
    void Start()
    {
        keyCollision = "";
        insideBox = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        coroutine = WaitAndDontDisplay();
        executeCor = false;
        if (keyCollision != other.gameObject.transform.root.name)
        {
            keyCollision = other.gameObject.transform.root.name;
            Colided(keyCollision, this);
            insideBox = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (insideBox)
        {
            int layerMask = 1 << 12;
            RaycastHit hit;
            Ray downRay = new Ray(this.transform.position, -Vector3.up);

            if (Physics.Raycast(downRay, out hit, 2, layerMask))
            {
            }
            else
            {
                executeCor = true;
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator WaitAndDontDisplay()
    {
        float i = 0;
        while (i <= 10 && executeCor)
        {

            //StopAllCoroutines();
            i+= Time.deltaTime;
            yield return null;
        }
        if (executeCor)
        {
            Colided("Null", null);
            insideBox = false;
            keyCollision = null;
            executeCor = false;
            StopCoroutine(WaitAndDontDisplay());
        }
    }
}
