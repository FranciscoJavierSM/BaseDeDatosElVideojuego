using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public int municion = 50;
	
	Camera camera;

	// Start is called before the first frame update
	void Start()
    {
		camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
	{
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");

		transform.Translate(new Vector3(inputX * Time.deltaTime * 10, inputY * Time.deltaTime * 10, 0));

		if(Input.GetButtonDown("Fire1"))
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, camera.ScreenToWorldPoint(Input.mousePosition) - transform.position);
			if(hit.collider)
			{
				hit.collider.gameObject.GetComponent<EnemyController>().vida--;
			}
			municion--;
		}
	}
}
