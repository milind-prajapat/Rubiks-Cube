using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CubeState : MonoBehaviour
{
    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();

    public List<GameObject> CentreHorizontal = new List<GameObject>();
    public List<GameObject> CentreLeftVertical = new List<GameObject>();
    public List<GameObject> CentreRightVertical = new List<GameObject>();

    public GameObject Centre;

    public Button SolveButton;
    public Button ShuffleButton;
    public Button StateButton;
    public Button QuitButton;

    public char upChar;
    public char downChar;
    public char leftChar;
    public char rightChar;
    public char frontChar;
    public char backChar;

    public bool wasMouse = false;
    public bool autoRotating = false;
    public bool dragging = false;
    public bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        Centre = GameObject.Find("C");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickUp(List<GameObject> cubeSide)
    {
        foreach(GameObject face in cubeSide)
        {
            if (face != cubeSide[4])
            {
                face.transform.parent.transform.parent = cubeSide[4].transform.parent;
            }    
        }
    }

    public void ExitGame()

    {
        Application.Quit();
    }

    public void PickCentreHorizontal()
    {
        for(int i = 3; i < 6; i++)
        {
            front[i].transform.parent.transform.parent = Centre.transform;
            back[i].transform.parent.transform.parent = Centre.transform;
            left[i].transform.parent.transform.parent = Centre.transform;
            right[i].transform.parent.transform.parent = Centre.transform;
        }
    }

    public void PickCentreLeftVertical()
    {
        for(int i = 1; i <= 7; i += 3)
        {
            up[i].transform.parent.transform.parent = Centre.transform;
            down[i].transform.parent.transform.parent = Centre.transform;
            front[i].transform.parent.transform.parent = Centre.transform;
            back[i].transform.parent.transform.parent = Centre.transform;
        }  
    }

    public void PickCentreRightVertical()
    {
        for (int i = 3; i < 6; i++)
        {
            up[i].transform.parent.transform.parent = Centre.transform;
            down[i].transform.parent.transform.parent = Centre.transform;
        }
        for (int i = 1; i <= 7; i += 3)
        {
            left[i].transform.parent.transform.parent = Centre.transform;
            right[i].transform.parent.transform.parent = Centre.transform;
        }
    }

    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        foreach(GameObject littleCube in littleCubes)
        {
            littleCube.transform.parent.transform.parent = pivot;
        }
    }

    void UpdateSingleString(GameObject G, char C)
    {
        if (G.name[0] == 'U')
        {
            upChar = C;
        }
        else if (G.name[0] == 'D')
        {
            downChar = C;
        }
        else if (G.name[0] == 'F')
        {
            frontChar = C;
        }
        else if (G.name[0] == 'B')
        {
            backChar = C;
        }
        else if (G.name[0] == 'R')
        {
            rightChar = C;
        }
        else if (G.name[0] == 'L')
        {
            leftChar = C;
        }
    }

    void UpdateStrings()
    {
        UpdateSingleString(up[4], 'U');
        UpdateSingleString(down[4], 'D');
        UpdateSingleString(front[4], 'F');
        UpdateSingleString(back[4], 'B');
        UpdateSingleString(right[4], 'R');
        UpdateSingleString(left[4], 'L');
    }

    string GetSideString(List<GameObject> side)
    {
        string sideString = "";
        foreach(GameObject face in side)
        {
            if(face.name[0] == 'U')
            {
                sideString += upChar.ToString();
            }
            else if (face.name[0] == 'D')
            {
                sideString += downChar.ToString();
            }
            else if (face.name[0] == 'F')
            {
                sideString += frontChar.ToString();
            }
            else if (face.name[0] == 'B')
            {
                sideString += backChar.ToString();
            }
            else if (face.name[0] == 'L')
            {
                sideString += leftChar.ToString();
            }
            else if (face.name[0] == 'R')
            {
                sideString += rightChar.ToString();
            }
        }
        return sideString;
    }

    public string GetStateString()
    {
        UpdateStrings();

        string stateString = "";
        stateString += GetSideString(up);
        stateString += GetSideString(right);
        stateString += GetSideString(front);
        stateString += GetSideString(down);
        stateString += GetSideString(left);
        stateString += GetSideString(back);

        return stateString;
    }

    string GetOriginalSideString(List<GameObject> side)
    {
        string sideString = "";
        foreach (GameObject face in side)
        {
            sideString += face.name[0];
        }
        return sideString;
    }

    public void WriteOriginalStateString()
    {
        string stateString="Current State\n\n";
        stateString += "Up Face\n";
        stateString += GetOriginalSideString(up);
        stateString += "\n\n";
        stateString += "Right Face\n";
        stateString += GetOriginalSideString(right);
        stateString += "\n\n";
        stateString += "Front Face\n";
        stateString += GetOriginalSideString(front);
        stateString += "\n\n";
        stateString += "Down Face\n";
        stateString += GetOriginalSideString(down);
        stateString += "\n\n";
        stateString += "Left Face\n";
        stateString += GetOriginalSideString(left);
        stateString += "\n\n";
        stateString += "Back Face\n";
        stateString += GetOriginalSideString(back);

        WriteString(stateString);
    }

    void WriteString(string StringTOWrite)
    {
        string path = Application.dataPath + "/CurrentState.txt";
        File.WriteAllText(path, StringTOWrite);
    }
}
