using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Player = Photon.Realtime.Player;

public class Damage : MonoBehaviourPunCallbacks
{
    Renderer[] renderers;

    int init_hp = 100;
    public int curr_hp = 100;

    Animator anim;
    CharacterController cc;

    readonly int hash_die = Animator.StringToHash("Die");
    readonly int hash_respawn = Animator.StringToHash("Respawn");

    GameManager game_manager;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        curr_hp = init_hp;

        game_manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (curr_hp > 0 && other.collider.CompareTag("BULLET"))
        {
            curr_hp -= 20;
            if (curr_hp <= 0)
            {
                if(photonView.IsMine == true)
                {
                    var actor_num = other.collider.GetComponent<Bullet>().actor_number;
                    Player last_shoot_player = PhotonNetwork.CurrentRoom.GetPlayer(actor_num);

                    string msg = string.Format("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",
                                               photonView.Owner.NickName,
                                               last_shoot_player.NickName);
                    photonView.RPC("KillMessage", RpcTarget.AllBufferedViaServer, msg);
                }
                StartCoroutine(PlayerDie());
            } 
        }
    }

    [PunRPC]
    void KillMessage(string msg)
    {
        game_manager.msg_list.text += msg;
    }

    IEnumerator PlayerDie()
    {
        cc.enabled = false;
        anim.SetBool(hash_respawn, false);
        anim.SetTrigger(hash_die);

        yield return new WaitForSeconds(3);

        anim.SetBool(hash_respawn, true);
        SetPlayerVisible(false);

        yield return new WaitForSeconds(1.5f);

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);
        transform.position = points[idx].position;

        curr_hp = 100;
        SetPlayerVisible(true);
        cc.enabled = true;
    }

    void SetPlayerVisible(bool is_visible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = is_visible;
        }
    }
}
