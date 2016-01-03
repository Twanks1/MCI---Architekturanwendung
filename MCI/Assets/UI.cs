using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

    public Canvas canvas;
    public Material[] materials;

    private Color wallColorWhenSelected;
    private Material wallMaterialWhenSelected;
    private GameObject selectedWall;

    private bool changedColorOrMat = false;
    
    void Start()
    {
        canvas.enabled = false;
    }

    //Will be called if clicked on a valid Wall
    public void enable(GameObject selectedObject)
    {
        selectedWall = selectedObject;
        Material mat = selectedWall.GetComponent<MeshRenderer>().material;
        wallMaterialWhenSelected = mat;
        wallColorWhenSelected = mat.color;

        //"Highlight the selected Object"
        mat.color = Color.yellow;

        changedColorOrMat = false;
        canvas.enabled = true;
    }

    //Will be called if "Tab" pressed
    public void disable()
    {
        if (!changedColorOrMat)
        {
            Material mat = selectedWall.GetComponent<MeshRenderer>().material;
            //Reset the "Highlight" Wall-Color
            mat.color = wallColorWhenSelected;
        }

        canvas.enabled = false;
    }

    public void changeMaterial(int index)
    {
        changedColorOrMat = true;
        selectedWall.GetComponent<MeshRenderer>().material = materials[index];
    }

    public void changeColor(Color col)
    {
        changedColorOrMat = true;
        selectedWall.GetComponent<MeshRenderer>().material.color = col;
    }

    public void resetWall()
    {
        changedColorOrMat = false;
        selectedWall.GetComponent<MeshRenderer>().material = wallMaterialWhenSelected;
    }
}
