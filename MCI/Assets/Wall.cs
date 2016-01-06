using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
* ATTENTION: This script is used not only for walls (for floors, stairs, ceiling and pillars too)
* but only use the default-Material Implementation from this script to reset those objects
*/
public class Wall : MonoBehaviour {

    public Material defaultMaterial;

    private List<GameObject> objects;        //The objects attached to this wall
    private MeshRenderer meshRenderer;
    private State lastState;

	void Start () {
        objects = new List<GameObject>();
        meshRenderer = GetComponent<MeshRenderer>();
	}

    public void changeMaterial(Material mat)
    {
        meshRenderer.material = mat;
    }

    public void addObject(GameObject obj)
    {
        objects.Add(obj);
    }

    public void removeObject(GameObject obj)
    {       
        objects.Remove(obj);
        Destroy(obj);        
    }

    public void removeAllObjects()
    {
        foreach (GameObject go in objects)
            Destroy(go);
        objects.Clear();
    }

    public void reset()
    {
        saveState();
        removeAllObjects();
        meshRenderer.material = defaultMaterial;
        meshRenderer.material.color = Color.yellow; //"Highlight" Color
    }
    
    public bool isInDefaultState()
    {
        if (meshRenderer.sharedMaterial.mainTexture == defaultMaterial.mainTexture && objects.Count == 0)
            return true;

        return false;
    }

    public void saveState()
    {
        lastState = new State(meshRenderer.material, objects);
    }

    public void restoreLastState()
    {
        meshRenderer.material = lastState.material;
        objects = lastState.objects;
        foreach(GameObject go in objects)
            go.SetActive(true);
    }

    private class State
    {
        public Material material;
        public List<GameObject> objects;

        public State(Material mat, List<GameObject> obj)
        {
            this.material = mat;
            objects = new List<GameObject>();
            foreach(GameObject go in obj)
            {
                GameObject duplicatedObj = Instantiate(go);
                objects.Add(duplicatedObj);
                duplicatedObj.SetActive(false);
            }
        }

    }
}
