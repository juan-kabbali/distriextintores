using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPort : MonoBehaviour {

	public bool isRight;
	public GameObject port;
	private Vector3 move;
	private float limit = -5.75f;

	// Use this for initialization
	void Start () {
		move = port.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (isRight && port.transform.position.z > limit) {
			port.transform.Translate(Vector3.right * Time.deltaTime);	
		} 
		if (!isRight && port.transform.position.z < (-1*limit)){
			port.transform.Translate(Vector3.left* Time.deltaTime);	
		}
	}

}
