using Photon.Pun;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class AutoCure : MonoBehaviourPunCallbacks
{
    private PlayerHealth playerHealth;
    private float nextCureTime = -1f;
    private bool flag_start_cure=false;
    private bool curing=false;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    [PunRPC]
    void Update()
    {
        if (photonView.IsMine) {
            //playerHealth.Cure(cureAmount);
            if(playerHealth.currentHealth<100 && !curing){
                //Debug.Log("flag_start_cure");
                flag_start_cure=true;
            }
            if ((Time.time>nextCureTime && nextCureTime!= -1f)|| flag_start_cure)
            {
                //Debug.Log("curing");
                flag_start_cure=false;
                curing=true;
                nextCureTime=Time.time+1;
                playerHealth.Cure(1);
                if(playerHealth.currentHealth>=100||playerHealth.currentHealth<=0){
                    //Debug.Log("end");
                    curing=false;
                    nextCureTime=-1f;
                }
            }
        }
    }
}
