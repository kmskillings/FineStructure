using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ControlBarMFD : Graphic
{
    public float min;
    public float max;
    public float neutral;
    public float zero;
    public float current;
    public float target;

    public Color backgroundColor;
    public Color borderColor;
    public float borderWidth;

    public Color maxColor;
    public Color minColor;

    public Color neutralMarkerColor;
    public Color targetMarkerColor;
    public float markerWidth;
    public float markerLength;

    private Vector2 cornerMin;
    private Vector2 cornerMax;
    private Vector2 cornerMinDrawingArea;
    private Vector2 cornerMaxDrawingArea;
    private Rect rectDrawingArea;

    private float neutralX;
    private float zeroX;
    private float currentX;
    private float targetX;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        cornerMin = new Vector2(-rectTransform.pivot.x * rectTransform.rect.width, -rectTransform.pivot.y * rectTransform.rect.height);
        cornerMax = new Vector2((1 - rectTransform.pivot.x) * rectTransform.rect.width, (1 - rectTransform.pivot.y) * rectTransform.rect.height);
        cornerMinDrawingArea = new Vector2(cornerMin.x + borderWidth, cornerMin.y + borderWidth);
        cornerMaxDrawingArea = new Vector2(cornerMax.x - borderWidth, cornerMax.y - borderWidth);
        rectDrawingArea = rectTransform.rect;
        rectDrawingArea.width -= 2 * borderWidth;
        rectDrawingArea.height -= 2 * borderWidth;

        neutralX = LerpUnNormalized(neutral, min, max, cornerMinDrawingArea.x, cornerMaxDrawingArea.x);
        zeroX = LerpUnNormalized(zero, min, max, cornerMinDrawingArea.x, cornerMaxDrawingArea.x);
        currentX = LerpUnNormalized(current, min, max, cornerMinDrawingArea.x, cornerMaxDrawingArea.x);
        targetX = LerpUnNormalized(target, min, max, cornerMinDrawingArea.x, cornerMaxDrawingArea.x);

        vh.Clear();
        DrawBackground(vh);
        DrawValueBar(vh);
        DrawMarker(vh, neutralX, cornerMaxDrawingArea.y, markerWidth, -markerLength, neutralMarkerColor);
        DrawMarker(vh, targetX, cornerMinDrawingArea.y, markerWidth, markerLength, targetMarkerColor);
    }

    public void Draw()
    {
        UpdateGeometry();
    }

    private void DrawMarker(VertexHelper vh, float x, float baseY, float width, float height, Color color)
    {
        Vector2 point = new Vector2(x, baseY + height);
        Vector2 baseLeft = new Vector2(Mathf.Clamp(x - width / 2, cornerMinDrawingArea.x, cornerMaxDrawingArea.x), baseY);
        Vector2 baseRight = new Vector2(Mathf.Clamp(x + width / 2, cornerMinDrawingArea.x, cornerMaxDrawingArea.x), baseY);

        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        int c = vh.currentVertCount;

        vert.position = point;
        vh.AddVert(vert);

        vert.position = baseLeft;
        vh.AddVert(vert);

        vert.position = baseRight;
        vh.AddVert(vert);

        vh.AddTriangle(c, c + 1, c + 2);
    }

    private void DrawBackground(VertexHelper vh)
    {
        DrawBox(vh, cornerMin, cornerMax, borderColor);
        DrawBox(vh, cornerMinDrawingArea, cornerMaxDrawingArea, backgroundColor);
    }

    private void DrawValueBar(VertexHelper vh)
    {
        Color currentColor = Color.Lerp(minColor, maxColor, (current - neutral) / (max - neutral));
        if(current < neutral)
        {
            currentColor = Color.Lerp(minColor, maxColor, (neutral - current) / (neutral - min));
        }

        DrawBoxXGradient(vh, new Vector2(zeroX, cornerMinDrawingArea.y), new Vector2(currentX, cornerMaxDrawingArea.y), minColor, currentColor);
    }

    private void DrawBox(VertexHelper vh, Vector2 cornerMin, Vector2 cornerMax, Color color)
    {
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;

        int c = vh.currentVertCount;

        vert.position = new Vector2(cornerMin.x, cornerMin.y);
        vh.AddVert(vert);

        vert.position = new Vector2(cornerMin.x, cornerMax.y);
        vh.AddVert(vert);

        vert.position = new Vector2(cornerMax.x, cornerMax.y);
        vh.AddVert(vert);

        vert.position = new Vector2(cornerMax.x, cornerMin.y);
        vh.AddVert(vert);

        vh.AddTriangle(c, c + 1, c + 2);
        vh.AddTriangle(c + 2, c + 3, c);
    }

    private void DrawBoxXGradient(VertexHelper vh, Vector2 cornerMin, Vector2 cornerMax, Color colorMin, Color colorMax)
    {
        UIVertex vert = UIVertex.simpleVert;
        vert.color = colorMin;

        int c = vh.currentVertCount;

        vert.position = new Vector2(cornerMin.x, cornerMin.y);
        vh.AddVert(vert);

        vert.position = new Vector2(cornerMin.x, cornerMax.y);
        vh.AddVert(vert);

        vert.color = colorMax;

        vert.position = new Vector2(cornerMax.x, cornerMax.y);
        vh.AddVert(vert);

        vert.position = new Vector2(cornerMax.x, cornerMin.y);
        vh.AddVert(vert);

        vh.AddTriangle(c, c + 1, c + 2);
        vh.AddTriangle(c + 2, c + 3, c);
    }

    private float LerpUnNormalized(float t, float a, float b, float x, float y)
    {
        return (Mathf.Clamp(t, a, b) - a) / (b - a) * (y - x) + x;
    }
}