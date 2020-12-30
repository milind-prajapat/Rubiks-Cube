using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    private List<GameObject> activeSide;

    private Vector3 mouseRef;
    private bool dragging = false;
    private float sensitivity = 0.3f;
    private Vector3 rotation;

    private bool autoRotating = false;
    private float speed = 400f;
    private Quaternion targetQuaternion;

    private ReadCube readCube;
    private CubeState cubeState;
    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
    }

    // Update is called once per frame
    void Update()
    {
        if(dragging && !autoRotating)
        {
            SpinSide(activeSide);
            if(Input.GetMouseButtonUp(0))
            {
                cubeState.dragging = false;
                dragging = false;
                RotateToRightAngle();
            }
        }
        if (autoRotating)
        {
            AutoRotate();
        }
    }

    private void SpinSide(List<GameObject> side)
    {
        rotation = Vector3.zero;
        Vector3 mouseOffset = (Input.mousePosition - mouseRef);
        if(side == cubeState.front || side == cubeState.back || side == cubeState.CentreRightVertical)
        {
            rotation.x = (mouseOffset.x - mouseOffset.y) * sensitivity * -1;
        }
        if (side == cubeState.up || side == cubeState.down || side == cubeState.CentreHorizontal)
        {
            rotation.y = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;
        }
        if (side == cubeState.right || side == cubeState.left || side == cubeState.CentreLeftVertical)
        {
            rotation.z = (mouseOffset.x + mouseOffset.y) * sensitivity * -1;
        }

        transform.Rotate(rotation, Space.World);
        mouseRef = Input.mousePosition;
    }

    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        mouseRef = Input.mousePosition;
        dragging = true;
        cubeState.dragging = true;
    }

    public void StartAutoRotate(List<GameObject> side, float angle)
    {
        Vector3 localForward = Vector3.zero;
        if (side == cubeState.CentreRightVertical)
        {
            cubeState.PickCentreRightVertical();
            localForward = Vector3.zero - cubeState.front[4].transform.parent.transform.localPosition;
            targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        }  
        else if (side == cubeState.CentreHorizontal)
        {
            cubeState.PickCentreHorizontal();
            localForward = Vector3.zero - cubeState.up[4].transform.parent.transform.localPosition;
            targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        }
        else if (side == cubeState.CentreLeftVertical)
        {
            cubeState.PickCentreLeftVertical();
            localForward = Vector3.zero - cubeState.right[4].transform.parent.transform.localPosition;
            targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        }
        else
        {
            cubeState.PickUp(side);
            localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
            targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
        }
        activeSide = side;
        autoRotating = true;
    }

    void RotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        targetQuaternion.eulerAngles = vec;
        autoRotating = true;
        cubeState.wasMouse = true;
    }

    void AutoRotate()
    {
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);

        if(Quaternion.Angle(transform.localRotation, targetQuaternion) <= 1)
        {
            transform.localRotation = targetQuaternion;
            cubeState.PutDown(activeSide, transform.parent);
            readCube.ReadState();

            autoRotating = false;
            cubeState.autoRotating = false;
            cubeState.wasMouse = false;
        }
    }
}