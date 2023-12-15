using UnityEngine;
//We make it Serializable so the values can be seen.
[System.Serializable]
//This is the class that we want to save our data to.
public class SavedData 
{
    public GameObject gameobject;
    public Color color;
    public Vector3 scale;
    public string objectName;
    public string folderName;
    public Vector3 position;
    public bool addRigidbody;
    public bool addCharController;
    public int colliderIndex;
    public string scriptName;
    public string tagName;

}
