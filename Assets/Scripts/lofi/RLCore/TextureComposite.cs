using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureComposite : MonoBehaviour
{
    [SerializeField] private Texture2D baseTexture;
    [SerializeField] private Texture2D roofTexture;
    [SerializeField] private Texture2D detailsTexture;
    [SerializeField] private Material compositeTexture;

    private void Awake()
    {
        Texture2D texture = new Texture2D(128, 64, TextureFormat.RGBA32, true);

        Color[] basePixels = baseTexture.GetPixels(0, 0, 128, 64);
        texture.SetPixels(0, 0, 128, 64, basePixels);

        Color[] roofPixels = roofTexture.GetPixels(0, 0, 128, 64);
        texture.SetPixels(0, 0, 128, 64, roofPixels);
        
        Color[] detailsPixels = detailsTexture.GetPixels(0, 0, 128, 64);
        texture.SetPixels(0, 0, 128, 64, detailsPixels);

        texture.Apply();

        compositeTexture.mainTexture = texture; 
    }

}
