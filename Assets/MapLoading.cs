using System;
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
    public Camera cam;
    public int y_index = 3;//305
    public int x_index = 3;//294

    GameObject[][] DataMap;
    GameObject[][] Tiles;
    GameObject[] TileRow;
    GameObject[] DataRow;
    public int tileSize;
    int length;


    float native_width = 1920;
    float native_height = 1080;

    // Use this for initialization
    void Start() {
        int roundedlength = Mathf.CeilToInt(y_index * x_index / (tileSize * tileSize) + y_index);
        length = roundedlength;
        DataRow = new GameObject[x_index];
        DataMap = new GameObject[y_index][];
        TileRow = new GameObject[tileSize * tileSize];
        Tiles = new GameObject[length][];
        string line;
        int y = 0;
        bool start = false;
        for(int i = 0; i < y_index; i++)
        {
            for(int j = 0; j < x_index; j++)
            {
                DataRow[j] = null;
            }
            DataMap[i] = DataRow;
        }
        int tilecount = 0;

        StreamReader reader = new StreamReader("Assets/Map2.txt", Encoding.Default);

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
                                type.transform.tag = "wall";
                                DataMap[y][i] = type;
                            }
                            else if (line[i] == '.')
                            {
                                type = Instantiate(Path, new Vector2(i * 1.28f + 0.64f, y * 1.28f + 0.64f), Quaternion.identity);
                                type.transform.tag = "path";
                                type.AddComponent<BoxCollider2D>();
                                type.AddComponent<SelectNode>();
                                type.GetComponent<SelectNode>().x = i;
                                type.GetComponent<SelectNode>().y = y;

                                DataMap[y][i] = type;
                            }
                            else if (line[i] == 'T')
                            {
                                type = Instantiate(Tree, new Vector2(i * 1.28f + 0.64f, y * 1.28f+ 0.64f), Quaternion.identity);
                                type.transform.tag = "tree";
                                DataMap[y][i] = type;
                            }
                            //Debug.Log("y:" + y + "     x:"  + i + "     type:" + DataMap[y][i]);
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
        /*
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
                        Instantiate(Tile, new Vector3(j * 1.28f+ (0.64f * tileSize), i * 1.28f + (0.64f * tileSize), 1f), Quaternion.identity);
                        tilecount++;
                        
                    }
                }
            }
        }
        */
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    private List<GameObject> findPath(GameObject startNode, GameObject endNode) {
        List<GameObject> openList = new List<GameObject>();

        startNode.GetComponent<SelectNode>().g = 0;
        startNode.GetComponent<SelectNode>().f = 0;

        openList.Add(startNode);
        startNode.GetComponent<SelectNode>().opened = true;

        while(openList.Count > 0) {
            //Debug.Log(openList.Count);
            GameObject node = openList[0];
            openList.Remove(openList[0]);
            //Debug.Log(openList.Count);
            node.GetComponent<SelectNode>().closed = true;

            if (node == endNode) {
                return composePath(startNode,endNode);
            }
            //Debug.Log(node);
            //Destroy(node);
           
            Debug.Log(startNode.GetComponent<SelectNode>().y);
            Debug.Log(startNode.GetComponent<SelectNode>().x);
            Destroy(DataMap[startNode.GetComponent<SelectNode>().y][startNode.GetComponent<SelectNode>().x]);

            Destroy(DataMap[node.GetComponent<SelectNode>().y][node.GetComponent<SelectNode>().x]);
            List<GameObject> neighbors = findNeighbors(node);
            for (int i = 0, l = neighbors.Count; i<l; ++i) {
                GameObject neighbor = neighbors[i];
                if (neighbor.GetComponent<SelectNode>().closed) {
                    continue;
                }

                int x = neighbor.GetComponent<SelectNode>().x;
                int y = neighbor.GetComponent<SelectNode>().y;

                float ng = node.GetComponent<SelectNode>().g + 1;
                if(!neighbor.GetComponent<SelectNode>().opened || ng < neighbor.GetComponent<SelectNode>().g) {
                    neighbor.GetComponent<SelectNode>().g = ng;
                    neighbor.GetComponent<SelectNode>().f = neighbor.GetComponent<SelectNode>().g + 1;
                    neighbor.GetComponent<SelectNode>().parent = node;

                    if (!neighbor.GetComponent<SelectNode>().opened) {
                        openList.Add(neighbor);
                        neighbor.GetComponent<SelectNode>().opened = true;
                    }
                    else {
                        openList.Sort(sortByFScore);
                    }
                }
            }
        }

        return null;
    }

    private void OnGUI() {
        float rx = Screen.width / native_width;
        float ry = Screen.height / native_height;
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));

        if (GUI.Button(new Rect((native_width) - 200, 50, 150, 100), "Start A*")) {
            GameObject start = GameObject.FindGameObjectWithTag("start");
            GameObject goal = GameObject.FindGameObjectWithTag("goal");
            findPath(start,goal);
        }
    }

    static int sortByFScore(GameObject node1, GameObject node2) {
        return node1.GetComponent<SelectNode>().f.CompareTo(node2.GetComponent<SelectNode>().f);
    }

    private List<GameObject> composePath(GameObject startNode, GameObject endNode) {
        List<GameObject> path = new List<GameObject>();
        GameObject curNode = endNode;
        while(curNode != startNode) {
            curNode = curNode.GetComponent<SelectNode>().parent;
            curNode.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            path.Add(curNode);
        }
        return path;
    }

    private List<GameObject> findNeighbors(GameObject currentNode) {
        List<GameObject> neighbors = new List<GameObject>();
        SelectNode node = currentNode.GetComponent<SelectNode>();
        int x = node.x;
        int y = node.y;

        if (DataMap[y-1][x].gameObject.transform.tag == "path") {
            neighbors.Add(DataMap[x - 1][y]);
        }
        if (DataMap[y + 1][x].gameObject.transform.tag == "path") {
            neighbors.Add(DataMap[x - 1][y]);
        }
        if (DataMap[y][x-1].gameObject.transform.tag == "path") {
            neighbors.Add(DataMap[x - 1][y]);
        }
        if (DataMap[y][x+1].gameObject.transform.tag == "path") {
            neighbors.Add(DataMap[x - 1][y]);
        }
        return neighbors;
    }
}
