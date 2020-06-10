using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    public LaserBeam[] beams;
    public float delay;         //How long to wait between when one laser starts firing, and when the next one should start firing
    private float elapsed = 0;  //How long it's been since the last laser started firing
    public bool isFiring = false;
    private int currentBeam = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(elapsed < delay)
        {
            elapsed += Time.deltaTime;
        }

        if (isFiring)
        {
            if(elapsed >= delay)
            {
                FireNextBeam();
            }
        }
    }

    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        isFiring = false;
    }

    private void AdvanceBeam()
    {
        currentBeam = (currentBeam + 1) % beams.Length;
    }

    private void FireNextBeam()
    {
        elapsed = 0;
        AdvanceBeam();
        beams[currentBeam].Fire();
    }
}
