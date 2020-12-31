using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBigCube : MonoBehaviour
{
    private Vector3 localForward;
    private Vector3 mouseRef;
    private Vector3 mouseOffset; 

    public bool dragging = false;
    public bool autoRotating = false;

    private float sensitivity = 0.3f;
    private Vector3 rotation;
    private float speed = 400f;
    private Quaternion targetQuaternion;
    private Vector3 start;
    private bool selectFunc;
    private string selectedFunction;

    private int layerMask = 1 << 8;
    private bool Auto = false;
    CubeState cubeState;
    ReadCube readCube;
    Automate automate;

    // Start is called before the first frame update
    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
        automate = FindObjectOfType<Automate>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((automate.moveList.Count == 0 && !cubeState.autoRotating && !cubeState.dragging && cubeState.started && !cubeState.QuitButton.GetComponent<ButtonCheck>().buttonHighlighted && !cubeState.ShuffleButton.GetComponent<ButtonCheck>().buttonHighlighted && !cubeState.SolveButton.GetComponent<ButtonCheck>().buttonHighlighted && !cubeState.StateButton.GetComponent<ButtonCheck>().buttonHighlighted) || (Auto && !cubeState.dragging && cubeState.started))
        {
            if (dragging && !autoRotating)
            {
                if (Distance(Input.mousePosition, start) > 5)
                {
                    if (selectFunc)
                    {
                        SelectFunc();
                        selectFunc = false;
                    }
                    else if (!selectFunc)
                    {
                        if (selectedFunction == "Func1")
                        {
                            Func1();
                        }
                        else if (selectedFunction == "Func2")
                        {
                            Func2();
                        }
                        else if (selectedFunction == "Func3")
                        {
                            Func3();
                        }
                    }        
                }
                if (Input.GetMouseButtonUp(0))
                {
                    dragging = false;

                    cubeState.ShuffleButton.interactable = true;
                    cubeState.SolveButton.interactable = true;
                    cubeState.StateButton.interactable = true;

                    RotateToRightAngle();
                }
            }
            else if (autoRotating)
            {
                AutoRotate();
            }
            else
            {
                Check();
            }
        }
    }

    int Distance(Vector3 V1, Vector3 V2)
    {
        return (int)Mathf.Sqrt(Mathf.Pow((V2.x - V1.x), 2) + Mathf.Pow((V2.y - V1.y), 2) + Mathf.Pow((V2.z - V1.z), 2));
    }

    void RotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        targetQuaternion.eulerAngles = vec;
        autoRotating = true;
    }

    void AutoRotate()
    {
        dragging = false;
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        if (Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            transform.localRotation = targetQuaternion;
            readCube.ReadState();

            autoRotating = false;
            selectFunc = false;
            Auto = false;
            cubeState.autoRotating = false;
        }
    }

    void Check()
    {
        if(Input.GetMouseButtonDown(0))
        {
            dragging = true;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
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

                foreach (List<GameObject> cubeSide in cubeSides)
                {
                    if (cubeSide.Contains(face))
                    {
                        dragging = false;
                    }
                }
            }
            if (dragging)
            {
                start = Input.mousePosition;
                mouseRef = Input.mousePosition;
                selectFunc = true;

                cubeState.ShuffleButton.interactable = false;
                cubeState.SolveButton.interactable = false;
                cubeState.StateButton.interactable = false;
            }
        }
    }

    void SelectFunc()
    {
        if (Input.GetMouseButton(0))
        {
            rotation = Vector3.zero;
            mouseOffset = (Input.mousePosition - mouseRef);
            Vector3 Normalised = mouseOffset;
            Normalised.Normalize();

            if(Normalised.x <= -0.5f || Normalised.x >= 0.5f)
            {
                selectedFunction = "Func1";
            }
            else if(start.x < Screen.width/2)
            {
                selectedFunction = "Func2";
            }
            else
            {
                selectedFunction = "Func3";
            }
            mouseRef = Input.mousePosition;
        }
    }

    void Func1()
    {
        rotation = Vector3.zero;
        mouseOffset = (Input.mousePosition - mouseRef);

        rotation.y = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;

        transform.Rotate(rotation, Space.World);
        mouseRef = Input.mousePosition;
    }
    void Func2()
    {
        rotation = Vector3.zero;
        mouseOffset = (Input.mousePosition - mouseRef);

        rotation.z = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;

        transform.Rotate(rotation, Space.World);
        mouseRef = Input.mousePosition;
    }
    void Func3()
    {
        rotation = Vector3.zero;
        mouseOffset = (Input.mousePosition - mouseRef);

        rotation.x = (mouseOffset.x - mouseOffset.y) * sensitivity * -1;

        transform.Rotate(rotation, Space.World);
        mouseRef = Input.mousePosition;
    }

    public void StartAutoRotate(string side, float angle)
    {
        Vector3 localForward = Vector3.zero;
        if (side == "Horizontal")
        {
            localForward = new Vector3(0.0f, 2.0f, 0.0f);
        }
        else if (side == "LeftVertical")
        {
            localForward = new Vector3(0.0f, 0.0f, -2.0f);
        }
        else if (side == "RightVertical")
        {
            localForward = new Vector3(-2.0f, 0.0f, 0.0f);
        }
        targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        autoRotating = true;
        Auto = true;
    }
}
