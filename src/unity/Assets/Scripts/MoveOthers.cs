using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOthers : MonoBehaviour
{
    public Vector3 pastPosition;
    Multiplayer multiplayer;
    private Animator animations;
    Vector3 target;
    bool moving;
  
    private void Start()
    {
        multiplayer = FindObjectOfType<Multiplayer>();
        animations = GetComponent<Animator>();
        target = multiplayer.infoPl[this.name].position;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curentP = this.transform.position;
        UpdatePosition();
        pastPosition = this.transform.position;
        float dist = DistanceToGround();
        if (dist < 20 & dist != 0)
        {
            if (moving)
            {
                animations.SetTrigger("Walk");
            }
            else
                animations.SetTrigger("idelGround");

        }
        else
        {

            if (curentP.y < pastPosition.y)
            {
               animations.SetTrigger("takeoff");
            }
            else
            {
               animations.SetTrigger("ForwordUp");
            }
        }
    }
    
    int DistanceToGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, transform.up * -1);
        if (Physics.Raycast(ray, out hit))
        {
            return (int)(hit.distance * 100);
        }
        else return 0;
    }

    void UpdatePosition()
    {
        if (multiplayer.infoPl.ContainsKey(this.name))
        {
          
                target = multiplayer.infoPl[this.name].position;
                float dist = Vector3.Distance(this.transform.position, target);
                float step = dist * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, target, step);
                transform.rotation = Quaternion.Euler(multiplayer.infoPl[this.name].rotation);
                moving = true;
          

        }
    }
}


//public class Controller : MonoBehaviour
//{
//    [Header("Global")]
//    public float MaxMovementPerUpdate = 5;
//    public float MinMovementPerUpdate = 1;
//    public bool SimulateNetworkDelay = false;
//    public float MinNetworkDelay = 0.05f;
//    public float MaxNetworkDelay = 0.6f;
//    private Vector3 startPos = Vector3.zero;
//    private Vector3 endPos;
//    public float SendsPerSecond = 10f;
//    public bool CalculateBasedOnSendRate = true;
//    [Tooltip("Has no effect if CalculateBasedOnSendRate is true")]
//    public float LerpPerFrame = 0.5f;
//    [Range(0.01f, 60f)]
//    public float FramesPerSecond = 10f;
//    public InterpolationType CurrentInterpolationType;
//    public float t = 0;
//    public GameObject CurrentPos;
//    public GameObject LastPos;

//    //Time.deltaTime but it uses the deltaTime for our faked framerate.
//    private float DeltaTime
//    {
//        get
//        {
//            return Time.time - lastFrame;
//        }
//    }

//    void Start()
//    {
//        endPos = new Vector3(Random.Range(MinMovementPerUpdate, MaxMovementPerUpdate), 0, Random.Range(MinMovementPerUpdate, MaxMovementPerUpdate));
//        CurrentPos.transform.position = endPos;
//        LastPos.transform.position = startPos;
//        StartCoroutine(SendPosition());
//    }

//    private void OnValidate()
//    {
//        if (!CalculateBasedOnSendRate && CurrentInterpolationType == InterpolationType.StartToTargetUsingLerp)
//        {
//            Debug.LogWarning("Cannot use fixed movestep for lerp when using StartToTarget. CalculateBasedOnSendRate has been turned on.");
//            CalculateBasedOnSendRate = true;
//        }
//    }




//    private float lastFrame;
//    void Update()
//    {
//        if (Time.time - lastFrame > (1f / FramesPerSecond))
//        {
//            if (CurrentInterpolationType == InterpolationType.CurrentToTargetUsingLerp)
//            {
//                if (CalculateBasedOnSendRate)
//                {
//                    t += DeltaTime / (1f / SendsPerSecond);
//                    transform.position = Vector3.Lerp(transform.position, endPos, t);
//                }
//                else
//                {
//                    transform.position = Vector3.Lerp(transform.position, endPos, LerpPerFrame * DeltaTime);
//                }
//            }
//            else if (CurrentInterpolationType == InterpolationType.StartToTargetUsingLerp)
//            {
//                if (CalculateBasedOnSendRate)
//                {
//                    t += DeltaTime / (1f / SendsPerSecond);
//                    transform.position = Vector3.Lerp(startPos, endPos, t);
//                }
//            }
//            else if (CurrentInterpolationType == InterpolationType.CurrentToTargetUsingMoveTowards)
//            {
//                if (CalculateBasedOnSendRate)
//                {
//                    transform.position = Vector3.MoveTowards(transform.position, endPos, Vector3.Distance(startPos, endPos) / (1f / SendsPerSecond) * DeltaTime);
//                }
//                else
//                {
//                    transform.position = Vector3.MoveTowards(transform.position, endPos, LerpPerFrame * DeltaTime);
//                }

//            }
//            lastFrame = Time.time;
//        }
//    }

//    IEnumerator SendPosition()
//    {
//        while (true)
//        {
//            //-> (1 / SendsPerSecond) = TimeBetweenSends
//            if (SimulateNetworkDelay)
//                yield return new WaitForSeconds((1f / SendsPerSecond) + Random.Range(MinNetworkDelay, MaxNetworkDelay));
//            else
//                yield return new WaitForSeconds(1f / SendsPerSecond);
//            //This could also be set to endPos. But this currentPos is probably prefered in case it arrives early.
//            startPos = transform.position;
//            endPos = GetTargetPosition();
//            t = 0;
//            LastPos.transform.position = startPos;
//            CurrentPos.transform.position = endPos;
//        }
//    }

//    private Vector3 GetTargetPosition()
//    {
//        return transform.position + new Vector3(Random.Range(MinMovementPerUpdate, MaxMovementPerUpdate), 0, Random.Range(MinMovementPerUpdate, MaxMovementPerUpdate));
//    }

//    public enum InterpolationType
//    {
//        CurrentToTargetUsingLerp,
//        StartToTargetUsingLerp,
//        CurrentToTargetUsingMoveTowards
//    }
//}
