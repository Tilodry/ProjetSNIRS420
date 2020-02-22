using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

public class Polaire : MonoBehaviour
{

    struct Valeur{
        public int Index;
        public bool Trouve;
    }

    enum AngleOuVitesse
    {
        Angle,Vitesse
    };

    public string path;
    double[,] tableau;
    double[] angles;
    double[] windSpeed;
    // Start is called before the first frame update
    void Start()
    {
        int lignes = TotalLines(path);
        int colonnes = TotalColonnes(path);
        initTableau(lignes,colonnes);
        printTableau(lignes,colonnes);
        getMaxSpeed(36, 13);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    double getMaxSpeed(double twa, double windSpeed)
    {
        int indexAngle;
        int IndexWindSpeed;
        Valeur closestAngle = getClosestValue(twa, AngleOuVitesse.Angle);
        Valeur closestWindSpeed = getClosestValue(windSpeed, AngleOuVitesse.Vitesse);
        if (closestAngle.Trouve) indexAngle = closestAngle.Index;
        else;
        if (closestWindSpeed.Trouve) IndexWindSpeed = closestWindSpeed.Index;
        else


        print("CLOSEST ANGLE = " + closestAngle.Index + "VALEUR TROUVÉE :" + closestAngle.Trouve);
        print("CLOSEST WINDSPEED = " + closestWindSpeed.Index + "VALEUR TROUVÉE :" + closestWindSpeed.Trouve);

        return 0.0;
    }
    double getMaxGite(double twa, double windSpeed)
    {
        return 0.0;
    }

    private int TotalLines(string path)
    {
        using (StreamReader r = new StreamReader(path))
        {
            int i = 0;
            while (r.ReadLine() != null) i++;
            return i;
        }
    }

    private int TotalColonnes(string path)
    {
        using (StreamReader r = new StreamReader(path))
        {
            string tab = r.ReadLine();
            char[] check = tab.ToCharArray();
            int i = 0;
            int retour = 0;
            while (i < tab.Length)
            {
                if (check[i] == '\t') retour++;
                i++;
            }
            r.Close();
            return retour + 1;
        }
    }

    void initTableau(int lignes, int colonnes)
    {
        
        tableau = new double[lignes, colonnes];
        angles = new double[lignes];
        windSpeed = new double[colonnes];
        StreamReader reader = new StreamReader(path);
        string tab = null;

        for (int i = 0; i < lignes; i++)
        {
            tab = reader.ReadLine();
            string[] result = tab.Split('\t');
            for (int j = 0; j < result.Length; j++)
            {
                if (double.TryParse(result[j], NumberStyles.Any, CultureInfo.InvariantCulture, out double parse))
                {
                    tableau[i, j] = parse;
                    if (j == 0) angles[i] = parse;
                    if (i == 0) windSpeed[j] = parse;
                }
                else
                {
                    tableau[i, j] = 0;
                    if (j == 0) angles[i] = 0;
                    if (i == 0) windSpeed[j] = 0;
                }
            }
        }
    }

    void printTableau(int lignes, int colonnes)
    {
        string sortie = "";
        for (int i = 0; i < lignes; i++)
        {
            for (int j = 0; j < colonnes; j++)
            {
                sortie += "\t" + tableau[i, j];
            }
            print(sortie);
            sortie = "";
        }
    }

    Valeur getClosestValue(double vCherche, AngleOuVitesse w)
    {
        Valeur retour;
        retour.Index = 0;
        retour.Trouve = false;
        int i = 0;
        if (w == AngleOuVitesse.Angle)
        {
            while(angles[i] <= vCherche)
            {
                if (vCherche == angles[i]) retour.Trouve = true;
                retour.Index = i;
                i++;
            }
        }
        else
        {
            while(windSpeed[i] <= vCherche)
            {
                if (vCherche == angles[i]) retour.Trouve = true;
                retour.Index = i;
                i++;
            }
        }
        return retour;
    }
}

