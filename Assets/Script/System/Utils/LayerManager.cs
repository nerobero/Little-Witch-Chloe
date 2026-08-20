using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoSingletonBase<LayerManager>
{
    private Dictionary<string, int> foregroundLayers = new Dictionary<string, int>();
    private Dictionary<string, int> backgroundLayers = new Dictionary<string, int>();

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();

        InitializeLayers();
    }
   
    private void InitializeLayers()
    {
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            
            if (!string.IsNullOrEmpty(layerName) && layerName.Contains("_"))
            {
                string[] split = layerName.Split('_');
                string prefix = split[0]; 
                string type = split[1];   

                if (prefix == "Foreground") foregroundLayers[type] = i;
                else if (prefix == "Background") backgroundLayers[type] = i;
            }
        }
    }

    public int GetLayer(bool isBackground, string typeName)
    {
        var targetDict = isBackground ? backgroundLayers : foregroundLayers;

        if (targetDict.TryGetValue(typeName, out int layerIndex))
        {
            return layerIndex;
        }

        Debug.LogWarning($"[LayerManager] {(isBackground ? "Background" : "Foreground")}_{typeName} no Layer!");
        return 0;
    }

    // Compares the Background_/Foreground_ prefix of each object's current layer,
    // so attack logic can gate damage without caring about the specific type suffix.
    public static bool IsSameSide(GameObject a, GameObject b)
    {
        string layerNameA = LayerMask.LayerToName(a.layer);
        string layerNameB = LayerMask.LayerToName(b.layer);

        if (string.IsNullOrEmpty(layerNameA) || string.IsNullOrEmpty(layerNameB)) return true;

        string prefixA = layerNameA.Split('_')[0];
        string prefixB = layerNameB.Split('_')[0];

        return prefixA == prefixB;
    }
}
