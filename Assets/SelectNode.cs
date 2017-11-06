using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNode : MonoBehaviour {

    public int x;
    public int y;
    public float g;
    public float f;
    public bool opened = false;
    public bool closed = false;
    public GameObject parent = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
            transform.tag = "start";
        }
        if (Input.GetMouseButtonDown(1)) {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            transform.tag = "goal";
        }
    }
}
