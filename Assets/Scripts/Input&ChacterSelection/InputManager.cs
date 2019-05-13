using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputCommand
{
    Start,
    MenuSubmit,
    VerticalMenu1,
    VerticalMenu2,
    Cancel,
    Nothing,

    TopPlayerSelect,
    TopPlayerRotate,
    TopPlayerMoveHorizontal1,
    TopPlayerMoveHorizontal2,
    TopPlayerMoveVertical1,
    TopPlayerMoveVertical2,
    TopPlayerMenu,

    BottomPlayerJump,
    BottomPlayerMoveStick,
    BottomPlayerMoveKeyboard,
    BottomPlayerBoost,
    BottomPlayerCrouch,
}



public class InputManager : MonoBehaviour {
    [HideInInspector] public bool P1IsTop;
    [HideInInspector] public bool TutorialSelected;

    //P1 is not top by default
    private void Awake()
    {
        P1IsTop = false;
    }

    //Set input enums for when controller 1 is top player
    private Dictionary<InputCommand, string> p2AsTop = new Dictionary<InputCommand, string>
    {
        //Universal Controls
        { InputCommand.Start, "Start" },
        { InputCommand.MenuSubmit, "Submit_Menu" },
        { InputCommand.VerticalMenu1, "Vertical_Menu_Dpad" },
        { InputCommand.VerticalMenu2, "Vertical_Menu_Stick" },
        { InputCommand.Cancel, "Cancel" },
        { InputCommand.Nothing, "Nothing" },

        //Top Player Controls
        { InputCommand.TopPlayerSelect, "Place_Joy_2" },
        { InputCommand.TopPlayerRotate, "Rotate_Joy_2" },
        { InputCommand.TopPlayerMoveHorizontal1, "Horizontal_Joy_2_Stick" },
        { InputCommand.TopPlayerMoveHorizontal2, "Horizontal_Joy_2_Dpad" },
        { InputCommand.TopPlayerMoveVertical1, "Vertical_Joy_2_Stick" },
        { InputCommand.TopPlayerMoveVertical2, "Vertical_Joy_2_Dpad" },
        { InputCommand.TopPlayerMenu, "Bumpers_Joy_2" },

        //Bottom Controls
        { InputCommand.BottomPlayerJump, "Jump_Joy_1" },
        { InputCommand.BottomPlayerMoveStick, "Horizontal_Joy_1_Stick"},
        { InputCommand.BottomPlayerMoveKeyboard, "Horizontal_Keyboard" },
        { InputCommand.BottomPlayerBoost, "Boost_Joy_1" }, //TODO
        { InputCommand.BottomPlayerCrouch, "Crouch_Joy_1" }
    };

    //Set input enums for when controller 2 is top player
    private Dictionary<InputCommand, string> p1AsTop = new Dictionary<InputCommand, string>
    {
        //Universal Controls
        { InputCommand.Start, "Start" },
        { InputCommand.MenuSubmit, "Submit_Menu" },
        { InputCommand.VerticalMenu1, "Vertical_Menu_Dpad" },
        { InputCommand.VerticalMenu2, "Vertical_Menu_Stick" },
        { InputCommand.Cancel, "Cancel" },
        { InputCommand.Nothing, "Nothing" },

        //Top Player Controls
        { InputCommand.TopPlayerSelect, "Place_Joy_1" },
        { InputCommand.TopPlayerRotate, "Rotate_Joy_1" },
        { InputCommand.TopPlayerMoveHorizontal1, "Horizontal_Joy_1_Stick" },
        { InputCommand.TopPlayerMoveHorizontal2, "Horizontal_Joy_1_Dpad" },
        { InputCommand.TopPlayerMoveVertical1, "Vertical_Joy_1_Stick" },
        { InputCommand.TopPlayerMoveVertical2, "Vertical_Joy_1_Dpad" },
        { InputCommand.TopPlayerMenu, "Bumpers_Joy_1" },

        //Bottom Controls
        { InputCommand.BottomPlayerJump, "Jump_Joy_2" },
        { InputCommand.BottomPlayerMoveStick, "Horizontal_Joy_2_Stick"},
        { InputCommand.BottomPlayerMoveKeyboard, "Horizontal_Keyboard" },
        { InputCommand.BottomPlayerBoost, "Boost_Joy_2" }, //TODO
        { InputCommand.BottomPlayerCrouch, "Crouch_Joy_2" }
    };

    public bool GetButton(InputCommand command)
    {
        string unityInputString = P1IsTop ? p1AsTop[command] : p2AsTop[command];
        return Input.GetButton(unityInputString);
    }
    public bool GetButtonDown(InputCommand command)
    {
        string unityInputString = P1IsTop ? p1AsTop[command] : p2AsTop[command];
        return Input.GetButtonDown(unityInputString);
    }

    public bool GetButtonUp(InputCommand command)
    {
        string unityInputString = P1IsTop ? p1AsTop[command] : p2AsTop[command];
        return Input.GetButtonUp(unityInputString);
    }

    public float GetAxis(InputCommand command)
    {
        string unityInputString = P1IsTop ? p1AsTop[command] : p2AsTop[command];
        return Input.GetAxis(unityInputString);
    }
}
