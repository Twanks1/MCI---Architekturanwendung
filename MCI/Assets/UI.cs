using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class UI : MonoBehaviour {

    public  EnableSpecificUI ui_script;
    
    private Material materialWhenSelected;
    private GameObject selectedObj;

    private Stack changes;                          //Allows Undo
    private bool changedColorOrMat = false;
    
    void Start()
    {
        changes = new Stack();                      //Stack for Undo steps
        disableCursor();                            //Disable Mouse-Cursor
    }

    //Will be called if clicked on a valid Wall
    public void enable(GameObject selectedObject)
    {
        selectedObj = selectedObject;
        Material mat = selectedObj.GetComponent<MeshRenderer>().material;
        materialWhenSelected = mat;

        mat.color = Color.yellow;               //"Highlight the selected Object"
        changes.Clear();                        //Empty the Stack
        changedColorOrMat = false;

        ui_script.enableUI(selectedObj.tag);

        enableCursor();
    }

    //Will be called if "Tab" pressed or "Accept" button clicked
    public void disable()
    {
        if (!changedColorOrMat)
            selectedObj.GetComponent<MeshRenderer>().material.color = Color.white;   //Reset the "Highlight" Wall-Color
        ui_script.disableUI();
        disableCursor();                        //Disable Mouse-Cursor
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




    private void disableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GetComponent<FirstPersonController>().lockRotation = false;
    }

    private void enableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GetComponent<FirstPersonController>().lockRotation = true;
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
