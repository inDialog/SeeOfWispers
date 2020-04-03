using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Collider[] colliders;
   public Transform closesObject;
   public float distace;
    public LayerMask lk;
 public    GameObject MainCharacter;
    // Update is called once per frame
   

    private void Update()
    {
        colliders = Physics.OverlapSphere(MainCharacter.transform.position, 100f, lk);
        closesObject = GetClosestEnemy(colliders);
        if (closesObject != null)
        {
            Vector3 target = closesObject.transform.position - MainCharacter.transform.position;
            target = MainCharacter.transform.position - target.normalized*-1;
            float step = 10 * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
    }
    Transform GetClosestEnemy(Collider[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = MainCharacter.transform.position;
        foreach (Collider t in enemies)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }
    private void OnTriggerStay(Collider other)
    {
        
    }
}
