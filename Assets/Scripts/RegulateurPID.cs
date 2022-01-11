using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegulateurPID : MonoBehaviour
{
    public GraphManager graphsManager;
    public Hauteur_fonction_Volume systeme;

    public float regulateur_sortie;
    public float regulateur_mesure;
    public float regulateur_consigne;
    public float regulateur_erreur;

    public float Kp = 1;
    public float Ki = 0;
    public float Kd = 0;
    public float OutMax = 0;
    public float OutMin = 0;

    public float integralTermPeriod_sec;

    public float proportionalTerm;
    public float integralTerm;
    public int integralTerms_count;
    public float derivativeTerm;

    PidController pidController;

    float t_newpoint = 0;
    const float t_period = 0.1f;

    public float pourcentage_0_1_validation_erreur;
    public float pourcentage_0_1_validation_erreur_duree_sec;
    float validation_erreur_duree_sec;
    public bool autoRandomConsigne;
    public float randomConsigne_min;
    public float randomConsigne_max;

    void Start()
    {
        //init régulateur
        pidController = new PidController();

        //init courbes
        graphsManager._NewCurve(gameObject.name, 0.03f, Color.blue, -0.009f);
        graphsManager._NewCurve(gameObject.name + " consigne", 0.03f, Color.red, -0.008f);

        //graphsManager._NewPoint(gameObject.name, new Vector2(-13, -1));
        //graphsManager._NewPoint(gameObject.name + " consigne", new Vector2(-13, -2));
    }

    void Update() //TODO each frame => c'est peut être un peu beaucoup !?
    {
        float t = Time.time;
        //update parameters
        pidController.Kp = Kp;
        pidController.Ki = Ki;
        pidController.Kd = Kd;
        pidController.OutMax = OutMax;
        pidController.OutMin = OutMin;
        pidController.regulateur_consigne = regulateur_consigne;
        regulateur_mesure = systeme.Hauteur_m;
        pidController.integralTermPeriod_sec = integralTermPeriod_sec;

        //PID Regulation set input, compute, get output
        pidController.regulateur_mesure = systeme.Hauteur_m;
        pidController.ControlVariable(Time.time, Time.deltaTime);
        regulateur_sortie = pidController.regulateur_sortie;

        //pour info
        proportionalTerm = pidController.proportionalTerm;
        integralTerm = pidController.integralTerm;
        integralTerms_count = pidController.integralTerms.Count;
        derivativeTerm = pidController.derivativeTerm;
        regulateur_erreur = pidController.regulateur_erreur;

        //Update Gameobject
        systeme.vanne_regulante_position_souhaitee = regulateur_sortie;

        //Update Graph
        if (t > t_newpoint)
        {
            t_newpoint = t + t_period;
            graphsManager._NewPoint(gameObject.name, new Vector2(t, systeme.Hauteur_m));
            graphsManager._NewPoint(gameObject.name + " consigne", new Vector2(t, regulateur_consigne));
        }

        if (autoRandomConsigne)
        {
            if (Mathf.Abs(pidController.regulateur_erreur) <= pourcentage_0_1_validation_erreur)
            {
                if (t - validation_erreur_duree_sec >= pourcentage_0_1_validation_erreur_duree_sec)
                {
                    regulateur_consigne = (float)(int)(Random.Range(randomConsigne_min, randomConsigne_max)*100)/100;
                    //validation_erreur_duree_sec = t;
                }
            }
            else
            {
                validation_erreur_duree_sec = t;
            }
        }
    }

    public void _SetAutoRandomConsigne(bool value)
    {
        autoRandomConsigne = value;
    }
}