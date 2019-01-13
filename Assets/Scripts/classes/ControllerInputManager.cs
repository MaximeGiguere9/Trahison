/**
 * classe controllerInputManager
 * Maxime Giguère
 * 
 * cette classe permet les contrôles de base avec une manette (XBox One testée seulement)
 * par soucis de temps, je ne pourrai pas changer InputManager pour supporter l'association dynamique des axis, 
 * alors les contrôles manette sont hardcoded et connecté à InputManager via cette classe
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ControllerInputManager  {

    //liste des commandes du controlleurs en utilisation pour fournir une fonctionnalité similaire à GetKeyDown
    private static List<string> _commandInUse;
    private static List<string> CommandInUse
    {
        get
        {
            if (_commandInUse == null) _commandInUse = new List<string>();
            return _commandInUse;
        }
    }

    //presque équivalent à GetKeyDown (un seul call au lieu de une seule frame)
    public static bool GetControllerCommandStateOnce(string commandName)
    {
        if (GetControllerCommandState(commandName))
        {
            if (CommandInUse.Contains(commandName))
            {
                return false;
            }
            else
            {
                CommandInUse.Add(commandName);
                return true;
            }
            
        }
        else
        {
            if (CommandInUse.Contains(commandName))
            {
                CommandInUse.Remove(commandName);
            }
            return false;
        }
    }

    //équivalent à GetKey
    public static bool GetControllerCommandStateContinuous(string commandName)
    {
        return GetControllerCommandState(commandName);
    }

    //boutons du controlleur sont défini directement par soucis de temps (InputManager ne supporte pas les axis comme commandes. C'est possible de le changer, mais je n'aurai pas le temps)
    private static bool GetControllerCommandState(string commandName)
    {
		//Debug.Log (Input.GetAxisRaw ("ControllerLeftJoystickHorizontal") + " " + Input.GetAxisRaw ("ControllerDPadHorizontal") + " " + Input.GetAxisRaw("ControllerLeftJoystickVertical") + " " + Input.GetAxisRaw("ControllerDPadVertical"));
        switch (commandName)
        {
		case "left":
				return Input.GetAxisRaw ("ControllerLeftJoystickHorizontal") < 0;
            case "right":
                return Input.GetAxisRaw("ControllerLeftJoystickHorizontal") > 0;
            case "up":
                return Input.GetAxisRaw("ControllerLeftJoystickVertical") < 0;
            case "down":
                return Input.GetAxisRaw("ControllerLeftJoystickVertical") > 0;
            case "interact":
                return Input.GetKey(KeyCode.Joystick1Button0);//A
            case "proceed":
                return Input.GetKey(KeyCode.Joystick1Button0);//A
            case "run":
                return Input.GetKey(KeyCode.Joystick1Button1);//B
            case "cancel":
                return Input.GetKey(KeyCode.Joystick1Button1);//B
            case "attack":
                return Input.GetKey(KeyCode.Joystick1Button2);//X
            case "jump":
                return Input.GetKey(KeyCode.Joystick1Button3);//Y
            case "menu":
                return Input.GetKey(KeyCode.Joystick1Button7);//Start
            default:
                return false;
        }
    }
}
