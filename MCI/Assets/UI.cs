using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class UI : MonoBehaviour {

    public Canvas canvas;
    public Material[] materials;
    
    private Material wallMaterialWhenSelected;
    private GameObject selectedWall;

    private Stack changes;                          //Allows Undo
    private bool changedColorOrMat = false;
    
    void Start()
    {
        canvas.enabled = false;                     //Disable UI at start
        changes = new Stack();                      //Stack for Undo steps
        disableCursor();                            //Disable Mouse-Cursor
    }

    //Will be called if clicked on a valid Wall
    public void enable(GameObject selectedObject)
    {
        selectedWall = selectedObject;
        Material mat = selectedWall.GetComponent<MeshRenderer>().material;
        wallMaterialWhenSelected = mat;

        mat.color = Color.yellow;               //"Highlight the selected Object"

        changes.Clear();                        //Empty the Stack

        changedColorOrMat = false;
        canvas.enabled = true;

        enableCursor();
    }

    //Will be called if "Tab" pressed
    public void disable()
    {
        if (!changedColorOrMat)
            selectedWall.GetComponent<MeshRenderer>().material.color = Color.white;   //Reset the "Highlight" Wall-Color

        canvas.enabled = false;                 //Disable Canvas-Object
        disableCursor();                        //Disable Mouse-Cursor
    }


    //Change Material with an given Index
    //To add a new Material, create it and add it in the Editor of the "First Person Controller" in the materials-Array from the UI-Script
    public void changeMaterial(int index)
    {
        if (selectedWall.GetComponent<MeshRenderer>().sharedMaterial == materials[index])
            return;     //User clicked on the same Button twice
        changedColorOrMat = true;
        changes.Push(new Change(selectedWall.GetComponent<MeshRenderer>().material));
        selectedWall.GetComponent<MeshRenderer>().material = materials[index];
    }

    //Reset Wall to state where it was when clicked on it
    public void resetWall()
    {
        changedColorOrMat = false;
        selectedWall.GetComponent<MeshRenderer>().material = wallMaterialWhenSelected;
    }

    //Undo the last change
    public void undo()
    {
        if (changes.Count == 0)
            return;                 //Stack is Empty
        Change lastChange = (Change) changes.Pop();        
        selectedWall.GetComponent<MeshRenderer>().material = lastChange.material;

        if (selectedWall.GetComponent<MeshRenderer>().material == wallMaterialWhenSelected)
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
