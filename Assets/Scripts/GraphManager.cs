using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    //Edit > Project Settings > Graphics and adding it into Always Include Shaders list

    public GameObject cadre;
    public GameObject zonegraph;
    public LineRenderer axeX;
    public LineRenderer axeY;
    public GameObject quadrillageV;
    public GameObject quadrillageH;

    public Vector3 origine;
    //public Vector3 xMax;
    //public Vector3 xMin;
    //public Vector3 yMax;
    //public Vector3 yMin;

    //TODO : comment mettre ça en forme ??????
    public float x_div; //10 sec => 1 div => 50 cm (dans unity)
    public float y_div; // 1 m   => 1 div =>  1 m (dans unity)

    internal float xMin, xMax, yMin, yMax;
    internal float xMin_prec, xMax_prec, yMin_prec, yMax_prec;

    internal float aX, bX, aY, bY;

    public int nbrPointsMax_parSerie;

    //private
    bool firstUpdate;
    Color gris = new Color(0.8f, 0.8f, 0.8f);
    Dictionary<string, SerieManager> graphs;
    bool needToUpdateGraphViewX, needToUpdateGraphViewY;
    List<KeyValuePair<string, SerieManager>> _graphs_list;

    void Start()
    {
        if (nbrPointsMax_parSerie == 0)
        {
            Debug.LogWarning("nbrPointsMax_parSerie = 0 ; nbrPointsMax_parSerie = 100 now by default");
            nbrPointsMax_parSerie = 100;
        }

        // centré
        origine = new Vector3(zonegraph.transform.position.x,
                              zonegraph.transform.position.y,
                              zonegraph.transform.position.z);
        graphs = new Dictionary<string, SerieManager>();
        firstUpdate = true;
    }

    public void _NewCurve(string nomSerie, float epaisseur, Color color, float layer_origine_lower_is_front)
    {
        GameObject go = new GameObject(nomSerie);
        go.transform.SetParent(transform);

        SerieManager sm = go.AddComponent<SerieManager>();
        sm.Link(this);
        graphs.Add(nomSerie, sm);
        _graphs_list = graphs.ToList();

        sm.epaisseur = epaisseur;
        sm.color = color;
        sm.layer_origine = layer_origine_lower_is_front;
    }

    public void _NewPoint(string nomSerie, Vector2 point)
    {
        if (!graphs.ContainsKey(nomSerie))
            _NewCurve(nomSerie, 0.01f, Random.ColorHSV(), -1);

        graphs[nomSerie].Add(point);
    }

    void Update()
    {
        #region MAJ des bornes
        for (int i = 0; i < _graphs_list.Count; i++)
        {
            SerieManager g = _graphs_list[i].Value;
            if (i == 0)
            {
                xMin = g.xMin_val;
                xMax = g.xMax_val;

                yMin = g.yMin_val;
                yMax = g.yMax_val;
            }
            else
            {
                if (xMin > g.xMin_val) xMin = g.xMin_val;
                if (xMax < g.xMax_val) xMax = g.xMax_val;
                if (yMin > g.yMin_val) yMin = g.yMin_val;
                if (yMax < g.yMax_val) yMax = g.yMax_val;
            }
        }
        #endregion

        #region MAJ de la vue du graph si nécessaire
        needToUpdateGraphViewX = (xMin_prec != xMin) || (xMax_prec != xMax);
        needToUpdateGraphViewY = (yMin_prec != yMin) || (yMax_prec != yMax);

        xMin_prec = xMin;
        xMax_prec = xMax;
        yMin_prec = yMin;
        yMax_prec = yMax;

        if (xMin < xMax && yMin < yMax)
        {
            Compute_CoeffRegLin_a_b(xMin, xMax, zonegraph.transform.position.x - zonegraph.transform.lossyScale.x / 2, zonegraph.transform.position.x + zonegraph.transform.lossyScale.x / 2, out aX, out bX);
            Compute_CoeffRegLin_a_b(yMin, yMax, zonegraph.transform.position.y - zonegraph.transform.lossyScale.y / 2, zonegraph.transform.position.y + zonegraph.transform.lossyScale.y / 2, out aY, out bY);

            //Update courbes
            if (needToUpdateGraphViewX || needToUpdateGraphViewY)
            {
                foreach (SerieManager g in graphs.Values)
                    g.UpdateDATABox(aX, bX, aY, bY, zonegraph.transform.position.z);
            }

            //Update axes & quadrillages
            if (needToUpdateGraphViewX) UpdateView_X();
            if (needToUpdateGraphViewY) UpdateView_Y();

            if (firstUpdate)
            {
                firstUpdate = false;
                UpdateView_X();
                UpdateView_Y();
            }
        }
        #endregion
    }

    void UpdateView_X()
    {
        if (xMin <= 0 && xMax >= 0)
            SetAxeY(aX * 0 + bX, 0.01f, Color.black, -0.01f);
        else if (xMax < 0)
            SetAxeY(aX * xMax + bX, 0.01f, Color.black, -0.01f);
        else
            SetAxeY(aX * xMin + bX, 0.01f, Color.black, -0.01f);

        SetQuadrillagesV(0.5f, 0.01f, gris, -0.001f);
    }

    void UpdateView_Y()
    {
        if (yMin <= 0 && yMax >= 0)
            SetAxeX(aY * 0 + bY, 0.01f, Color.black, -0.01f);
        else if (yMax < 0)
            SetAxeX(aY * yMax + bY, 0.01f, Color.black, -0.01f);
        else
            SetAxeX(aY * yMin + bY, 0.01f, Color.black, -0.01f);

        SetQuadrillagesH(1, 0.01f, gris, -0.001f);
    }

    void Compute_CoeffRegLin_a_b(float x1, float x2, float y1, float y2, out float a, out float b)
    {
        a = (y2 - y1) / (x2 - x1);
        b = y1 - a * x1;
    }

    void SetAxeX(float val, float epaisseur, Color color, float layer_origine_lower_is_front)
    {
        Vector3 xMin = new Vector3(zonegraph.transform.position.x - zonegraph.transform.lossyScale.x / 2,
                           val,
                           origine.z + layer_origine_lower_is_front);
        Vector3 xMax = new Vector3(zonegraph.transform.position.x + zonegraph.transform.lossyScale.x / 2,
                           val,
                           origine.z + layer_origine_lower_is_front);
        SetLineRenderer(axeX, new Vector3[] { xMin, xMax }, epaisseur, color);
    }

    void SetAxeY(float val, float epaisseur, Color color, float layer_origine_lower_is_front)
    {
        Vector3 yMin = new Vector3(val,
                           zonegraph.transform.position.y - zonegraph.transform.lossyScale.y / 2,
                           origine.z + layer_origine_lower_is_front);
        Vector3 yMax = new Vector3(val,
                           zonegraph.transform.position.y + zonegraph.transform.lossyScale.y / 2,
                           origine.z + layer_origine_lower_is_front);
        SetLineRenderer(axeY, new Vector3[] { yMin, yMax }, epaisseur, color);
    }

    void SetQuadrillagesV(float ecart, float epaisseur, Color color, float layer_origine_lower_is_front)
    {
        //suppression des anciens quadrillages
        while (quadrillageV.transform.childCount > 0)
            DestroyImmediate(quadrillageV.transform.GetChild(0).gameObject);







        //x+
        for (float i = ecart; i < zonegraph.transform.lossyScale.x / 2; i += ecart)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(quadrillageV.transform);
            LineRenderer q = go.AddComponent<LineRenderer>();
            SetLineRenderer(q,
                            new Vector3[] { new Vector3(origine.x + i,
                                                        zonegraph.transform.position.y - zonegraph.transform.lossyScale.y / 2,
                                                        origine.z + layer_origine_lower_is_front),
                                            new Vector3(origine.x + i,
                                                        zonegraph.transform.position.y + zonegraph.transform.lossyScale.y / 2,
                                                        origine.z + layer_origine_lower_is_front)
                                            },
                            epaisseur,
                            color);
        }

        //x-
        for (float i = -ecart; i > -zonegraph.transform.lossyScale.x / 2; i -= ecart)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(quadrillageV.transform);
            LineRenderer q = go.AddComponent<LineRenderer>();
            SetLineRenderer(q,
                            new Vector3[] { new Vector3(origine.x + i,
                                                        zonegraph.transform.position.y - zonegraph.transform.lossyScale.y / 2,
                                                        origine.z + layer_origine_lower_is_front),
                                            new Vector3(origine.x + i,
                                                        zonegraph.transform.position.y + zonegraph.transform.lossyScale.y / 2,
                                                        origine.z + layer_origine_lower_is_front)
                                            },
                            epaisseur,
                            color);
        }
    }
    //float RoundUpNice(float x)
    //{
    //    Mathf.Ceil(x);

    //    float[] nice = new float[] { 1, 2, 4, 5, 6, 8, 10 };
    //    float val = 10 ^ Mathf.Floor(Mathf.Log10(x)) * nice[[which(x <= 10 ^ floor(log10(x)) * nice)[[1]]]];
    //    return val;
    //}

    void SetQuadrillagesH(float ecart, float epaisseur, Color color, float layer_origine_lower_is_front)
    {
        //suppression des anciens quadrillages
        while (quadrillageH.transform.childCount > 0)
            DestroyImmediate(quadrillageH.transform.GetChild(0).gameObject);

        float ecartMinMax = yMax - yMin;            //1.1
        float y_div_log = Mathf.Log10(ecartMinMax); //0.04
                                                    //        Debug.Log(ecartMinMax.ToString + "[" + yMax + "-" + yMin +  "] => " + y_div_log + " : " + Mathf.Floor(y_div_log) + " to " + Mathf.Ceil(y_div_log));
                                                    //float echelle = Mathf.Pow(10, Mathf.Round(y_div_log)); //1
                                                    //y_div = echelle / 5;
                                                    //Debug.Log(y_div);
        int y_div_log_int = (int)y_div_log;

        float a1 = Mathf.Pow(10, Mathf.Log10(yMin));
        float a2 = Mathf.Pow(10, Mathf.Log10(yMax));

        DebugManager.instance._PRINT(a1.ToString("0.0000") + " - " + a2.ToString("0.0000"));


        //List<float> val = new List<float>();

        //DebugManager.instance._PRINT(
        //    yMax.ToString("0.0000") + " - " + yMin.ToString("0.0000") + " = " + ecartMinMax.ToString("0.0000") + 
        //    "\n=> en log10 : " + y_div_log.ToString("0.00") + 
        //    "\n" + Mathf.Floor(y_div_log) + " -> " + Mathf.Ceil(y_div_log));
        //DebugManager.instance._PRINT(y_div.ToString("0.0000"));



        //y+
        for (float i = ecart; i < zonegraph.transform.lossyScale.y / 2; i += ecart)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(quadrillageH.transform);
            LineRenderer q = go.AddComponent<LineRenderer>();
            SetLineRenderer(q,
                            new Vector3[] { new Vector3(zonegraph.transform.position.x - zonegraph.transform.lossyScale.x / 2,
                                                        origine.y + i,
                                                        origine.z + layer_origine_lower_is_front),
                                            new Vector3(zonegraph.transform.position.x + zonegraph.transform.lossyScale.x / 2,
                                                        origine.y + i,
                                                        origine.z + layer_origine_lower_is_front)
                                            },
                            epaisseur,
                            color);
        }

        //y-
        for (float i = -ecart; i > -zonegraph.transform.lossyScale.y / 2; i -= ecart)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(quadrillageH.transform);
            LineRenderer q = go.AddComponent<LineRenderer>();
            SetLineRenderer(q,
                            new Vector3[] { new Vector3(zonegraph.transform.position.x - zonegraph.transform.lossyScale.x / 2,
                                                        origine.y + i,
                                                        origine.z + layer_origine_lower_is_front),
                                            new Vector3(zonegraph.transform.position.x + zonegraph.transform.lossyScale.x / 2,
                                                        origine.y + i,
                                                        origine.z + layer_origine_lower_is_front)
                                            },
                            epaisseur,
                            color);
        }
    }

    void SetLineRenderer(LineRenderer lr, Vector3[] points, float epaisseur, Color color)
    {
        lr.enabled = true;
        //tracé
        lr.positionCount = points.Length;
        lr.SetPositions(points);
        //épaisseur
        lr.startWidth = epaisseur;
        lr.endWidth = lr.startWidth;
        //couleur
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = color;

        lr.useWorldSpace = false;
    }
}