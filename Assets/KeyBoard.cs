using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KeyBoard : MonoBehaviour
{
    Automate automate;
    CubeState cubeState;
    RotateBigCube rotateBigCube;

    [System.Runtime.InteropServices.DllImport("USER32.dll")] public static extern short GetKeyState(int nVirtKey);
    private bool IsCapsLockOn => (GetKeyState(0x14) & 1) > 0;

    private bool isCapsLockOn;

    // Start is called before the first frame update
    void Start()
    {
        if (IsCapsLockOn)
            isCapsLockOn = true;
        else
            isCapsLockOn = false;
        automate = FindObjectOfType<Automate>();
        cubeState = FindObjectOfType<CubeState>();
        rotateBigCube = FindObjectOfType<RotateBigCube>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
            isCapsLockOn = !isCapsLockOn;
        if (cubeState.started && !cubeState.dragging && !rotateBigCube.dragging && !rotateBigCube.autoRotating && cubeState.ShuffleButton.interactable && !cubeState.wasMouse)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("U");
                else
                    automate.moveList.Add("u");
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("D");
                else
                    automate.moveList.Add("d");
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("F");
                else
                    automate.moveList.Add("f");
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("B");
                else
                    automate.moveList.Add("b");
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("R");
                else
                    automate.moveList.Add("r");
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("L");
                else
                    automate.moveList.Add("l");
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("M");
                else
                    automate.moveList.Add("m");
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("S");
                else
                    automate.moveList.Add("s");
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if ((isCapsLockOn || (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) && !(isCapsLockOn && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
                    automate.moveList.Add("E");
                else
                    automate.moveList.Add("e");
            }
        }
    }
}