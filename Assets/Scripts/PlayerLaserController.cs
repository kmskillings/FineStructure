using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserController : MonoBehaviour
{
    public StickLogic stickLogic;
    public LaserManager laserManager;
    public float triggerThreshold;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        laserManager.isFiring = stickLogic.CurrentTrigger > triggerThreshold;
    }
}
