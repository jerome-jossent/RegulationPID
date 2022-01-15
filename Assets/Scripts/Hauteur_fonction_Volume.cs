using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hauteur_fonction_Volume : MonoBehaviour
{
    //système : fuite = 50Lps, vanne vitesse à 0.1
    //Kp = 2000 ; Ki = 1500000 ; Kd = 90000 ; out = 0-200 ; integral period = 5 sec

    #region PARAMETRES
    public float Diametre_m { get; set; }
    public TMPro.TMP_InputField if_Diametre_m;
    public GameObject cuveTransparente;
    public GameObject niveau;

    public float Section_m2;
    public TMPro.TMP_Text t_Section_m2;

    //fuite du système
    public float debit_fuite_Lps { get; set; } //TODO : à remplacer par modèle physique : varie en fonction de la hauteur => pression => débit (seul paramètre section de sortie)
    public TMPro.TMP_InputField if_debit_fuite_Lps;
    #endregion

    #region VARIABLES
    public float Hauteur_m;
    public TMPro.TMP_Text t_Hauteur_m;

    //volumes
    public float Volume_L;
    public TMPro.TMP_Text t_Volume_L;
    public float Volume_L_fuite;
    public float Volume_L_remplissage;

    //vanne regulée de remplissage de la cuve
    public float vanne_regulante_position_souhaitee;
    public float vanne_regulante_position;
    public float vanne_regulante_position_erreur;
    public float vanne_regulante_vitesse_positionnement { get; set; }
    public TMPro.TMP_InputField if_vanne_regulante_vitesse_positionnement;

    public RegulateurPID regulateurPID;

    public float debit_remplissage_Lps;
    #endregion

    void Start()
    {

        Diametre_m = transform.localScale.x;
        Hauteur_m = transform.localScale.y * 2;
        Section_m2 = Mathf.PI * Diametre_m * Diametre_m / 4;

        //volume initial
        Volume_L = Section_m2 * Hauteur_m * 1000;

        InitParamFromUI();
    }

    private void InitParamFromUI()
    {
        _Set_vanne_regulante_vitesse_positionnement(if_vanne_regulante_vitesse_positionnement.text);
        _Set_debit_fuite_Lps(if_debit_fuite_Lps.text);
        _Set_Diametre_m(if_Diametre_m.text);
    }

    public void _Set_vanne_regulante_vitesse_positionnement(string text) { SetValue(text, "vanne_regulante_vitesse_positionnement"); }
    public void _Set_debit_fuite_Lps(string text) { SetValue(text, "debit_fuite_Lps"); }
    public void _Set_Diametre_m(string text)
    {
        SetValue(text, "Diametre_m");
        Section_m2 = Mathf.PI * Diametre_m * Diametre_m / 4;

        cuveTransparente.transform.localScale = new Vector3(Diametre_m + 0.01f, cuveTransparente.transform.localScale.y, Diametre_m + 0.01f);
        niveau.transform.localScale = new Vector3(Diametre_m+0.01f, niveau.transform.localScale.y, Diametre_m + 0.01f);
    }

    void SetValue(string textinput, string variablename)
    {
        float? v = GetValueFromText(textinput);
        if (v != null)
        {
            System.Reflection.PropertyInfo propName = this.GetType().GetProperty(variablename);
            if (propName != null)
                propName.SetValue(this, v.Value, null);
        }
    }

    public static float? GetValueFromText(string text)
    {
        if (text == null) return null;
        if (text == "") return null;

        if (float.TryParse(text, out var v))
            return v;

        return null;
    }

    void Update()
    {
        //Calcul du débit de remplissage en fonction de la vitesse d'action de la vanne
        vanne_regulante_position_erreur = vanne_regulante_position_souhaitee - vanne_regulante_position;
        vanne_regulante_position = vanne_regulante_position + vanne_regulante_vitesse_positionnement * vanne_regulante_position_erreur * Time.deltaTime;
        debit_remplissage_Lps = vanne_regulante_position;

        //calcul des volumes
        Volume_L_fuite = debit_fuite_Lps * Time.deltaTime;
        Volume_L_remplissage = debit_remplissage_Lps * Time.deltaTime;
        Volume_L += Volume_L_remplissage - Volume_L_fuite;
        if (Volume_L < 0) Volume_L = 0;

        //calcul de la hauteur
        Hauteur_m = Volume_L / (1000 * Section_m2);

        //mise à jour du modèle 3D
        transform.localScale = new Vector3(Diametre_m, Hauteur_m / 2, Diametre_m);
        transform.position = new Vector3(transform.position.x, Hauteur_m / 2, transform.position.z);

        niveau.transform.position = new Vector3(niveau.transform.position.x, regulateurPID.regulateur_consigne, niveau.transform.position.z);

        //UI
        t_Hauteur_m.text = Hauteur_m.ToString("0.000");
        t_Volume_L.text = Volume_L.ToString("0.0");
        t_Section_m2.text = Section_m2.ToString("0.000");

    }
}