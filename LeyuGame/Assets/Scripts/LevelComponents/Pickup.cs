using UnityEngine;

public class Pickup : MonoBehaviour
{
	Transform cameraTrans;
	public GameObject feedbackPrefab;

	void Awake ()
	{
		cameraTrans = GameObject.Find("Main Camera").transform;
	}

	void Update ()
	{
		transform.LookAt(cameraTrans.position);
	}

	private void OnTriggerStay (Collider other)
	{
		if (other.tag == "Player") {
			GameObject feedbackGO = Instantiate(feedbackPrefab, transform.position, Quaternion.identity);
			Destroy(feedbackGO, 1);
			Destroy(gameObject);
		}
	}
}