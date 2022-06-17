using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSkill_niuzi : MonoBehaviourPunCallbacks
{
    public bool isSkillAvailable=true;//是否可用
    private bool isSkillValid=false;//是否生效
    public float SkillTime;
    public float SkillCD;
    private float nextAvailableTime;//下次可用技能的时间
    private float nextInvalidTime;//技能结束的时间
    public Camera _camera;
    FirstPersonController controller;
    private Slider skillChargeSlider;
    private float targetFOV=-1f;
    public AudioClip skillSoundClip;

    void Start()
    {
        controller = GetComponent<FirstPersonController>();
        skillChargeSlider = GameObject.FindGameObjectWithTag("SkillChargeSlider").GetComponent<Slider>();
        skillChargeSlider.value=100;
        
    }

    private void skill()
    {
        //Debug.Log("skill()");
        skillChargeSlider.value=0;
        AudioSource.PlayClipAtPoint(skillSoundClip,transform.position);
        controller.SetSpeed(8,14,17);//controller.SetSpeed(8,14,-1);   
        //controller.SetSpeed(12,20,-1);    
        smoothFOVChangeTo(80);
    }

    private void skillInvalid()
    {
        //Debug.Log("skillInvalid()");
        controller.ResetSpeed();    
        smoothFOVChangeTo(60);
    }
    
    private void smoothFOVChangeTo(int fov){
        targetFOV=fov;
    }

    [PunRPC]
    void Update()
    {
        if (photonView.IsMine) {
            bool skilling = CrossPlatformInputManager.GetButton("Fire2");
            if (skilling && isSkillAvailable)
            {
                isSkillAvailable=false;
                isSkillValid=true;
                nextAvailableTime=Time.time+SkillCD;
                nextInvalidTime=Time.time+SkillTime;
                skill();
            }
            if (!isSkillAvailable && isSkillValid && Time.time > nextInvalidTime)
            {
                isSkillValid=false;
                skillInvalid();
            }
            if (!isSkillAvailable && Time.time > nextAvailableTime)
            {
                //Debug.Log("isSkillAvailable=true");
                isSkillAvailable=true;
                skillChargeSlider.value=100;
            }
            if (!isSkillAvailable){//每一帧更新技能充能条
                skillChargeSlider.value=100-(nextAvailableTime-Time.time)/SkillCD*100;
            }
            if(targetFOV!=-1f && targetFOV!=_camera.fieldOfView){
                if(Mathf.Abs(targetFOV-_camera.fieldOfView)< 2){
                    _camera.fieldOfView=targetFOV;
                }
                if(targetFOV>_camera.fieldOfView){
                    _camera.fieldOfView+=2;
                }
                else{
                    _camera.fieldOfView-=0.5f;
                }
            }
        }
    }
}
