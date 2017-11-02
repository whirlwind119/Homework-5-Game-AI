using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class MapLoading : MonoBehaviour {

    public GameObject Path;
    public GameObject Wall;
    public GameObject Tree;


	// Use this for initialization
	void Start () {
        Load_file("Assets/Map.txt");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Load_file(string filename)
    {
        string line;
        float y = 0;
        bool start = false;


        StreamReader reader = new StreamReader(filename, Encoding.Default);

        using (reader)
        {
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    if(start == true)
                    {
                        create_row(line, y);
                        y++;
                    }
                    else if (line == "map")
                    {
                        start = true;
                    }
                }
            } while (line != null);
            reader.Close();

        }
    }

    private void create_row(string line, float rowNum)
    {
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '@')
            {
                Instantiate(Wall, new Vector2(i, rowNum), Quaternion.identity);
            }
            else if (line[i] == '.')
            {
                Instantiate(Path, new Vector2(i, rowNum), Quaternion.identity);
            }
            else if (line[i] == 'T')
            {
                Instantiate(Tree, new Vector2(i, rowNum), Quaternion.identity);
            }
        }
    }
}
