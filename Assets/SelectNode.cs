using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNode : MonoBehaviour {

    public int x;
    public int y;
    public float g;
    public float f;
    public float h;
    public bool opened = false;
    public bool closed = false;
    public GameObject parent = null;
    Color originalColor;

	// Use this for initialization
	void Start () {
        originalColor = gameObject.GetComponent<Renderer>().material.color;

    }
	
	// Update is called once per frame
	void Update () {
        if(transform.tag == "waypoint") {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }

	}

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            if (GameObject.FindGameObjectWithTag("start") == null) {
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                transform.tag = "start";
            }
            else {
                GameObject asdf = GameObject.FindGameObjectWithTag("start");
                asdf.transform.tag = "path";
                asdf.GetComponent<Renderer>().material.color = originalColor;
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                transform.tag = "start";
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            if (GameObject.FindGameObjectWithTag("goal") == null) {
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                transform.tag = "goal";
            }
            else {
                GameObject asdf = GameObject.FindGameObjectWithTag("goal");
                asdf.transform.tag = "path";
                asdf.GetComponent<Renderer>().material.color = originalColor;
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                transform.tag = "goal";
            }
        }
        if (Input.GetMouseButton(2)) {
            transform.tag = "tree";
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Tree");
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            transform.tag = "waypoint";
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
}
