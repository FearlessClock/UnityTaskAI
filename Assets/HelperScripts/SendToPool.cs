using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToPool : MonoBehaviour
{
    [HideInInspector]
    public GameObjectPool pool;
    public void SendBackToPool(){
        if(pool != null){
            pool.ReturnObject(this.gameObject);
        }else{
            Destroy(this.gameObject);
        }
    }
}
