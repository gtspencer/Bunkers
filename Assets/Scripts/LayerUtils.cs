using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerUtils : MonoBehaviour
{
    public static Dictionary<string, int> GetLayers()
    {
        Dictionary<string, int> layers = new Dictionary<string, int>();
        for (int layer = 0; layer < 32; layer++)
        {
            string layerName = LayerMask.LayerToName(layer);
            if (layerName.Length > 0)
            {
                layers.Add(layerName, layer);
            }
        }
        return layers;
    }
}
