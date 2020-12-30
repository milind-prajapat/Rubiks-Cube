using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadCube : MonoBehaviour
{
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;
    public GameObject emptyGO;

    private List<GameObject> frontRays = new List<GameObject>();
    private List<GameObject> backRays = new List<GameObject>();
    private List<GameObject> upRays = new List<GameObject>();
    private List<GameObject> downRays = new List<GameObject>();
    private List<GameObject> leftRays = new List<GameObject>();
    private List<GameObject> rightRays = new List<GameObject>();

    private int layerMask = 1 << 8;
    CubeState cubeState;
    
    // Start is called before the first frame update
    void Start()
    {
        SetRayTransforms();
        ReadState();
        cubeState = FindObjectOfType<CubeState>();
        cubeState.started = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReadState()
    {
        cubeState = FindObjectOfType<CubeState>();

        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);

        cubeState.CentreHorizontal = H();
        cubeState.CentreLeftVertical = LV();
        cubeState.CentreRightVertical = RV();
    }

    List<GameObject> RV()
    {
        List<GameObject> Temp = new List<GameObject>();

        for (int i = 3; i < 6; i++)
        {
            Temp.Add(cubeState.up[i]);
            Temp.Add(cubeState.down[i]);
        }
        for (int i = 1; i <= 7; i += 3)
        {
            Temp.Add(cubeState.left[i]);
            Temp.Add(cubeState.right[i]);
        }
        return Temp;
    }

    List<GameObject> LV()
    {
        List<GameObject> Temp = new List<GameObject>();

        for (int i = 1; i <= 7; i += 3)
        {
            Temp.Add(cubeState.up[i]);
            Temp.Add(cubeState.down[i]);
            Temp.Add(cubeState.front[i]);
            Temp.Add(cubeState.back[i]);
        }
        return Temp;
    }

    List<GameObject> H()
    {
        List<GameObject> Temp = new List<GameObject>();

        for (int i = 3; i < 6; i++)
        {
            Temp.Add(cubeState.front[i]);
            Temp.Add(cubeState.back[i]);
            Temp.Add(cubeState.left[i]);
            Temp.Add(cubeState.right[i]);
        }
        return Temp;
    }

    void SetRayTransforms()
    {
        upRays = BuildRays(tUp, new Vector3(90, 90, 0));
        downRays = BuildRays(tDown, new Vector3(270, 90, 0));
        leftRays = BuildRays(tLeft, new Vector3(0, 180, 0));
        rightRays = BuildRays(tRight, new Vector3(0, 0, 0));
        frontRays = BuildRays(tFront, new Vector3(0, 90, 0));
        backRays = BuildRays(tBack, new Vector3(0, 270, 0));
    }
    
    List<GameObject> BuildRays(Transform rayTransform, Vector3 direction)
    {
        int rayCount = 0;
        List<GameObject> rays = new List<GameObject>();

        for(int y = 1;y > -2; y--)
        {
            for(int x = -1; x < 2; x++)
            {
                Vector3 startPos = new Vector3(rayTransform.localPosition.x + x,
                                               rayTransform.localPosition.y + y,
                                               rayTransform.localPosition.z);

                GameObject rayStart = Instantiate(emptyGO, startPos, Quaternion.identity, rayTransform);
                rayStart.name = rayCount.ToString();
                rays.Add(rayStart);
                rayCount++;
            }
        }
        rayTransform.localRotation = Quaternion.Euler(direction);
        return rays;
    }

    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> facesHit = new List<GameObject>();

        foreach (GameObject rayStart in rayStarts)
        {
            Vector3 ray = rayStart.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(ray,rayTransform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(ray, rayTransform.forward * hit.distance, Color.yellow);
                facesHit.Add(hit.collider.gameObject);
            }
            else
            {
                Debug.DrawRay(ray, rayTransform.forward * 1000, Color.green);
            }
        }
       return facesHit;
    }
}
