using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Dedicated editor-only tool window for CSV to Binary conversion 
/// </summary>
public class CsvToBinaryConverterWindow : EditorWindow
{
    private Label _conversionStatus;
    [MenuItem("Team DAON Tools/Data")]
    public static void ShowMyEditor()
    {
        EditorWindow window = GetWindow<CsvToBinaryConverterWindow>();
        window.titleContent = new GUIContent("CSV Binary Converter");
    }

    public void CreateGUI()
    {
        rootVisualElement.Add(
            new Label("Press on the button to convert the CSV to Binary bytes. \n"));

        var button = new Button(OnConvertClicked)
        {
            text = "CSV => Binary Convert Start"
        };
        rootVisualElement.Add(button);

        _conversionStatus = new Label { style = { whiteSpace = WhiteSpace.Normal } };

        rootVisualElement.Add(_conversionStatus);
    }

    private void OnConvertClicked()
    {

        List<string> results = CsvBinaryImporter.ConvertAll();
        _conversionStatus.text = string.Join("\n", results);
    }

}
