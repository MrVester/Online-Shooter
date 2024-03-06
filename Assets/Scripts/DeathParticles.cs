using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticles : MonoBehaviour
{
    private PhotonView PV;
    private ParticleSystem _particles;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _particles = GetComponent<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        if (_particles.isStopped)
        {
        PhotonNetwork.Destroy(PV);
        }
    }
}
