using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlamerCntrller : MonoBehaviour {

	public ParticleSystem fire;
	public ParticleSystem water;
	private float rateOverTimeOff;
	private ParticleSystem.EmissionModule emission;
	private Animator anim;
	public float smooth = 1f;
	private Vector3 targetAngles;

	public GameObject scrollView;
	public Text Congratulations;
	private bool isPutOut = false;
	private bool isTurned = false;
	private bool isSartAnim = false;
	// Use this for initialization
	void Start () {
		rateOverTimeOff = fire.main.maxParticles;
		emission = fire.emission;
		Congratulations.gameObject.SetActive(false);
		anim = GetComponent<Animator> ();
		StartCoroutine (initPutOutFire());
		StartCoroutine (initStartAnimations()); 
	}
	
	// Update is called once per frame
	void Update () {
		
		putOutFire();
		StartAnimation ();
	}

	void putOutFire(){
		if(isPutOut){
			rateOverTimeOff -= 1f;
			emission.rateOverTime = rateOverTimeOff;	
			if (emission.rateOverTime.constant == 0f) {
				anim.Play ("idle");	
				water.Stop ();
				Congratulations.gameObject.SetActive (true);
			}
		}
	}

	IEnumerator initPutOutFire(){
		yield return new WaitForSeconds (30);
		isPutOut = true;
	}

	IEnumerator initStartAnimations(){
		yield return new WaitForSeconds (20);
		isSartAnim = true;
	}


	void StartAnimation(){
		if(isSartAnim){
			if(!isTurned){
				scrollView.SetActive (false);
				anim.Play ("turn_left");	
				water.Play ();
				isTurned = true;
			}
			if (this.transform.eulerAngles.y < 250) {
				Debug.Log (this.transform.eulerAngles.y);
				this.transform.rotation *= Quaternion.Euler (0, 1f, 0);
			} else {
				Debug.Log ("Stop Rotation");
			}	
		}
	}
}
