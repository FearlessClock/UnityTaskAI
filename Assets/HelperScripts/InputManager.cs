using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
public enum InputDirection { Up, Down, Left, Right }
public class InputManager
{
    public static bool InputExistsContinuous()
    {
        return (Input.GetMouseButton(0)) ||
                (Input.touchCount > 0 &&
                (Input.touches[0].phase == TouchPhase.Began ||
                 Input.touches[0].phase == TouchPhase.Moved ||
                 Input.touches[0].phase == TouchPhase.Stationary) &&
                !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId));
    }

    public static bool InputExistsMoved()
    {
        return (Input.GetMouseButton(0)) ||
                (Input.touchCount > 0 &&
                (Input.touches[0].phase == TouchPhase.Moved) &&
                !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId));
    }

    public static bool InputExistsUp()
    {
        return (Input.GetMouseButtonUp(0)) ||
                (Input.touchCount > 0 &&
                (Input.touches[0].phase == TouchPhase.Ended) &&
                !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId));   // IsPointerOverGameobject only works with touchPhase Begin, workaround in playerController.cs
    }

    public static bool InputExistsDown()
    {
#if UNITY_EDITOR
        return (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) || 
                (Input.touchCount > 0 &&
                (Input.touches[0].phase == TouchPhase.Began) &&
                !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)); ;
#else
        return (Input.touchCount > 0 &&
                (Input.touches[0].phase == TouchPhase.Began) &&
                !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId));
#endif
    }



    public static Vector3 GetInput(int id)
    {
        if (Application.isMobilePlatform)
        {
            return Input.touches[id].position;
        }
        else
        {
            return Input.mousePosition;
        }
    }

    public static bool GetButtonDown(InputDirection dir)
    {
        switch (dir)
        {
            case InputDirection.Up:
                return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z);
            case InputDirection.Down:
                return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
            case InputDirection.Left:
                return Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q);
            case InputDirection.Right:
                return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        }
        return false;
    }

    public static bool IsPressing(int id = 0)
    {
        if (Application.isMobilePlatform)
        {
            return Input.touchCount > 0;
        }
        else
        {
            return Input.GetMouseButton(id);
        }
    }
}