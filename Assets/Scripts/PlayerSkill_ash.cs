using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSkill_ash : MonoBehaviourPunCallbacks
{
    public bool isSkillAvailable=true;//是否可用
    private bool isSkillValid=false;//是否生效
    public float SkillTime=0.1f;
    public float SkillCD;
    private float nextAvailableTime;//下次可用技能的时间
    private float nextInvalidTime;//技能结束的时间
    FirstPersonController controller;
    private Slider skillChargeSlider;

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
        controller.SetSpeed(200,200,-1);    
        //Debug.Log(skillChargeSlider.value);
    }

    private void skillInvalid()
    {
        //Debug.Log("skillInvalid()");
        controller.ResetSpeed();
    }

    [PunRPC]
    void Update()
    {
        if (photonView.IsMine) {
            bool skilling = CrossPlatformInputManager.GetButton("Fire2");
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");
            bool moving=(horizontal!=0)||(vertical!=0);
            if (skilling && isSkillAvailable && moving)//使用技能时
            {
                isSkillAvailable=false;
                isSkillValid=true;
                nextAvailableTime=Time.time+SkillCD;
                nextInvalidTime=Time.time+SkillTime;
                skill();
            }
            if (!isSkillAvailable && isSkillValid && Time.time > nextInvalidTime)//技能效果结束时
            {
                isSkillValid=false;
                skillInvalid();
            }
            if (!isSkillAvailable && Time.time > nextAvailableTime)//技能cd结束时
            {
                //Debug.Log("isSkillAvailable=true");
                isSkillAvailable=true;
                skillChargeSlider.value=100;
            }
            if (!isSkillAvailable){//每一帧更新技能充能条
                skillChargeSlider.value=100-(nextAvailableTime-Time.time)/SkillCD*100;
            }
        }
    }
}
