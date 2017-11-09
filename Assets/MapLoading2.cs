using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoading2 : MonoBehaviour {

    public GameObject Path;
    public GameObject Wall;
    public GameObject Tree;
    public GameObject Tile;
    public Camera cam;
    public int y_index = 305;//305
    public int x_index = 294;//294
    public int speed = 5;

    GameObject[][] DataMap;
    GameObject[][] Tiles;
    GameObject[] TileRow;
    GameObject[] DataRow;
    public int tileSize;
    int length;
    float weight = 1f;
    string heuristic = "None";

    List<List<GameObject>> TileMap = new List<List<GameObject>>();

    float native_width = 1920;
    float native_height = 1080;

    // Use this for initialization
    void Start() {
        int roundedlength = Mathf.CeilToInt(y_index * x_index / (tileSize * tileSize) + y_index);
        length = roundedlength;
        /*
        DataRow = new GameObject[x_index];
        DataMap = new GameObject[y_index][];
        TileRow = new GameObject[tileSize * tileSize];
        Tiles = new GameObject[length][];
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
        */
        cam = Camera.main;
        for (int i = 0; i < y_index; ++i) {
            List<GameObject> temp = new List<GameObject>();
            for (int j = 0; j < x_index; ++j) {
                temp.Add(null);
            }
            TileMap.Add(temp);
        }
        string line;
        bool start = false;
        int y = 0;
        StreamReader reader = new StreamReader("Assets/Map2.txt", Encoding.Default);

        using (reader) {
            do {
                line = reader.ReadLine();
                if (line != null) {
                    if (start == true) {
                        for (int i = 0; i < line.Length; i++) {

                            GameObject type;
                            if (line[i] == '@') {
                                type = Instantiate(Wall, new Vector2(i * (1.28f) + 0.64f, y * (1.28f) + 0.64f), Quaternion.identity);
                                type.transform.tag = "wall";
                                TileMap[y][i] = type;
                            }
                            else if (line[i] == '.') {
                                type = Instantiate(Path, new Vector2(i * 1.28f + 0.64f, y * 1.28f + 0.64f), Quaternion.identity);
                                type.transform.tag = "path";
                                type.AddComponent<BoxCollider2D>();
                                type.AddComponent<SelectNode>();
                                type.GetComponent<SelectNode>().x = i;
                                type.GetComponent<SelectNode>().y = y;

                                TileMap[y][i] = type;

                            }
                            else if (line[i] == 'T') {
                                type = Instantiate(Tree, new Vector2(i * 1.28f + 0.64f, y * 1.28f + 0.64f), Quaternion.identity);
                                type.transform.tag = "tree";
                                TileMap[y][i] = type;
                            }

                        }
                        y++;
                    }
                    else if (line == "map") {
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
    void Update() {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
{
            cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 50;
        }
        if (Input.GetKey(KeyCode.W)) {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + speed * Time.deltaTime, cam.transform.position.z);
        }
        if (Input.GetKey(KeyCode.A)) {
            cam.transform.position = new Vector3(cam.transform.position.x - speed * Time.deltaTime, cam.transform.position.y, cam.transform.position.z);
        }
        if (Input.GetKey(KeyCode.S)) {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - speed * Time.deltaTime, cam.transform.position.z);
        }
        if (Input.GetKey(KeyCode.D)) {
            cam.transform.position = new Vector3(cam.transform.position.x + speed * Time.deltaTime, cam.transform.position.y, cam.transform.position.z);
        }

    }

    private List<GameObject> findPath(GameObject startNode, GameObject endNode) {
        List<GameObject> openList = new List<GameObject>();

        startNode.GetComponent<SelectNode>().g = 0;
        startNode.GetComponent<SelectNode>().f = 0;

        openList.Add(startNode);
        startNode.GetComponent<SelectNode>().opened = true;

        while (openList.Count > 0) {
            //Debug.Log(openList.Count);
            GameObject node = openList[0];
            openList.Remove(openList[0]);
            node.GetComponent<SelectNode>().closed = true;

            if (node == endNode || (node.transform.tag == "waypoint" && node != startNode)) {
                if (node.transform.tag == "waypoint") {
                    composePath(startNode, node);
                    findPath(node, endNode);
                }
                else {
                    return composePath(startNode, endNode);
                }
            }
            List<GameObject> neighbors = findNeighbors(node);
            for (int i = 0, l = neighbors.Count; i < l; ++i) {
                GameObject neighbor = neighbors[i];
                neighbor.gameObject.GetComponent<Renderer>().material.color = Color.black;
                if (neighbor.GetComponent<SelectNode>().closed) {
                    continue;
                }

                //int x = neighbor.GetComponent<SelectNode>().x;
                //int y = neighbor.GetComponent<SelectNode>().y;
                //Destroy(DataMap[y][x]);

                float ng = node.GetComponent<SelectNode>().g + 1;

                if (!neighbor.GetComponent<SelectNode>().opened || ng < neighbor.GetComponent<SelectNode>().g) {
                    neighbor.GetComponent<SelectNode>().g = ng;
                    if (heuristic == "none") {
                        neighbor.GetComponent<SelectNode>().h = 1f;
                        neighbor.GetComponent<SelectNode>().f = neighbor.GetComponent<SelectNode>().g + neighbor.GetComponent<SelectNode>().h;
                    }
                    else {
                        neighbor.GetComponent<SelectNode>().h = calculateHeuristics(node, endNode) * weight;
                        neighbor.GetComponent<SelectNode>().f = neighbor.GetComponent<SelectNode>().g + neighbor.GetComponent<SelectNode>().h;
                    }
                    neighbor.GetComponent<SelectNode>().parent = node;

                    if (!neighbor.GetComponent<SelectNode>().opened) {
                        neighbor.GetComponent<SelectNode>().opened = true;
                        openList.Add(neighbor);
                        openList.Sort(sortByFScore);
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
            findPath(start, goal);
        }
        if (GUI.Button(new Rect((native_width) - 200, 200, 150, 100), "Heuristic: " + heuristic)) {
            if (heuristic == "None") {
                heuristic = "Manhattan";
            }
            else if (heuristic == "Manhattan") {
                heuristic = "Euclidean";
            }
            else {
                heuristic = "None";
            }

        }
        if (GUI.Button(new Rect((native_width) - 200, 350, 150, 100), "Weight Percentage: \n " + weight * 100 + "%")) {
            weight = weight + .25f;
            if (weight > 2) {
                weight = .25f;
            }

        }
        if (GUI.Button(new Rect((native_width) - 200, 500, 150, 100), "Reset")) {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
        if (GUI.Button(new Rect((native_width) - 200, 650, 150, 100), "Other Map")) {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "scene1") {
                SceneManager.LoadScene("scene2");
            }
            if (scene.name == "scene2") {
                SceneManager.LoadScene("scene1");
            }
        }
    }

    static int sortByFScore(GameObject node1, GameObject node2) {
        return node1.GetComponent<SelectNode>().f.CompareTo(node2.GetComponent<SelectNode>().f);
    }

    private List<GameObject> composePath(GameObject startNode, GameObject endNode) {
        List<GameObject> path = new List<GameObject>();
        GameObject curNode = endNode;
        curNode.gameObject.GetComponent<Renderer>().material.color = Color.red;
        while (curNode != startNode) {
            curNode = curNode.GetComponent<SelectNode>().parent;
            curNode.gameObject.GetComponent<Renderer>().material.color = Color.magenta;
            path.Add(curNode);
        }
        curNode.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        return path;
    }

    private List<GameObject> findNeighbors(GameObject currentNode) {
        List<GameObject> neighbors = new List<GameObject>();
        SelectNode node = currentNode.GetComponent<SelectNode>();
        int x = node.x;
        int y = node.y;

        if (TileMap[y - 1][x].gameObject.transform.tag == "path" || TileMap[y - 1][x].gameObject.transform.tag == "goal" || TileMap[y - 1][x].gameObject.transform.tag == "waypoint") {
            neighbors.Add(TileMap[y - 1][x]);
        }
        if (TileMap[y + 1][x].gameObject.transform.tag == "path" || TileMap[y + 1][x].gameObject.transform.tag == "goal" || TileMap[y + 1][x].gameObject.transform.tag == "waypoint") {
            neighbors.Add(TileMap[y + 1][x]);
        }
        if (TileMap[y][x - 1].gameObject.transform.tag == "path" || TileMap[y][x - 1].gameObject.transform.tag == "goal" || TileMap[y][x - 1].gameObject.transform.tag == "waypoint") {
            neighbors.Add(TileMap[y][x - 1]);
        }
        if (TileMap[y][x + 1].gameObject.transform.tag == "path" || TileMap[y][x + 1].gameObject.transform.tag == "goal" || TileMap[y][x + 1].gameObject.transform.tag == "waypoint") {
            neighbors.Add(TileMap[y][x + 1]);
        }
        return neighbors;
    }

    private float calculateHeuristics(GameObject currentNode, GameObject endNode) {
        float to_return = 0;
        if (heuristic == "Manhattan") {
            SelectNode currentNodeInfo = currentNode.GetComponent<SelectNode>();
            SelectNode endNodeInfo = endNode.GetComponent<SelectNode>();
            float dx = currentNodeInfo.x - endNodeInfo.x;
            dx = Math.Abs(dx);
            float dy = currentNodeInfo.y - endNodeInfo.y;
            dy = Math.Abs(dy);
            to_return = dx + dy;
        }
        else if (heuristic == "Euclidean") {
            SelectNode currentNodeInfo = currentNode.GetComponent<SelectNode>();
            SelectNode endNodeInfo = endNode.GetComponent<SelectNode>();
            float dx = currentNodeInfo.x - endNodeInfo.x;
            dx = Math.Abs(dx);
            float dy = currentNodeInfo.y - endNodeInfo.y;
            dy = Math.Abs(dy);
            to_return = (float)Math.Sqrt(dx * dx + dy * dy);
        }
        return to_return;
    }
}
