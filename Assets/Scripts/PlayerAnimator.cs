using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerAnimator : MonoBehaviourPunCallbacks
{
    private SpriteRenderer _renderer;
    private IPlayerController _player;
    public ParticleSystem damageParticles;
    //private int flipVector;
    private PhotonView PV;
    // Start is called before the first frame update
    void Awake()
    {
        PV= GetComponent<PhotonView>();
        _player = GetComponent<IPlayerController>();
        _renderer = GetComponent<SpriteRenderer>();
        _player.OnDamage += DamageParticles;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            HandleSpriteFlipping(_player.Input.x);
        }
        
    }
    private void DamageParticles()
    {
        PV.RPC("RPC_DamageParticles", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_DamageParticles()
    {
        damageParticles.Play();
    }
   

private void HandleSpriteFlipping(float flipVector)
    {
        bool switched = Mathf.Abs(flipVector) > 0.1f;
        if (switched)
            _renderer.flipX = flipVector < 0;

        if (PV.IsMine)
        {

            Hashtable hash = new Hashtable();
            hash.Add("flipVector", flipVector);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("flipVector"))
            {
                HandleSpriteFlipping((float)changedProps["flipVector"]);
            }
        }

    }
}
