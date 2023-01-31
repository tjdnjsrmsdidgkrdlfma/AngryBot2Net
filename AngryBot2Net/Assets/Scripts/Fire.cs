using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviour
{
    public Transform fire_pos;
    public GameObject bullet_prefab;

    ParticleSystem muzzle_flash;

    PhotonView pv;
    bool is_mouse_click => Input.GetMouseButtonDown(0);

    void Start()
    {
        pv = GetComponent<PhotonView>();
        muzzle_flash = fire_pos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(pv.IsMine == true && is_mouse_click == true)
        {
            FireBullet(pv.Owner.ActorNumber);
            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber);
        }
    }

    [PunRPC]
    void FireBullet(int actor_num)
    {
        if (muzzle_flash.isPlaying == false)
            muzzle_flash.Play(true);

        GameObject bullet = Instantiate(bullet_prefab,
                                        fire_pos.position,
                                        fire_pos.rotation);
        bullet.GetComponent<Bullet>().actor_number = actor_num;
    }
}
