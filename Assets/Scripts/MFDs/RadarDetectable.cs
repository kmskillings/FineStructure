using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarDetectable : MonoBehaviour
{
    public static List<RadarDetectable> allInScene = new List<RadarDetectable>();
    public PipType pipType;

    private void Awake()
    {
        AddIfNotAlreadyAdded();
    }

    private void OnEnable()
    {
        AddIfNotAlreadyAdded();
    }

    private void OnDisable()
    {
        allInScene.Remove(this);
    }

    private void OnDestroy()
    {
        allInScene.Remove(this);
    }

    private void AddIfNotAlreadyAdded()
    {
        if (!allInScene.Contains(this)) allInScene.Add(this);
    }

    public enum PipType
    {
        Scenery,
        Enemy,
        Objective
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
