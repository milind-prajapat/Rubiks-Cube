using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCheck : Selectable
{
    public bool buttonHighlighted = false;

    void Update()
    {
        if (IsHighlighted() == true || IsPressed() == true)
        {
            buttonHighlighted = true;
        }
        else
        {
            buttonHighlighted = false;
        }
    }
}