using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proportions : MonoBehaviour
{
    public GameObject P;
    public GameObject I;
    public GameObject D;

    public RegulateurPID RegulateurPID;

    void Update()
    {
        float p = RegulateurPID.proportionalTerm;
        float i = RegulateurPID.integralTerm;
        float d = RegulateurPID.derivativeTerm;

        float t = RegulateurPID.regulateur_sortie;

        if (float.IsNaN(p)) return;
        if (float.IsNaN(i)) return;
        if (float.IsNaN(d)) return;
        if (t == 0) return;

        p = Mathf.Abs(p);
        i = Mathf.Abs(i);
        d = Mathf.Abs(d);
        t = p + i + d;
        float p_p = p / t;
        float p_i = i / t;
        float p_d = d / t;

        P.transform.localScale = new Vector3(P.transform.localScale.x, p_p / 2, P.transform.localScale.z);
        P.transform.localPosition = new Vector3(P.transform.localPosition.x, p_p / 2, P.transform.localPosition.z);

        I.transform.localScale = new Vector3(I.transform.localScale.x, p_i / 2, I.transform.localScale.z);
        I.transform.localPosition = new Vector3(I.transform.localPosition.x, p_p + p_i / 2, I.transform.localPosition.z);

        D.transform.localScale = new Vector3(D.transform.localScale.x, p_d / 2, D.transform.localScale.z);
        D.transform.localPosition = new Vector3(D.transform.localPosition.x, p_p + p_i + p_d / 2, D.transform.localPosition.z);
    }
}