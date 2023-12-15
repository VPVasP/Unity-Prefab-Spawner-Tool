using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class PrefabSpawner : EditorWindow
{
    //These are the variables we want the user to modify for the spawn Object.
    GameObject prefab;
    Color color;
    Vector3 scale;
    string objectName;
    string folderName;
    new Vector3 position;
    bool addRigidbody;
    bool addCharController;
    int colliderIndex;
    string[] colliderOptions = new string[] { "None", "Box Collider", "Sphere Collider", "Capsule Collider", "Mesh Collider" };//an array of the colliders we want to add
    string scriptName;
    string tagName;
    [MenuItem("Window/Prefab Spawner")]

    //We show the window for the Prefab spawner in the editor.
    public static void ShowWindow()
    {
        GetWindow<PrefabSpawner>("Prefab Spawner");
    }
    //OnGui for the prefab spawner window.
    void OnGUI()
    {

        GUILayout.Label("Prefab Spawner", EditorStyles.boldLabel);
        //We let the user choose the prefab he wants to spawn.
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        //We let the user to change the color.
        color = EditorGUILayout.ColorField("Color", color);
        //We let the user to change the scale.
        scale = EditorGUILayout.Vector3Field("Scale", scale);
        //We let the user to change the object name.
        objectName = EditorGUILayout.TextField("Object Name", objectName);
        //We let the user to put their folder name.
        folderName = EditorGUILayout.TextField("Folder Name", folderName);
        //We let the user to choose the position of the object to spawn.
        position = EditorGUILayout.Vector3Field("Position", position);
        //We let the user to choose if he wants rigibody or not by choosing the toggle.
        addRigidbody = EditorGUILayout.Toggle("Add Rigidbody", addRigidbody);
        //We let the user to choose if he wants the Character Controller or not by choosing the toggle.
        addCharController = EditorGUILayout.Toggle("Add CharacterController", addCharController);
        //We let the user to choose the Collider type he wants.
        colliderIndex = EditorGUILayout.Popup("Collider Type", colliderIndex, colliderOptions);
        //We let the user to choose the script he wants to add.
        scriptName = EditorGUILayout.TextField("Script Name", scriptName);
        //We let the user to choose the tag he wants.
        tagName = EditorGUILayout.TagField("Tag", tagName);

        if (GUILayout.Button("Instantiate and Save Prefab"))
        {
            SavedData savedData = new SavedData();
            if (prefab != null && !string.IsNullOrEmpty(folderName) && !string.IsNullOrEmpty(objectName))
            {
                //We instatiate the object.
                GameObject instance = Instantiate(prefab, position, Quaternion.identity); 
                savedData.gameobject = instance;
                //We set the position and scale of the object.
                instance.transform.localScale = scale;
                savedData.position = position;
                savedData.scale = instance.transform.localScale;
                //We change the color of the spawned Object.
                Renderer renderer = instance.GetComponent<Renderer>();
                Material material = new Material(renderer.sharedMaterial);
                material.color = color;
                savedData.color = material.color;
                renderer.material = material;
                //We set the name and tag for our Object.
                instance.name = objectName;
                savedData.objectName = instance.name;
                instance.tag = tagName;
                savedData.tagName = instance.tag;
                if (addRigidbody) 
                {
                    instance.AddComponent<Rigidbody>();//We add the Rigibody to our instance Game Object.
                    savedData.addRigidbody = addRigidbody; //We save the Rigibody Values to our SavedData.
                }
                if (addCharController)
                {
                    instance.AddComponent<CharacterController>();//We add A Character Controller to our instance Game Object.
                    savedData.addCharController = addCharController;//We save the Character Controller Values to our SavedData.
                }
         
                if (colliderIndex == 1) //if the collider index is 1
                {
                    if (!(instance.GetComponent<BoxCollider>()))
                    {
                        instance.AddComponent<BoxCollider>(); //We add a collider to our instance GameObject.
                        savedData.colliderIndex = colliderIndex;
                    }//if we do not have a boxcollider.
                       ; //We save the collider index to the colliderIndex Data.
                }
                
                else if (colliderIndex == 2)
                {
                    if (!(instance.GetComponent<SphereCollider>()))
                    {
                        instance.AddComponent<SphereCollider>();
                        savedData.colliderIndex = colliderIndex;
                    }

                }
                else if (colliderIndex == 3)
                {
                   
                        instance.AddComponent<CapsuleCollider>();
                    savedData.colliderIndex = colliderIndex;
                }
                else if (colliderIndex == 4)
                {
                   
                        instance.AddComponent<MeshCollider>();
                    savedData.colliderIndex = colliderIndex;
                }

            
                if (!string.IsNullOrEmpty(scriptName)) 
                {
                    System.Type scriptType = System.Type.GetType(scriptName); 
                    if (scriptType != null)
                    {
                        instance.AddComponent(scriptType); //We add to our instance object the script type.
                        savedData.scriptName = scriptName; //We save the script name to our savedData.
                    }
                    else
                    {
                        Debug.Log("Script not found: " + scriptName);
                    }
                }
                
                if (!AssetDatabase.IsValidFolder("Assets/" + folderName)) //We check if it does not exist.
                {
                    AssetDatabase.CreateFolder("Assets", folderName); //We creat the folder with the folder name we choose.
                }
                savedData.folderName = folderName; //We put the folder name to our savedData.
       
                string materialPath = "Assets/" + folderName + "/" + objectName + ".mat"; //We save the material as .mat asset.
                AssetDatabase.CreateAsset(material, materialPath); //We make our material at the path we choose.
            
                string prefabPath = "Assets/" + folderName + "/" + objectName + ".prefab"; //We save the prefab as a .prefab asset.
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);//We save the prefab where we chose the location to be saved.
         
                string json = JsonUtility.ToJson(savedData); //We make the savedData to Json.
                string filePath = "Assets/savedata.json"; //We save it as SavedData.json.
                File.WriteAllText(filePath, json); //We write it on the string filepath and string json we gave above our savedData.

                AssetDatabase.Refresh(); //We refresh the assetDatabase.
         
            }

        }
        if (GUILayout.Button("Load Values"))
        {
         
            string filePath = "Assets/savedata.json";
          
           
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath); //We read the values from the filePath.
              
                SavedData savedData = JsonUtility.FromJson<SavedData>(json); //We get the values from json.
                
                Debug.Log("Loaded values: " + savedData.objectName);

                //We load the values here.
                string prefabPath = "Assets/" + savedData.folderName + "/" + savedData.objectName + ".prefab";
                //We load the asset at path which is our prefab path even if we close Unity.
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                //We take the values we want to save and add it as the savedata values for example our color = the save.data.color etc.
                color = savedData.color;
                scale = savedData.scale;
                objectName = savedData.objectName;
                folderName = savedData.folderName;
                position = savedData.position;
                addRigidbody = savedData.addRigidbody;
                addCharController = savedData.addCharController;
                colliderIndex = savedData.colliderIndex;
                scriptName = savedData.scriptName;
                tagName = savedData.tagName;
            }
        }
    }
}
          
