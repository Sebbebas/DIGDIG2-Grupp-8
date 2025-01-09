using System.IO;
using Unity.IO;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    public InputActionAsset actions;

    //public object FilesPath { get; private set; }

    public void OnEnable()
    {
        /*if (File.Exists(FilesPath.jsonInputAsset))
        {
            string JSONDatas;
            StreamReader file = File.OpenText(FilesPath.jsonInputAsset);
            JSONDatas = file.ReadToEnd();
            file.Close();

            Debug.Log(JSONDatas);
            InputActionRebindingExtensions.LoadBindingOverridesFromJson(inputAsset, JSONDatas);
        }
        else
        {
            Debug.LogError($"Unable to find the Config file at {FilesPath.jsonInputAsset}");
        }*/

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }

    public void OnDisable()
    {
        /*string JSONDatas = InputActionRebindingExtensions.SaveBindingOverridesAsJson(inputAsset);
        Debug.Log(JSONDatas);

        if (File.Exists(FilesPath.jsonInputAsset))
        { File.Delete(FilesPath.jsonInputAsset); }

        StreamWriter file = File.CreateText(FilesPath.jsonInputAsset);
        file.WriteLine(JSONDatas);
        file.Close();

        PlayerPrefs.SetString("controls", JSONDatas);*/

        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}
