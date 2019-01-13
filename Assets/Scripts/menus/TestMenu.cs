//test/debug

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMenu : MonoBehaviour {

    private void Update()
    {

        //Debug.Log(Input.GetAxis("Horizontal") + " " + Input.GetAxis("Vertical")); //L
        //Debug.Log(Input.anyKey); //A B X Y LB RB Start Select
        //Debug.Log(Input.GetAxis("Fire1") + " " + Input.GetAxis("Fire2") + " " + Input.GetAxis("Fire3")); //joystick axis 3 -> LT - RT, joystick axis 9 -> LT, joystick axis 10 -> RT
        //Debug.Log(InputManager.GetCommand(InputManager.AvatarControl.up) + " " + InputManager.GetCommand(InputManager.AvatarControl.down) + " " + InputManager.GetCommand(InputManager.AvatarControl.left) + " " + InputManager.GetCommand(InputManager.AvatarControl.right));
        //Debug.Log(InputManager.GetCommand(InputManager.UIControl.up) + " " + InputManager.GetCommand(InputManager.UIControl.down) + " " + InputManager.GetCommand(InputManager.UIControl.left) + " " + InputManager.GetCommand(InputManager.UIControl.right));
        Debug.Log(Input.GetKey(KeyCode.Joystick1Button4) + " " + Input.GetKey(KeyCode.Joystick1Button5) + " " + Input.GetKey(KeyCode.Joystick1Button6) + " " + Input.GetKey(KeyCode.Joystick1Button7) + " " + Input.GetKey(KeyCode.Joystick1Button8) + " " + Input.GetKey(KeyCode.Joystick1Button9));

    }
}
