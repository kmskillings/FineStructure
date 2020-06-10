using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarMFD : Graphic
{
    public float radarRange;
    public Transform shipTransform;

    public int innerRingSegments;
    public int outerRingSegments;

    public float innerRingThickness;
    public float outerRingThickness;

    public Color outerRingColor;
    public Color innerRingColor;

    public float centerCrossRadius;
    public float centerCrossThickness;
    public Color centerCrossColor;

    public float pipSize;
    public Color enemyPipColor;
    public Color sceneryPipColor;
    public Color objectivePipColor;

    private Vector2[] outerRingOuterVerts;
    private Vector2[] outerRingInnerVerts;

    private Vector2[] innerRingOuterVerts;
    private Vector2[] innerRingInnerVerts;

    private Vector2 center;
    private float drawingAreaRadius;

    [ExecuteInEditMode]
    protected override void Awake()
    {
        outerRingOuterVerts = new Vector2[outerRingSegments];
        outerRingInnerVerts = new Vector2[outerRingSegments];

        innerRingOuterVerts = new Vector2[innerRingSegments];
        innerRingInnerVerts = new Vector2[innerRingSegments];

        //Finds center of rectTransform
        center = new Vector2((0.5f - rectTransform.pivot.x) * rectTransform.rect.width, (0.5f - rectTransform.pivot.y) * rectTransform.rect.height);

        //Finds radii
        float outerRingOuterRadius = rectTransform.rect.width / 2;
        float outerRingInnerRadius = outerRingOuterRadius * (1 - outerRingThickness);
        drawingAreaRadius = outerRingInnerRadius;

        float innerRingOuterRadius = rectTransform.rect.width / 4;
        float innerRingInnerRadius = innerRingOuterRadius * (1 - innerRingThickness);

        PopulateRingVerts(center, outerRingOuterVerts, outerRingInnerVerts, outerRingOuterRadius, outerRingInnerRadius);
        PopulateRingVerts(center, innerRingOuterVerts, innerRingInnerVerts, innerRingOuterRadius, innerRingInnerRadius);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        //Draws the 'background' (rings)
        vh.Clear();
        DrawRing(vh, outerRingOuterVerts, outerRingInnerVerts, outerRingColor);
        DrawRing(vh, innerRingOuterVerts, innerRingInnerVerts, innerRingColor);

        //Draws the actual pips
        foreach(RadarDetectable contact in RadarDetectable.allInScene)
        {
            Vector3 offset = shipTransform.InverseTransformPoint(contact.transform.position);
            if (offset.magnitude < radarRange)
            {

                Vector2 zProjPosition = new Vector2(offset.x, offset.y);
                float phi = Mathf.Atan2(zProjPosition.magnitude, offset.z);

                float radial = phi / Mathf.PI * drawingAreaRadius;
                Vector2 pipCenter = zProjPosition.normalized * radial;

                switch (contact.pipType)
                {
                    case RadarDetectable.PipType.Scenery:
                        DrawSceneryPip(vh, pipCenter);
                        break;
                    case RadarDetectable.PipType.Enemy:
                        DrawEnemyPip(vh, pipCenter);
                        break;
                    case RadarDetectable.PipType.Objective:
                        DrawObjectivePip(vh, pipCenter);
                        break;
                    default:
                        break;
                }
            }
        }

        DrawCross(vh, center, centerCrossRadius, centerCrossThickness, centerCrossColor);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGeometry();
    }

    private void PopulateRingVerts(Vector2 center, Vector2[] outerVerts, Vector2[] innerVerts, float outerRadius, float innerRadius)
    {
        if (outerVerts.Length != innerVerts.Length) throw new System.Exception("Arrays passed to PopulateRingVerts are of different lengths! On object " + gameObject.name);

        float theta;
        for (int i = 0; i < outerVerts.Length; i++)
        {
            theta = 2 * Mathf.PI / outerVerts.Length * i;
            float s = Mathf.Sin(theta);
            float c = Mathf.Cos(theta);
            outerVerts[i] = new Vector2(c * outerRadius, s * outerRadius) + center;
            innerVerts[i] = new Vector2(c * innerRadius, s * innerRadius) + center;
        }
    }

    private void DrawRing(VertexHelper vh, Vector2[] outerVerts, Vector2[] innerVerts, Color color)
    {
        int oc = vh.currentVertCount;

        UIVertex v = UIVertex.simpleVert;
        v.color = color;

        foreach (Vector2 outerVert in outerVerts)
        {
            v.position = outerVert;
            vh.AddVert(v);
        }

        int ic = vh.currentVertCount;

        foreach (Vector2 innerVert in innerVerts)
        {
            v.position = innerVert;
            vh.AddVert(v);
        }

        for(int i = 1; i < outerVerts.Length; i++)
        {
            vh.AddTriangle(oc + i - 1, oc + i, ic + i - 1);
            vh.AddTriangle(ic + i - 1, ic + i, oc + i);
        }
        vh.AddTriangle(oc + outerVerts.Length - 1, oc, ic + innerVerts.Length - 1);
        vh.AddTriangle(ic + innerVerts.Length - 1, ic, oc);
    }

    private void DrawCross(VertexHelper vh, Vector2 center, float radius, float thickness, Color color)
    {
        DrawRect(vh, center, radius * 2, thickness, color);
        DrawRect(vh, center, thickness, radius * 2, color);
    }

    private void DrawRect(VertexHelper vh, Vector2 center, float height, float width, Color color)
    {
        int c = vh.currentVertCount;

        UIVertex v = UIVertex.simpleVert;
        v.color = color;
        v.position = new Vector2(center.x - width / 2, center.y + height / 2);
        vh.AddVert(v);

        v.position.x += width;
        vh.AddVert(v);

        v.position.y -= height;
        vh.AddVert(v);

        v.position.x -= width;
        vh.AddVert(v);

        vh.AddTriangle(c, c + 1, c + 2);
        vh.AddTriangle(c + 2, c + 3, c);
    }

    //Draws an upside-down triangle
    private void DrawEnemyPip(VertexHelper vh, Vector2 center)
    {
        UIVertex v = UIVertex.simpleVert;
        v.color = enemyPipColor;

        int c = vh.currentVertCount;
        float s = Mathf.Sqrt(3f) / 2;

        v.position = center;
        v.position.y -= pipSize;
        vh.AddVert(v);

        v.position.y += 1.5f * pipSize;
        v.position.x += s * pipSize;
        vh.AddVert(v);

        v.position.x -= s * pipSize * 2;
        vh.AddVert(v);

        vh.AddTriangle(c, c + 1, c + 2);
    }

    //Draws a square
    private void DrawSceneryPip(VertexHelper vh, Vector2 center)
    {
        float s = Mathf.Sqrt(2);
        DrawRect(vh, center, s * pipSize, s * pipSize, sceneryPipColor);
    }

    //Draws a diamond
    private void DrawObjectivePip(VertexHelper vh, Vector2 center)
    {
        UIVertex v = UIVertex.simpleVert;
        v.color = objectivePipColor;

        int c = vh.currentVertCount;

        v.position = center;
        v.position.y += pipSize;
        vh.AddVert(v);

        v.position.y -= 2 * pipSize;
        vh.AddVert(v);

        v.position = center;
        v.position.x += pipSize;
        vh.AddVert(v);

        v.position.x -= 2 * pipSize;
        vh.AddVert(v);

        vh.AddTriangle(c, c + 1, c + 2);
        vh.AddTriangle(c, c + 1, c + 3);
    }
}
