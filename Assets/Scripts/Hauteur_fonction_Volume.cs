using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hauteur_fonction_Volume : MonoBehaviour
{
    //syst�me : fuite = 50Lps, vanne vitesse � 0.1
    //Kp = 2000 ; Ki = 1500000 ; Kd = 90000 ; out = 0-200 ; integral period = 5 sec

    #region PARAMETRES
    public enum Geometrie { cube, cylindre }
    public Geometrie geometrie;
    //cube
    public float Longueur_m, largeur_m;
    //cylindre
    public float Diametre_m;
    //commun
    public float Section_m2;

    //fuite du syst�me
    public float debit_fuite_Lps; //TODO : � remplacer par mod�le physique : varie en fonction de la hauteur => pression => d�bit (seul param�tre section de sortie)
    #endregion

    #region VARIABLES
    public float Hauteur_m;
    
    //volumes
    public float Volume_L;
    public float Volume_L_fuite;
    public float Volume_L_remplissage;

    //vanne regul�e de remplissage de la cuve
    public float vanne_regulante_position_souhaitee;
    public float vanne_regulante_position;
    public float vanne_regulante_position_erreur;
    public float vanne_regulante_vitesse_positionnement;

    public float debit_remplissage_Lps;
    #endregion

    void Start()
    {
        switch (geometrie)
        {
            case Geometrie.cube:
                Longueur_m = transform.localScale.x;
                largeur_m = transform.localScale.z;
                Hauteur_m = transform.localScale.y;
                Section_m2 = Longueur_m * largeur_m;
                break;

            case Geometrie.cylindre:
                Diametre_m = transform.localScale.x;
                Hauteur_m = transform.localScale.y * 2;
                Section_m2 = Mathf.PI * Diametre_m * Diametre_m / 4;
                break;
        }

        //volume initial
        Volume_L = Section_m2 * Hauteur_m * 1000;
    }

    void Update()
    {
        //Calcul du d�bit de remplissage en fonction de la vitesse d'action de la vanne
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

        //mise � jour du mod�le 3D
        switch (geometrie)
        {
            case Geometrie.cube:
                transform.localScale = new Vector3(Longueur_m, Hauteur_m, largeur_m);
                transform.position = new Vector3(transform.position.x, Hauteur_m / 2, transform.position.z);
                break;

            case Geometrie.cylindre:
                transform.localScale = new Vector3(Diametre_m, Hauteur_m / 2, Diametre_m);
                transform.position = new Vector3(transform.position.x, Hauteur_m / 2, transform.position.z);
                break;
        }
    }
}