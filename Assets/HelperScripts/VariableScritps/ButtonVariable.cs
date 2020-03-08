using UnityEngine;

[CreateAssetMenu(fileName = "ButtonVariable", menuName = "UnityHelperScripts/ButtonVariable", order = 0)]
public class ButtonVariable : ScriptableObject {
    [SerializeField] private KeyCode buttonKeyboardName = KeyCode.None;
    [SerializeField] private KeyCode buttonJoystickName = KeyCode.None;
    [SerializeField] private string buttonDescription = "";

    public KeyCode GetButton(bool joystick = true){
        return joystick? buttonJoystickName:buttonKeyboardName;
    }

    public string GetButtonDescription
    {
        get
        {
            return buttonDescription;
        }
    }
}