using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class PlayerSkill_lifeline : MonoBehaviourPunCallbacks
{
    public bool isSkillAvailable=true;//是否可用
    private bool isSkillValid=false;//是否生效
    public float SkillTime=0.1f;
    public float SkillCD;
    public int cureAmount=30;
    private float nextAvailableTime;//下次可用技能的时间
    private float nextInvalidTime;//技能结束的时间
    private PlayerHealth playerHealth;
    private Slider skillChargeSlider;
    public AudioClip skillSoundClip;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        skillChargeSlider = GameObject.FindGameObjectWithTag("SkillChargeSlider").GetComponent<Slider>();
        skillChargeSlider.value=100;
    }

    private void skill()
    {
        //Debug.Log("skill()");
        skillChargeSlider.value=0;
        AudioSource.PlayClipAtPoint(skillSoundClip,transform.position);
        playerHealth.Cure(cureAmount);
    }

    private void skillInvalid()
    {
        //Debug.Log("skillInvalid()");
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
        }
    }
}
