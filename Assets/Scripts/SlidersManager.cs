using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidersManager : MonoBehaviour
{
    public Slider slider_consigne;
    public Slider slider_mesure;

    public TMPro.TMP_Text text_consigne;
    public TMPro.TMP_Text text_mesure;
    public TMPro.TMP_Text text_erreur;

    public RegulateurPID regulateurPID;

    void Start()
    {
        slider_consigne.minValue = 0;
        slider_mesure.minValue = 0;

        slider_consigne.maxValue = regulateurPID.randomConsigne_max * 1.1f;
        slider_mesure.maxValue = regulateurPID.randomConsigne_max * 1.1f;
    }

    void Update()
    {
        slider_mesure.value = regulateurPID.regulateur_mesure;

        text_consigne.text = regulateurPID.regulateur_consigne.ToString("0.000");
        text_mesure.text = regulateurPID.regulateur_mesure.ToString("0.000");
        text_erreur.text = regulateurPID.regulateur_erreur.ToString("0.000");
    }

    public void _SetConsigne(float val) //slider event
    {
        regulateurPID.regulateur_consigne = val;
    }

    public void _SetConsigneAuto(float val) //slider event
    {
        regulateurPID.regulateur_consigne = val;
        slider_consigne.value = regulateurPID.regulateur_consigne;
    }
}