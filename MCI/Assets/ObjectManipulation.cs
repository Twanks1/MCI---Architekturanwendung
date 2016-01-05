using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class ObjectManipulation : MonoBehaviour {
        
    private Material materialWhenSelected;
    private GameObject selectedObj;

    private Stack changes;                          //Allows Undo
    private bool changedColorOrMat = false;
    
    void Start()
    {
        changes = new Stack();                      //Stack for Undo steps    
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
        changes.Push(new Change(selectedObj.GetComponent<MeshRenderer>().material));
        selectedObj.GetComponent<MeshRenderer>().material = mat;
    }

    //add the given object to the wall
    public void addObject(GameObject obj)
    {

    }

    //Reset object to state where it was when clicked on it
    public void reset()
    {
        changedColorOrMat = false;
        selectedObj.GetComponent<MeshRenderer>().material = materialWhenSelected;
    }

    //Undo the last change
    public void undo()
    {
        if (changes.Count == 0)
            return;                 //Stack is Empty
        Change lastChange = (Change) changes.Pop();        
        selectedObj.GetComponent<MeshRenderer>().material = lastChange.material;

        if (selectedObj.GetComponent<MeshRenderer>().material == materialWhenSelected)
            changedColorOrMat = false;
    }



    //Represents one change 
    private class Change
    {
        public Material material;

        public Change(Material mat)
        {
            material = mat;
        }
    }
}
