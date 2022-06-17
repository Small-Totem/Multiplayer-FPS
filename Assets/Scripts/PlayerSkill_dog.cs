using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

/*
狗子的技能 调用时遍历有DogSkillShaderTarget这个tag的GameObject(要手动指定) 并对其着色
shader @ref https://blog.csdn.net/sinat_25415095/article/details/123752904

*/


public class PlayerSkill_dog : MonoBehaviourPunCallbacks
{
    public bool isSkillAvailable=true;//是否可用
    private bool isSkillValid=false;//是否生效
    public float SkillTime=3f;
    public float SkillCD;
    private float nextAvailableTime;//下次可用技能的时间
    private float nextInvalidTime;//技能结束的时间
    private Slider skillChargeSlider;
    private SkinnedMeshRenderer[] _renderer;
    private Material[] backups;
    public AudioClip skillSoundClip;


    void Start()
    {
        skillChargeSlider = GameObject.FindGameObjectWithTag("SkillChargeSlider").GetComponent<Slider>();
        skillChargeSlider.value=100;
    }

    void skill()
    {
        //Debug.Log("skill()");
        //targets=GameObject.FindGameObjectWithTag("DogSkillShaderTarget").GetComponent<SkinnedMeshRenderer>();
        AudioSource.PlayClipAtPoint(skillSoundClip,transform.position);
        
        Material cubeMat= new Material(Shader.Find("Custom/XRay"));//load shader
        cubeMat.color=new Color(1, 0, 0, 1);//设置颜色,r g b a 注意的rgb的数值范围是0->1
        cubeMat.renderQueue=2001;//渲染队列，越大越后渲染

        GameObject[] targets = GameObject.FindGameObjectsWithTag("DogSkillShaderTarget");
        //Debug.Log(targets.Length);
        backups = new Material[targets.Length];
        _renderer = new SkinnedMeshRenderer[targets.Length];
        int index=0;
        foreach (GameObject obj in targets) {
            SkinnedMeshRenderer renderer = obj.GetComponent<SkinnedMeshRenderer>();
            _renderer[index] = renderer;
            backups[index] = renderer.material;
            index++;
            renderer.material = cubeMat;
        }
        
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().AddMessage("检测到雷达!");
    }

    private void skillInvalid()
    {
        //Debug.Log("skillInvalid()");
        int index=0;
        foreach (SkinnedMeshRenderer renderer in _renderer) {
            renderer.material = backups[index];
            index++;
        }
    }


    [PunRPC]
    void Update()
    {
        //不判断isMine会有同步问题（玩家a触发玩家b的技能）
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
