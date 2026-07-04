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

    public int GetLayer(bool isForeground, string typeName)
    {
        var targetDict = isForeground ? foregroundLayers : backgroundLayers;

        if (targetDict.TryGetValue(typeName, out int layerIndex))
        {
            return layerIndex;
        }

        Debug.LogWarning($"[LayerManager] {(isForeground ? "Foreground" : "Background")}_{typeName} no Layer!");
        return 0; 
    }
}
