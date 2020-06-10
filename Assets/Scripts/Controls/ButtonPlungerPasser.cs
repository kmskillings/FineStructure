using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlungerPasser : MonoBehaviour
{
    public ButtonLogic parentButton;

    private void OnCollisionEnter(Collision collision)
    {
        //parentButton.OnCollisionEnterChild(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        //parentButton.OnCollisionExitChild(collision);
    }
}
