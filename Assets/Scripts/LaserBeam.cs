using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public LineRenderer beamRenderer;
    public Vector3 direction;
    public float range;
    public Transform startingPoint;
    [ColorUsage(true, true)]
    public Color lightColor;    //The color of light around the central white beam (Blue for the player, red for enemies, etc.)

    public float duration;      //How long the laser should be on when fired
    private float elapsed;      //How long the laser has been 'on' this firing
    private bool isFiring;

    public LayerMask hitSceneryLayer;
    public LayerMask hitShipLayer;
    public AudioSource sound;   //The sound to play when the laser fires

    public GameObject hitSceneryParticle;  //The particleSystem prefabs to instantiate
    public GameObject hitShipParticle;
    public GameObject flameParticle;
    public Color particleInitialColor;

    public Vector3 particleDirection;   //The direction particles are emitted in without any rotation (in the particlesystem's local space)

    public float damage;

    private Transform hitTransform;     //The transform that was hit by the beam (if any)
    private Vector3 hitPoint;           //The point that was hit (in the hitTransform's local space, if applicable, in world space otherwise)

    private void Awake()
    {
        beamRenderer.material.SetColor("_EmissionColor", lightColor);
        beamRenderer.enabled = false;
        beamRenderer.useWorldSpace = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //hitAnythingParticle.colorOverLifetime.color.gradient.colorKeys[0].color = particleInitialColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFiring)
        {
            elapsed += Time.deltaTime;
            if(elapsed >= duration)
            {
                StopFiring();
            }

            beamRenderer.SetPosition(0, startingPoint.position);

            if(hitTransform != null)
            {
                //Sets the end point to track the hit target
                Vector3 worldHitPoint = hitTransform.TransformPoint(hitPoint);
                beamRenderer.SetPosition(1, worldHitPoint);
            }
        }
    }

    private void StopFiring()
    {
        isFiring = false;
        beamRenderer.enabled = false;
    }

    public void Fire()
    {
        if (isFiring) return;
        elapsed = 0;
        isFiring = true;

        sound.Play();

        beamRenderer.positionCount = 2;
        beamRenderer.SetPosition(0, startingPoint.position);

        RaycastHit hit;
        Vector3 worldDirection = transform.TransformDirection(direction);
        Ray ray = new Ray(startingPoint.position, worldDirection);
        if (Physics.Raycast(ray, out hit, range, hitSceneryLayer.value | hitShipLayer.value))
        {
            hitTransform = hit.transform;
            hitPoint = hitTransform.InverseTransformPoint(hit.point);

            if(1 << hit.transform.gameObject.layer == hitShipLayer.value)
            {
                PlaceOnTarget(hitShipParticle, hit.transform, particleDirection, hit.normal, hitPoint);
                PlaceOnTarget(flameParticle, hit.transform, particleDirection, hit.normal, hitPoint);
            }
            else
            {
                PlaceOnTarget(hitSceneryParticle, hit.transform, particleDirection, hit.normal, hitPoint);
            }

            hit.transform.GetComponent<ShipDamage>()?.DealDamage(damage);
        }
        else
        {
            hitTransform = null;
            beamRenderer.SetPosition(1, startingPoint.position + worldDirection * range);
        }

        beamRenderer.enabled = true;
    }

    private void PlaceOnTarget(GameObject go, Transform parent, Vector3 direction, Vector3 normal, Vector3 localPoint)
    {
        GameObject instance = Instantiate(go);
        instance.transform.parent = parent;
        instance.transform.localPosition = localPoint;
        instance.transform.rotation = Quaternion.FromToRotation(direction, normal);
        instance.transform.localScale = new Vector3(1, 1, 1);
    }
}
