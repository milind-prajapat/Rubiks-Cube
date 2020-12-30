using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Automate : MonoBehaviour
{
    public List<string> moveList = new List<string>() {};
    private readonly List<string> allMoves = new List<string>()
    {
        "U","D","L","R","F","B","M","S","E",
        "U'","D'","L'","R'","F'","B'","M'","S'","E'",
        "U2","D2","L2","R2","F2","B2","M2","S2","E2"
    };

    CubeState cubeState;
    ReadCube readCube;
    RotateBigCube rotateBigCube;
    // Start is called before the first frame update
    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
        rotateBigCube = FindObjectOfType<RotateBigCube>();
    }

    // Update is called once per frame
    void Update()
    {
        if(moveList.Count > 0 && !cubeState.autoRotating && !cubeState.dragging && cubeState.started && !rotateBigCube.dragging && !rotateBigCube.autoRotating)
        {
            DoMove(moveList[0]);
            moveList.Remove(moveList[0]);
        }
        else if(moveList.Count == 0)
        {
            cubeState.ShuffleButton.interactable = true;
            cubeState.SolveButton.interactable = true;
            cubeState.StateButton.interactable = true;
        }
    }

    public void Shuffle()
    {
        cubeState.ShuffleButton.interactable = false;
        cubeState.SolveButton.interactable = false;
        cubeState.StateButton.interactable = false;

        List<string> moves = new List<string>();
        int shuffleLength = Random.Range(20, 25);

        for(int i=0; i<shuffleLength; i++)
        {
            int randomMove = Random.Range(0, allMoves.Count);
            moves.Add(allMoves[randomMove]);
        }
        moveList = moves;
    }

    void DoMove(string move)
    {
        cubeState.autoRotating = true;

        if (move == "U")
        {
            RotateSide(cubeState.up, -90);
        }
        if (move == "u" || move == "U'")
        {
            RotateSide(cubeState.up, 90);
        }
        if (move == "U2")
        {
            RotateSide(cubeState.up, 180);
        }
        if (move == "D")
        {
            RotateSide(cubeState.down, -90);
        }
        if (move == "d" || move == "D'")
        {
            RotateSide(cubeState.down, 90);
        }
        if (move == "D2")
        {
            RotateSide(cubeState.down, 180);
        }
        if (move == "L")
        {
            RotateSide(cubeState.left, -90);
        }
        if (move == "l" || move == "L'")
        {
            RotateSide(cubeState.left, 90);
        }
        if (move == "L2")
        {
            RotateSide(cubeState.left, -180);
        }
        if (move == "R")
        {
            RotateSide(cubeState.right, -90);
        }
        if (move == "r" || move == "R'")
        {
            RotateSide(cubeState.right, 90);
        }
        if (move == "R2")
        {
            RotateSide(cubeState.right, -180);
        }
        if (move == "F")
        {
            RotateSide(cubeState.front, -90);
        }
        if (move == "f" || move == "F'")
        {
            RotateSide(cubeState.front, 90);
        }
        if (move == "F2")
        {
            RotateSide(cubeState.front, -180);
        }
        if (move == "B")
        {
            RotateSide(cubeState.back, -90);
        }
        if (move == "b" || move == "B'")
        {
            RotateSide(cubeState.back, 90);
        }
        if (move == "B2")
        {
            RotateSide(cubeState.back, -180);
        }
        if (move == "M")
        {
            RotateSide(cubeState.CentreRightVertical, -90, true);
        }
        if (move == "m" || move == "M'")
        {
            RotateSide(cubeState.CentreRightVertical, 90, true);
        }
        if (move == "M2")
        {
            RotateSide(cubeState.CentreRightVertical, -180, true);
        }
        if (move == "S")
        {
            RotateSide(cubeState.CentreLeftVertical, -90, true);
        }
        if (move == "s" || move == "S'")
        {
            RotateSide(cubeState.CentreLeftVertical, 90, true);
        }
        if (move == "S2")
        {
            RotateSide(cubeState.CentreLeftVertical, -180, true);
        }
        if (move == "E")
        {
            RotateSide(cubeState.CentreHorizontal, -90, true);
        }
        if (move == "e" || move == "E'")
        {
            RotateSide(cubeState.CentreHorizontal, 90, true);
        }
        if (move == "E2")
        {
            RotateSide(cubeState.CentreHorizontal, -180, true);
        }
        if (move == "X")
        {
            rotateBigCube.StartAutoRotate("LeftVertical", -90);
        }
        if (move == "x" || move == "X'")
        {
            rotateBigCube.StartAutoRotate("LeftVertical", 90);
        }
        if (move == "X2")
        {
            rotateBigCube.StartAutoRotate("LeftVertical", -180);
        }
        if (move == "Y")
        {
            rotateBigCube.StartAutoRotate("Horizontal", -90);
        }
        if (move == "y" || move == "Y'")
        {
            rotateBigCube.StartAutoRotate("Horizontal", 90);
        }
        if (move == "Y2")
        {
            rotateBigCube.StartAutoRotate("Horizontal", -180);
        }
        if (move == "Z")
        {
            rotateBigCube.StartAutoRotate("RightVertical", -90);
        }
        if (move == "z" || move == "Z'")
        {
            rotateBigCube.StartAutoRotate("RightVertical", 90);
        }
        if (move == "Z2")
        {
            rotateBigCube.StartAutoRotate("RightVertical", -180);
        }
    }

    void RotateSide(List<GameObject> side, float angle, bool check = false)
    {
        PivotRotation pr;
        if (check)
        {
            pr = cubeState.Centre.GetComponent<PivotRotation>();
            pr.StartAutoRotate(side, angle);
        }
        else
        {
            pr = side[4].transform.parent.GetComponent<PivotRotation>();
            pr.StartAutoRotate(side, angle);
        } 
    }
}
