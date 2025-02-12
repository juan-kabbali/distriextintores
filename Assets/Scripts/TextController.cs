using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {

	private Scrollbar scroll;

	// Use this for initialization
	void Start () {
		scroll = GameObject.Find ("Scrollbar Vertical").GetComponent<Scrollbar> ();

	}
	
	// Update is called once per frame
	void Update () {
		scroll.value = scroll.value - 0.003f;

	}
}
