using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounterMFD : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float framerate = 1f / Time.unscaledDeltaTime;

        textMesh.text = ((int)framerate).ToString();
    }
}
