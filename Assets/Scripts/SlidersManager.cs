using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidersManager : MonoBehaviour
{
    public Slider slider1;
    public Slider slider2;

    public TMPro.TMP_Text text1;
    public TMPro.TMP_Text text2;

    public RegulateurPID regulateurPID;

    // Start is called before the first frame update
    void Start()
    {
        slider1.minValue = 0;
        slider2.minValue = 0;

        slider1.maxValue = regulateurPID.randomConsigne_max * 1.1f;
        slider2.maxValue = regulateurPID.randomConsigne_max * 1.1f;
    }

    // Update is called once per frame
    void Update()
    {
        slider1.value = regulateurPID.regulateur_consigne;
        slider2.value = regulateurPID.regulateur_mesure;

        text1.text = regulateurPID.regulateur_consigne.ToString("0.000");
        text2.text = regulateurPID.regulateur_mesure.ToString("0.000");
    }
}
