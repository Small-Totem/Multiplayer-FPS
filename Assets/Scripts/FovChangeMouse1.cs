using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

//注意：右键开镜会改fov 与动力牛子技能冲突

public class FovChangeMouse1 : MonoBehaviour
{
    public Camera _camera;
    private float targetFOV=-1f;

    void Start()
    {
    }

    void Update()
    {
        bool press = CrossPlatformInputManager.GetButton("Fire3");
        if(press){
            targetFOV=30;
        }
        else if(_camera.fieldOfView==60){
            return;
        }
        else{
            targetFOV=60;
        }
        if(targetFOV!=-1f && targetFOV!=_camera.fieldOfView){
            if(Mathf.Abs(targetFOV-_camera.fieldOfView)<= 3f){
                _camera.fieldOfView=targetFOV;
            }
            if(targetFOV>_camera.fieldOfView){
                _camera.fieldOfView+=1f;
            }
            else{
                _camera.fieldOfView-=2f;
            }
        }
    }
}
