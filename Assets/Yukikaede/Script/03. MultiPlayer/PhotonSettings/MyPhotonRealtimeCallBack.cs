using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyPhotonRealtimeCallBack : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
