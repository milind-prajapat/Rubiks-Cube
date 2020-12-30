using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFace : MonoBehaviour
{
    CubeState cubeState;
    ReadCube readCube;
    RotateBigCube rotateBigCube;
    Automate automate;

    private Vector3 start;
    private bool Clicked;
    private int faceIndex, pieceIndex;

    private int layerMask = 1 << 8;
    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
        rotateBigCube = FindObjectOfType<RotateBigCube>();
        automate = FindObjectOfType<Automate>();
    }

    int Distance(Vector3 V1, Vector3 V2)
    {
        return (int)Mathf.Sqrt(Mathf.Pow((V2.x - V1.x), 2) + Mathf.Pow((V2.y - V1.y), 2) + Mathf.Pow((V2.z - V1.z), 2));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && automate.moveList.Count == 0 && !cubeState.autoRotating && !rotateBigCube.autoRotating && cubeState.started && cubeState.ShuffleButton.interactable)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                GameObject face = hit.collider.gameObject;
                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    cubeState.up,
                    cubeState.down,
                    cubeState.left,
                    cubeState.right,
                    cubeState.front,
                    cubeState.back
                };

                foreach(List<GameObject> cubeSide in cubeSides)
                {
                    if (cubeSide.Contains(face))
                    {
                        faceIndex = cubeSides.IndexOf(cubeSide);
                        pieceIndex = cubeSide.IndexOf(face);
                        start = Input.mousePosition;
                        Clicked = true;
                    }
                }
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            Clicked = false;
        }
        if (Distance(Input.mousePosition, start) > 5 && Clicked)
        {
            List<List<GameObject>> cubeSides = new List<List<GameObject>>()
            {
                cubeState.up,
                cubeState.down,
                cubeState.left,
                cubeState.right,
                cubeState.front,
                cubeState.back
            };

            Vector3 mouseDelta = Input.mousePosition - start;

            if (faceIndex == 0)
            {
                if(mouseDelta.x * mouseDelta.y < 0)
                {
                    if (pieceIndex < 3)
                    {
                        cubeState.PickUp(cubeSides[5]);
                        cubeSides[5][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[5]);
                    }
                    else if (pieceIndex > 5)
                    {
                        cubeState.PickUp(cubeSides[4]);
                        cubeSides[4][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[4]);
                    }
                    else
                    {
                        cubeState.PickCentreRightVertical();
                        cubeState.Centre.GetComponent<PivotRotation>().Rotate(cubeState.CentreRightVertical);
                    }
                }
                else
                {
                    if (pieceIndex == 0 || pieceIndex == 3 || pieceIndex == 6)
                    {
                        cubeState.PickUp(cubeSides[2]);
                        cubeSides[2][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[2]);
                    }
                    else if (pieceIndex == 2 || pieceIndex == 5 || pieceIndex == 8)
                    {
                        cubeState.PickUp(cubeSides[3]);
                        cubeSides[3][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[3]);
                    }
                    else
                    {
                        cubeState.PickCentreLeftVertical();
                        cubeState.Centre.GetComponent<PivotRotation>().Rotate(cubeState.CentreLeftVertical);
                    }
                }   
            }
            else if (faceIndex == 3)
            {
                if (mouseDelta.x < 0)
                    mouseDelta.x *= -1;
                if (mouseDelta.y < 0)
                    mouseDelta.y *= -1;
                if (mouseDelta.x > mouseDelta.y)
                {
                    if (pieceIndex < 3)
                    {
                        cubeState.PickUp(cubeSides[0]);
                        cubeSides[0][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[0]);
                    }
                    else if (pieceIndex > 5)
                    {
                        cubeState.PickUp(cubeSides[1]);
                        cubeSides[1][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[1]);
                    }
                    else
                    {
                        cubeState.PickCentreHorizontal();
                        cubeState.Centre.GetComponent<PivotRotation>().Rotate(cubeState.CentreHorizontal);
                    }
                }
                else
                {
                    if (pieceIndex == 0 || pieceIndex == 3 || pieceIndex == 6)
                    {
                        cubeState.PickUp(cubeSides[4]);
                        cubeSides[4][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[4]);
                    }
                    else if (pieceIndex == 2 || pieceIndex == 5 || pieceIndex == 8)
                    {
                        cubeState.PickUp(cubeSides[5]);
                        cubeSides[5][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[5]);
                    }
                    else
                    {
                        cubeState.PickCentreRightVertical();
                        cubeState.Centre.GetComponent<PivotRotation>().Rotate(cubeState.CentreRightVertical);
                    }
                }  
            }
            else if(faceIndex == 4)
            {
                if (mouseDelta.x < 0)
                    mouseDelta.x *= -1;
                if (mouseDelta.y < 0)
                    mouseDelta.y *= -1;
                if (mouseDelta.x > mouseDelta.y)
                {
                    if (pieceIndex < 3)
                    {
                        cubeState.PickUp(cubeSides[0]);
                        cubeSides[0][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[0]);
                    }
                    else if (pieceIndex > 5)
                    {
                        cubeState.PickUp(cubeSides[1]);
                        cubeSides[1][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[1]);
                    }
                    else
                    {
                        cubeState.PickCentreHorizontal();
                        cubeState.Centre.GetComponent<PivotRotation>().Rotate(cubeState.CentreHorizontal);
                    }
                }
                else
                {
                    if (pieceIndex == 0 || pieceIndex == 3 || pieceIndex == 6)
                    {
                        cubeState.PickUp(cubeSides[2]);
                        cubeSides[2][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[2]);
                    }
                    else if (pieceIndex == 2 || pieceIndex == 5 || pieceIndex == 8)
                    {
                        cubeState.PickUp(cubeSides[3]);
                        cubeSides[3][4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSides[3]);
                    }
                    else
                    {
                        cubeState.PickCentreLeftVertical();
                        cubeState.Centre.GetComponent<PivotRotation>().Rotate(cubeState.CentreLeftVertical);
                    }
                }
            }
            Clicked = false;
        }
    }
}
