using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MovementSetup", menuName = "UnityHelperScripts/MovementSetup", order = 0)]
public class MovementSetup : ScriptableObject {
    public AxisVariable horizontal;
    public AxisVariable vertical;

    public ButtonVariable actionButton;
    public ButtonVariable attackButton;

    public Vector3 GetMovementVector2D(bool vertical = true, bool horizontal = true){
        return new Vector3(horizontal? this.horizontal.GetRawAxis(): 0, vertical? this.vertical.GetRawAxis():0, 0);
    }

    public bool isActionButtonDown(){
        return (Input.GetKey(actionButton.GetButton()) || Input.GetKey(actionButton.GetButton(false))); 
    }

    public bool isActionButtonUp(){
        return (Input.GetKeyUp(actionButton.GetButton()) || Input.GetKeyUp(actionButton.GetButton(false))); 
    }

    public bool isAttackButtonDown(){
        return (Input.GetKeyDown(attackButton.GetButton()) || Input.GetKeyDown(attackButton.GetButton(false))); 
    }
}