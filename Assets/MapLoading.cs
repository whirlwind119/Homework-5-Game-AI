using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class MapLoading : MonoBehaviour {

    public GameObject Path;
    public GameObject Wall;
    public GameObject Tree;
    public GameObject Tile;

    GameObject[][] DataMap;
    GameObject[][] Tiles;
    GameObject[] TileRow;
    GameObject[] DataRow;
    public int tileSize;
    int length;
    // Use this for initialization
    void Start() {
        int roundedlength = Mathf.CeilToInt(294 * 305 / (tileSize * tileSize) + 294);
        length = roundedlength;
        DataRow = new GameObject[294];
        DataMap = new GameObject[305][];
        TileRow = new GameObject[tileSize * tileSize];
        Tiles = new GameObject[length][];
        string line;
        int y = 0;
        bool start = false;
        for(int i = 0; i < 305; i++)
        {
            for(int j = 0; j < 294; j++)
            {
                DataRow[j] = null;
            }
            DataMap[i] = DataRow;
        }
        int tilecount = 0;

        StreamReader reader = new StreamReader("Assets/Map.txt", Encoding.Default);

        using (reader)
        {
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    if (start == true)
                    {
                        for (int i = 0; i < line.Length; i++)
                        {
                            GameObject type;
                            if (line[i] == '@')
                            {
                                type = Instantiate(Wall, new Vector2(i * (1.28f) + 0.64f, y * (1.28f) + 0.64f), Quaternion.identity);
                                DataMap[y][i] = type;
                            }
                            else if (line[i] == '.')
                            {
                                type = Instantiate(Path, new Vector2(i * 1.28f + 0.64f, y * 1.28f + 0.64f), Quaternion.identity);
                                DataMap[y][i] = type;
                            }
                            else if (line[i] == 'T')
                            {
                                type = Instantiate(Tree, new Vector2(i * 1.28f + 0.64f, y * 1.28f+ 0.64f), Quaternion.identity);
                                DataMap[y][i] = type;
                            }
                        }
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
        for (int i = 0; i < DataMap.Length; i++)
        {
            if (i % tileSize == 0)
            {
                for (int j = 0; j < DataMap[i].Length; j++)
                {
                    if (j % tileSize == 0)
                    {
                        //make tile starting here
                        int k = 0;
                        int counter = 0;
                        while(k < tileSize)
                        {
                            int l = 0;
                            while (l < tileSize)
                            {
                                if((i+k < DataMap.Length) && (j + l) < DataMap[i+k].Length)
                                {
                                    TileRow[counter] = DataMap[i + k][j + l];
                                    counter++;
                                }
                                l++;
                            }
                            k++;
                        }
                        Tiles[tilecount] = TileRow;
                        Instantiate(Tile, new Vector3(j * 1.28f+ (0.64f * tileSize), i * 1.28f + (0.64f * tileSize), 1.1f), Quaternion.identity);
                        tilecount++;
                        
                    }
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
