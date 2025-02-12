using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMovement : MonoBehaviour {

	public GameObject target;
	public float speed;
	private bool can_i_move;
	private float timeToStartMovement;
	// Use this for initialization
	void Start () {
		can_i_move = false;
		timeToStartMovement = 4f;
		StartCoroutine (initHike());	
	}
	
	// Update is called once per frame
	void Update () {
		if (can_i_move) {
			walk ();
		}
	}

	IEnumerator initHike(){
		yield return new WaitForSeconds (timeToStartMovement);
		can_i_move = true;
	}

	void walk(){
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, target.transform.position, step);
	}

}
