using UnityEngine;
using System.Collections;

public class TemplateDecalScript : MonoBehaviour {

    private static int numObject = 0;
    public GameObject parentWall;

    public float maxScale = 0.5f;
    public float minScale = 0.1f;

    public float scaleSpeed = 2;
 
    private float maxDistance = 15;
    private bool attachedToMouse;
    private Camera cam;
    private UI ui_script;
    private int objectID;


    private float distanceFromWall = 0.0001f;
    private float aspectRatio;
    private bool rotateMode = false;

    //Called from "ObjectManipulation.cs"
    public void init(GameObject wall, float aspectRatio, UI ui_script)
    {
        parentWall = wall;
        attachedToMouse = true;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        this.aspectRatio = aspectRatio;
        objectID = numObject++;
        this.ui_script = ui_script;
    }
    
	// Update is called once per frame
	void Update () {

        if (attachedToMouse)
        {
            //Check if if Wall were hit
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                //Return if not the ParelWall was hit
                if (hit.transform.gameObject != parentWall)
                    return;

                //Rotate Object
                if (rotateMode)
                {
                    Vector3 objectOriginToMousePos = (hit.point - transform.position).normalized;
                    Vector3 wallright = parentWall.transform.right;
                    float angle = Vector3.Angle(wallright, objectOriginToMousePos);
                    if (objectOriginToMousePos.y < 0)
                        angle = 360 - angle;

                    Vector3 rotationAxis = parentWall.transform.forward;

                    transform.rotation = Quaternion.AngleAxis(angle, rotationAxis);
                    transform.Rotate(parentWall.transform.right, 90);
                   

                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        rotateMode = false;
                    }
                }
                else //Move Object
                {
                    Vector3 diffVec = (ray.origin - hit.point).normalized;
                    Vector3 scaleVec = new Vector3(distanceFromWall + objectID * distanceFromWall, distanceFromWall + objectID * distanceFromWall, distanceFromWall + objectID * distanceFromWall);
                    diffVec.Scale(scaleVec);
                    transform.position = hit.point + diffVec;       //Set the position

                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        rotateMode = true;
                    }
                }              
            }

            //Scale Objects with Mouse-Wheel
            float axisMouseWheel = Input.GetAxis("Mouse ScrollWheel");           
            if (axisMouseWheel != 0)
            {
                float scaleAmt = axisMouseWheel * scaleSpeed;

                Vector3 newScale = transform.localScale;
                newScale.Scale(new Vector3(1 - scaleAmt, 1 - scaleAmt, 1 - scaleAmt));

                newScale.x = Mathf.Clamp((float)newScale.x, minScale * aspectRatio, maxScale * aspectRatio);
                newScale.y = Mathf.Clamp((float)newScale.y, minScale, maxScale);
                newScale.z = Mathf.Clamp((float)newScale.z, minScale, maxScale);

                transform.localScale = newScale;
            }

            //Leave Object on its current position if "left mouse button" was clicked
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                attachedToMouse = false;
                ui_script.enableScrollView();
                ui_script.setPapierkorbButtonActive(false);
                ui_script.setUndoButtonActive(true);
            }            
        }
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
        ui_script.enableScrollView();
        parentWall.GetComponent<Wall>().removeObject(this.gameObject);
    }
}
