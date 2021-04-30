using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Input Character")]
public class InputScriptable : ScriptableObject
{

    public string nameInput;

    [Header("AXIS MOVEMENT")]
    [Space]
    public InputActions axisMovementX;
    public InputActions axisMovementY;


    [Header("AXIS CAMERA")]
    [Space]
    public InputActions axisCameraX;
    public InputActions axisCameraY;

    [Header("JUMP BUTTON")]
    [Space]
    public InputActions jumpBtn;


    [Header("CROUCH BUTTON")]
    [Space]
    public InputActions crouchButton;


    [Header("SPRINT BUTTON")]
    [Space]
    public InputActions sprintButton;

    [Header("FIRE BUTTON")]
    [Space]
    public InputActions fireButton;

    [Header("AIM BUTTON")]
    [Space]
    public InputActions aimButton;


    [Header("HIT BUTTON")]
    [Space]
    public InputActions hitButton;

    [Header("THROWABLE BUTTON")]
    [Space]
    public InputActions throwableButton;

    [Header("RELOAD BUTTON")]
    [Space]
    public InputActions reloadButton;

    [Header("SWITCH BUTTON")]
    [Space]
    public InputActions switchButton;

    [Header("SWITCH AXIS")]
    [Space]
    public InputActions axisCrossHorizontal;
    public InputActions axisCrossVertical;

    [Header("ESCAPE BUTTON")]
    [Space]
    public InputActions escapeButton;
}
