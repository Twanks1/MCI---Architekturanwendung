using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;

public class ObjectManipulation : MonoBehaviour {

    public UI ui_script;

    public GameObject templateDecalPlane;
    public float defaultObjectScale = 0.1f;
    public Material defaultMaterial;
        
    private Material materialWhenSelected;
    private GameObject selectedObj;
    private List<GameObject> objects;

    private Stack changes;                          //Allows Undo
    private bool changedColorOrMat = false;
    
    void Start()
    {
        changes = new Stack();                      //Stack for Undo steps  
        objects = new List<GameObject>();  
    }

    //Will be called if clicked on a valid object
    public void init(GameObject selectedObject)
    {
        selectedObj = selectedObject;
        Material mat = selectedObj.GetComponent<MeshRenderer>().material;
        materialWhenSelected = mat;

        mat.color = Color.yellow;               //"Highlight the selected Object"
        changes.Clear();                        //Empty the Stack
        changedColorOrMat = false;        
    }

    //Will be called if "Tab" pressed or "Accept" button clicked
    public void shutdown()
    {
        if (!changedColorOrMat)
            selectedObj.GetComponent<MeshRenderer>().material.color = Color.white;   //Reset the "Highlight" Wall-Color
    }
    
    //Change Material from the selected object
    public void changeMaterial(Material mat)
    {
        if (selectedObj.GetComponent<MeshRenderer>().sharedMaterial == mat)
            return;     //User clicked on the same Button twice
        changedColorOrMat = true;
        changes.Push(new Change(selectedObj.GetComponent<MeshRenderer>().material, selectedObj));

        selectedObj.GetComponent<MeshRenderer>().material = mat;

        ui_script.setUndoButtonActive(true);
    }

    //add the given object to the wall
    public void addObject(Material mat)
    {
        //Set Material
        templateDecalPlane.GetComponent<MeshRenderer>().material = mat;
        float aspectRatio = (float) mat.mainTexture.width / (float) mat.mainTexture.height;
        templateDecalPlane.transform.localScale = new Vector3(aspectRatio * defaultObjectScale, 1 * defaultObjectScale, 1 * defaultObjectScale);

        //Set Rotation
        Vector3 rot = selectedObj.transform.rotation.eulerAngles;
        rot.x += 90;

        //Instantiate Prefab
        GameObject instance = (GameObject)Instantiate(templateDecalPlane, transform.position, Quaternion.Euler(rot));
        instance.GetComponent<TemplateDecalScript>().init(selectedObj, aspectRatio, ui_script);

        changes.Push(new Change(instance, selectedObj));
        objects.Add(instance);

        ui_script.disableScrollView();
        ui_script.setUndoButtonActive(true);
    }

    //Remove everything from wall
    public void reset()
    {
        changedColorOrMat = false;
        selectedObj.GetComponent<MeshRenderer>().material = defaultMaterial;
        selectedObj.GetComponent<MeshRenderer>().material.color = Color.yellow;
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if(objects[i].GetComponent<TemplateDecalScript>().parentWall == selectedObj)
            {
                GameObject objectToDestroy = objects[i];
                objects.Remove(objectToDestroy);
                Destroy(objectToDestroy);
            }
        }
        changes.Clear();
        ui_script.setUndoButtonActive(false);
    }

    //Undo the last change
    public void undo()
    {
        if (changes.Count == 0)
            return;                 //Stack is Empty
        Change lastChange = (Change) changes.Pop();
        if (lastChange.gameObject != null)
            objects.Remove(lastChange.gameObject);
        lastChange.undo();

        if (changes.Count == 0)
            ui_script.setUndoButtonActive(false);

        if (selectedObj.GetComponent<MeshRenderer>().material == materialWhenSelected)
            changedColorOrMat = false;
    }


    //Represents one change 
    private class Change
    {
        public Material material;       //The material
        public GameObject gameObject;   //The object attached to the wall 
        public GameObject selectedObj;  //the selected object (e.g. wall)

        public Change(Material mat, GameObject selectedObject)
        {
            material = mat;
            selectedObj = selectedObject;
        }

        public Change(GameObject go, GameObject selectedObject)
        {
            gameObject = go;
            selectedObj = selectedObject;
        }

        public void undo()
        {
            if (gameObject == null)
            {
                selectedObj.GetComponent<MeshRenderer>().material = material;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    /*
    private class Wall
    {
        public Material material;
        public List<GameObject> objects;

        public Wall(Material mat)
        {
            material = mat;
            objects = new List<GameObject>();
        }

        public void addObject(GameObject obj)
        {
            objects.Add(obj);
        }
    }*/
}
