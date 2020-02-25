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
    double[] tabAngles;
    double[] tabWindSpeed;
    // Start is called before the first frame update
    void Start()
    {
        int lignes = TotalLines(path);
        int colonnes = TotalColonnes(path);
        initTableau(lignes,colonnes);
        printTableau(lignes,colonnes);
        print(getMaxSpeed(38.999, 9.99));
    }

    public Polaire(string path) => this.path = path;

    // Update is called once per frame
    void Update()
    {
        
    }

    double getMaxSpeed(double twa, double windSpeed)
    {
        int IndexAngle = 0;
        int IndexWindSpeed = 0;
        double returnValue;
        Valeur closestAngle = getClosestValue(twa, AngleOuVitesse.Angle);
        Valeur closestWindSpeed = getClosestValue(windSpeed, AngleOuVitesse.Vitesse);

        if (closestAngle.Trouve && closestWindSpeed.Trouve)
        {
            IndexAngle = closestAngle.Index;
            IndexWindSpeed = closestWindSpeed.Index;
            return (tableau[IndexAngle, IndexWindSpeed]);
        }
        else if (closestAngle.Trouve)
        {
            IndexAngle = closestAngle.Index;
            // FORMULE : T(x,y) = T(x-1,y) + [[T(x+1,y)-T(x-1,y)] / (x+1) - (x-1) x [x-(x-1)]]
            double closeVal = tableau[IndexAngle, closestWindSpeed.Index]; // = T(x-1,y)
            double nextVal = tableau[IndexAngle ,closestWindSpeed.Index + 1]; // = T(x+1,y)
            returnValue = closeVal + (((nextVal - closeVal) / (tabWindSpeed[closestWindSpeed.Index + 1] - tabWindSpeed[closestWindSpeed.Index])) * (windSpeed - tabWindSpeed[closestWindSpeed.Index]));
            return returnValue;
        }
        else if (closestWindSpeed.Trouve)
        {
            IndexWindSpeed = closestWindSpeed.Index;
            // FORMULE : T(x,y) = T(x,y-1) + [[T(x,y+1)-T(x,y-1)] / (y+1) - (y-1) x [y-(y-1)]]
            double closeVal = tableau[closestAngle.Index, IndexWindSpeed]; // = T(x,y-1)
            double nextVal = tableau[closestAngle.Index + 1, IndexWindSpeed]; // = T(x,y+1)
            returnValue = closeVal + (((nextVal - closeVal) / (tabAngles[closestAngle.Index + 1] - tabAngles[closestAngle.Index])) * (twa - tabAngles[closestAngle.Index]));
            return returnValue;
        }
        else
        {
            double closeVal = tableau[closestAngle.Index, closestWindSpeed.Index];
            double nextVal = tableau[closestAngle.Index + 1, closestWindSpeed.Index + 1];
            double nextValx = tableau[closestAngle.Index + 1, closestWindSpeed.Index];
            double nextValy = tableau[closestAngle.Index, closestWindSpeed.Index + 1];
            double value1 = closeVal + (((nextValy - closeVal) / (tabAngles[closestAngle.Index + 1] - tabAngles[closestAngle.Index])) * (twa - tabAngles[closestAngle.Index]));
            double value2 = nextValx + (((nextVal - nextValx) / (tabAngles[closestAngle.Index + 1] - tabAngles[closestAngle.Index])) * (twa - tabAngles[closestAngle.Index]));
            print("Value1 = " + value1);
            print("Value2 = " + value2);
            returnValue = value1 + (((value2 - value1) / (tabWindSpeed[closestWindSpeed.Index + 1] - tabWindSpeed[closestWindSpeed.Index])) * (windSpeed - tabWindSpeed[closestWindSpeed.Index]));

            return returnValue;
            //returnValue = closeVal + ((()))
            //returnValue = (closeVal + nextVal + nextValx + nextValy) / 4;
            //return returnValue;
        }
        //print(tableau[IndexAngle, IndexWindSpeed]);
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
        tabAngles = new double[lignes];
        tabWindSpeed = new double[colonnes];
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
                    if (j == 0) tabAngles[i] = parse;
                    if (i == 0) tabWindSpeed[j] = parse;
                }
                else
                {
                    tableau[i, j] = 0;
                    if (j == 0) tabAngles[i] = 0;
                    if (i == 0) tabWindSpeed[j] = 0;
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
            while(tabAngles[i] <= vCherche)
            {
                if (vCherche == tabAngles[i]) retour.Trouve = true;
                retour.Index = i;
                i++;
            }
        }
        else
        {
            while(tabWindSpeed[i] <= vCherche)
            {
                if (vCherche == tabWindSpeed[i]) retour.Trouve = true;
                retour.Index = i;
                i++;
            }
        }
        return retour;
    }
}

