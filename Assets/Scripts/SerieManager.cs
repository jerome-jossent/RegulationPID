using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerieManager : MonoBehaviour
{
    private GraphManager graphManager;

    public List<Vector3> points = new List<Vector3>();
    public LineRenderer lineRenderer;

    internal float epaisseur;
    internal Color color;
    internal float layer_origine;

    internal float xMin_val, xMax_val, yMin_val, yMax_val;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = epaisseur;
        lineRenderer.endWidth = lineRenderer.startWidth;

        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = color;

        lineRenderer.useWorldSpace = false;
        lineRenderer.alignment = LineAlignment.TransformZ;
    }
    internal void Link(GraphManager graphManager)
    {
        this.graphManager = graphManager;
        transform.SetParent(graphManager.zonegraph.transform.parent.transform);
    }

    internal void Add(Vector2 point)
    {
        points.Add(new Vector3(point.x, point.y, layer_origine));

        while (points.Count > graphManager.nbrPointsMax_parSerie)
            points.RemoveAt(0);

        //MAJ des bornes
        xMin_val = points[0].x;
        xMax_val = points[0].x;
        yMin_val = points[0].y;
        yMax_val = points[0].y;

        foreach (Vector3 pt in points)
        {
            if (pt.x > xMax_val) xMax_val = pt.x;
            if (pt.x < xMin_val) xMin_val = pt.x;
            if (pt.y > yMax_val) yMax_val = pt.y;
            if (pt.y < yMin_val) yMin_val = pt.y;
        }

        UpdateDATABox();
    }

    //internal void UpdateDATABox(float aX, float bX, float aY, float bY, float z)
    internal void UpdateDATABox()
    {
        //recalcul le linerenderer en entier
        if (lineRenderer == null) return;

        Vector3[] DATA = points.ToArray();

        ////calcul des positions des points pour ajuster au maximum aux limites de la zone de graphique
        //for (int i = 0; i < DATA.Length; i++)
        //    DATA[i] = new Vector3(aX * DATA[i].x + bX,
        //                          aY * DATA[i].y + bY,
        //                          z);

        lineRenderer.positionCount = DATA.Length;
        lineRenderer.SetPositions(DATA);

        if (graphManager.xMax > graphManager.xMin && graphManager.yMax > graphManager.yMin)
        {
            transform.localScale = new Vector3(graphManager.zonegraph.transform.localScale.x / (graphManager.xMax - graphManager.xMin),
                                               graphManager.zonegraph.transform.localScale.y / (graphManager.yMax - graphManager.yMin),
                                               1);

            transform.localPosition = graphManager.zonegraph.transform.localPosition
                                      - new Vector3(graphManager.zonegraph.transform.localScale.x / 2,
                                                    graphManager.zonegraph.transform.localScale.y / 2,
                                                    0)
                                      - new Vector3(graphManager.xMin * transform.localScale.x,
                                                    graphManager.yMin * transform.localScale.y,
                                                    0);

            ;// + layer_origine);

            float scalemax = Mathf.Max(transform.localScale.x, transform.localScale.y);

            lineRenderer.startWidth = epaisseur / scalemax;
            lineRenderer.endWidth = lineRenderer.startWidth;
        }
    }
}