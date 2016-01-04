using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class DetectWalls : MonoBehaviour {

    public float maxDistance = 15f;
    public bool drawDebugRay = true;
    public float debugRayLifetime = 5;

    private Camera cam;

    private bool inUIState = false;

    //The UI-Script
    private UI ui;

	// Use this for initialization
	void Start () {
        cam = GetComponentInChildren<Camera>();
        ui = gameObject.GetComponent<UI>();
    }
	
	// Update is called once per frame
	void Update () {
       
        if (!inUIState)
        {
            //Left-Mouse Button Click
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (drawDebugRay)
                    Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, debugRayLifetime);

                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    float distance = (this.transform.position - hit.transform.position).magnitude;

                    Debug.Log("Hit GameObject: " + hit.collider.gameObject + " Distance: " + distance);

                    //Return if not hit a Wall
                    if (hit.transform.tag != "Wall")
                           return;

                    //Enable UI     
                    inUIState = true;
                    ui.enable(hit.collider.gameObject);
                }
            }
        }
        else
        {
            //Pressed Tab-Button
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                disableUI();
            }
        }
      
    }

    public void disableUI()
    {
        inUIState = false;
        ui.disable();
    }

}
