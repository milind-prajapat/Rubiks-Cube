using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kociemba;

public class SolveTwoPhase : MonoBehaviour
{
    ReadCube readCube;
    CubeState cubeState;
    Automate automate;
    private bool doOnce = true;
    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
        automate = FindObjectOfType<Automate>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cubeState.started && doOnce)
        {
            doOnce = false;
            Solver();
            cubeState.ShuffleButton.interactable = true;
            cubeState.SolveButton.interactable = true;
            cubeState.StateButton.interactable = true;
        }
    }

    public void Solver()
    {
        automate.wasButton = true;
        cubeState.ShuffleButton.interactable = false;
        cubeState.SolveButton.interactable = false;
        cubeState.StateButton.interactable = false;

        string moveString = cubeState.GetStateString();
        string info = "";
        
        //string solution = SearchRunTime.solution(moveString, out info, buildTables: true);

        string solution = Search.solution(moveString, out info);
 
        List<string> solutionList = StringToList(solution);

        automate.moveList = solutionList;
    }

    List<string> StringToList(string solution)
    {
        List<string> solutionList = new List<string>(solution.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries));
        return solutionList;
    }
}
