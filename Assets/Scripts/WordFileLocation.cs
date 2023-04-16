using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WordFileLocation : MonoBehaviour
{
    public Button folderLocation;
    public Button CreateBtn;
    public Text wordName;
    public Text wordSeed;
    public Text msg;
    public Text msg2;
    public GameObject firstChoise;
    public GameObject secondChoise;

    string filePath;

    public void Start()
    {
        try
        {
            SceneManager.UnloadSceneAsync("SampleScene");
        }
        catch(ArgumentException)
        {

        }
    }

    public void OnFileLocationBtnClicked()
    {
        string directory = EditorUtility.OpenFolderPanel("Select Word Save Location", "", "");
        if (directory != null)
        {
            folderLocation.GetComponentInChildren<Text>().text = directory;
            folderLocation.GetComponentInChildren<Text>().color = Color.green;

            // Check if folderLocation is viable
            CreateBtn.interactable = true;
        }
    }

    public void OnWorldLoadBtnClicked()
    {
        string worldFilePath = EditorUtility.OpenFilePanel("Select Word file", "", "");
        if(worldFilePath != null && worldFilePath.Length > 0)
        {
            PlayerPrefs.SetString("worldFilePath", worldFilePath);
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void OnCreateClicked()
    {
        string wordNameS = wordName.text;
   
        // Check if user typed valid word name
        if (wordNameS != null && wordNameS.Length > 0 )
        {
            try
            {
                filePath = Path.Combine(folderLocation.GetComponentInChildren<Text>().text, $"{wordNameS}.json");

                // Create world file
                if (File.Exists(filePath))
                {
                    msg.color = Color.red;
                    msg.text = "World with that name already exists!";
                }
                else
                {
                    File.Create(filePath);

                    // Generate and save wrold to this file
                    int seed = 0;
                    int.TryParse(wordSeed.text,out seed);

                    WorldManager worldManager = new WorldManager();
                    worldManager.CreateWorld(filePath,seed);

                    // Active first menu
                    firstChoise.SetActive(true);
                    secondChoise.SetActive(false);
                    msg2.color = Color.green;
                    msg2.text = "Word created!";
                    Task msg2Delay = new Task(async delegate {
                        await Task.Delay(2000);
                        msg2.color = Color.green;
                        msg2.text = "";
                        return;
                    });
                    msg2Delay.RunSynchronously();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                msg.color = Color.red;
                msg.text = "Invalid file path!";

            }    
        }
        else
        {
            msg.color = Color.red;
            msg.text = "Word name too short!";
        }
  
    }
}

