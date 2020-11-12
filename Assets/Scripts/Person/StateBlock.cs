using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface StateBlock 
{
    void Entry();
    void Exit();
    bool Update();
}
