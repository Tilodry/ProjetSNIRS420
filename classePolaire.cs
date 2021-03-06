using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Polaire : MonoBehaviour
{
    struct Valeur
    {
        public int Index;
        public bool Trouve;
    }

    enum AngleOuVitesse
    {
        Angle, Vitesse
    };

    double[,] tableau;
    double[] tabAngles;
    double[] tabWindSpeed;



    int selectedValue = 0;

    public string path;


    public Text lb;
    public GameObject cube;
    public GameObject beaume;
    public GameObject mat;


    public double localTwa;
    public double localWindSpeed;
    public double localBeta;


    ~Polaire() { }

    void Start()
    {
        int lignes = TotalLines(path);
        int colonnes = TotalColonnes(path);
        InitTableau(lignes, colonnes);
        PrintTableau(lignes, colonnes);
        print(GetMaxSpeed(30.0001, 10.00001));
        RefreshValue();
    }

    public Polaire(string path) => this.path = path; // Constructeur

    public void InitPolaire(string path) // Permet de forcer l'initialisation de la polaire avec le chemin d'accès au fichier .pol
    {
        this.path = path;
        Start();
    }

    public double GetMaxSpeed(double twa, double windSpeed) // Renvoie la vitesse maximale du bateau en cas de condition optimale de navigation en fonction de l'angle de la voile et la vitesse du vent
    {
        int IndexAngle = 0;
        int IndexWindSpeed = 0;
        double returnValue;
        twa = Mathf.Abs((float)twa);
        Valeur closestAngle = GetClosestValue(twa, AngleOuVitesse.Angle);
        Valeur closestWindSpeed = GetClosestValue(windSpeed, AngleOuVitesse.Vitesse);

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
            double nextVal = tableau[IndexAngle, closestWindSpeed.Index + 1]; // = T(x+1,y)
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
            returnValue = value1 + (((value2 - value1) / (tabWindSpeed[closestWindSpeed.Index + 1] - tabWindSpeed[closestWindSpeed.Index])) * (windSpeed - tabWindSpeed[closestWindSpeed.Index]));

            return returnValue;
        }
    }

    public double GetMaxGite(double twa, double beta, double windSpeed) // Fourni l'angle de Gite maximal par rapport aux valeurs de vitesse du vent et de l'angle 
    {
        double inclinaisonMax = 25;
        double trueWindSpeedMax = 50;
        double pourcInclinaison = inclinaisonMax * (windSpeed / trueWindSpeedMax) * Mathf.Sin((float)((Mathf.PI / 180) * twa));
        return pourcInclinaison;
    }

    private int TotalLines(string path) // Retourne le nombre total de lignes du fichier .pol chargé
    {
        using (StreamReader r = new StreamReader(path))
        {
            int i = 0;
            while (r.ReadLine() != null) i++;
            return i;
        }
    }

    private int TotalColonnes(string path) // Retourne le nombre total de colonnes du fichier .pol chargé
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

    void InitTableau(int lignes, int colonnes) // Initialise le tableau local a partir des données contenues dans le fichier .pol chargé
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


    Valeur GetClosestValue(double vCherche, AngleOuVitesse w) // Récupère la valeur contenue dans le tableau la plus proche de celle demandée
    {
        Valeur retour;
        retour.Index = 0;
        retour.Trouve = false;
        int i = 0;
        if (w == AngleOuVitesse.Angle)
        {
            while (tabAngles[i] <= vCherche)
            {
                if (vCherche == tabAngles[i]) retour.Trouve = true;
                retour.Index = i;
                i++;
            }
        }
        else
        {
            while (tabWindSpeed[i] <= vCherche)
            {
                if (vCherche == tabWindSpeed[i]) retour.Trouve = true;
                retour.Index = i;
                i++;
            }
        }
        return retour;
    }

	void Update() // Pour DEBUG
    {
        if (Input.GetKey(KeyCode.UpArrow)) // Change la valeur du champ sélectionné
        {
            if (selectedValue == 0) localTwa += 0.1;
            else if (selectedValue == 1) localWindSpeed += 0.1;
            else localBeta += 0.1;
            RefreshValue();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (selectedValue == 0) localTwa -= 0.1;
            else if (selectedValue == 1) localWindSpeed -= 0.1;
            else localBeta -= 0.1;
            RefreshValue();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) selectedValue = (selectedValue + 1) % 3; // Change le champs sélectionné
        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectedValue = (selectedValue + 2) % 3;
    }

    void RefreshValue() // Pour DEBUG, permet d'afficher les valeurs des différents champs possibles
    {
        lb.text = "Valeur de TWA : " + localTwa + "\n Valeur de WINDSPEED : " + localWindSpeed + "\n Valeur de BETA : " + localBeta + "\n Valeur de BOATSPEED : " + GetMaxSpeed(localTwa, localWindSpeed) + "\n Valeur de GITE : " + GetMaxGite(localTwa, localBeta, localWindSpeed);
        cube.transform.eulerAngles = new Vector3(0, 0, (float)GetMaxGite(localTwa, localBeta, localWindSpeed));
        RotationBeaume();
    }

	void PrintTableau(int lignes, int colonnes) // Pour DEBUG, affiche le contenu du tableau
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

	void RotationBeaume() // Pour DEBUG, permet de réinitialiser l'angle du beaume
    {
        mat.transform.Rotate(new Vector3(0, (float)localTwa - mat.transform.eulerAngles.y, 0));
    }
}


