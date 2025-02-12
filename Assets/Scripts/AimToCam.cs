using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimToCam : MonoBehaviour {

	public GameObject characterReference;
	private Vector3 characterPosition;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		characterPosition = characterReference.transform.position;
		transform.rotation = Quaternion.LookRotation (transform.position - characterPosition);
	}
}
