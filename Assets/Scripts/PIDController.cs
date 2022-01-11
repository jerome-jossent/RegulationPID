using System;
using System.Collections.Generic;
using System.Linq;

public class PidController
{
    #region PARAMETRES
    public float regulateur_consigne;
    public float Kp;
    public float Ki;
    public float integralTermPeriod_sec;
    public float Kd;
    public float OutMax;
    public float OutMin;
    #endregion

    #region VARIABLES
    public float regulateur_erreur;
    public float proportionalTerm;
    public float integralTerm;
    public float derivativeTerm;
    public List<Vec2> integralTerms = new List<Vec2>();
    public float regulateur_mesure
    {
        get { return _regulateur_mesure; }
        set
        {
            regulateur_mesure_prec = _regulateur_mesure;
            _regulateur_mesure = value;
        }
    }
    float _regulateur_mesure;
    public float regulateur_mesure_prec;
    public float regulateur_sortie;
    #endregion

    public struct Vec2
    {
        public float T;
        public float V;
    }

    public PidController() { }

    public float ControlVariable(float now_sec, float deltaTime_sec)
    {
        regulateur_erreur = regulateur_consigne - regulateur_mesure;

        // proportionnelle
        proportionalTerm = Kp * regulateur_erreur;

        // intégrale
        integralTerms.Add(new Vec2 { T = now_sec, V = regulateur_erreur * deltaTime_sec });
        float t_trop_tard = now_sec - integralTermPeriod_sec;
        for (int i = 0; i < integralTerms.Count; i++)
        {
            Vec2 TV = integralTerms[i];
            if (TV.T < t_trop_tard)
            {
                integralTerms.RemoveAt(i);
                i--;
            }
        }
        integralTerm = integralTerms.Select(item => item.V).Average() * Ki;

        // derivée
        float dInput = regulateur_mesure - regulateur_mesure_prec;
        derivativeTerm = Kd * (dInput / deltaTime_sec);

        regulateur_sortie = proportionalTerm + integralTerm - derivativeTerm;
        regulateur_sortie = Clamp(regulateur_sortie);
        return regulateur_sortie;
    }

    float Clamp(float variableToClamp)
    {
        if (variableToClamp <= OutMin) { return OutMin; }
        if (variableToClamp >= OutMax) { return OutMax; }
        return variableToClamp;
    }
}