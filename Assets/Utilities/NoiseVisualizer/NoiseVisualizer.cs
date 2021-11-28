using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NoiseVisualizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2Int dimsInChunks;
    private Noise noise = new BiomeNoise();
    private Texture2D texture;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private bool useBounds;
    
    public void DisplayNoise()
    {
        int width = dimsInChunks.x * Chunk.Dims.x;
        int height = dimsInChunks.y * Chunk.Dims.z;
        texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = noise.GetNoise(new Vector2(x, y));
                Color color = new Color(noiseValue, noiseValue, noiseValue);
                if (!useBounds) texture.SetPixel(x, y, color);
                else
                {
                    if (noiseValue >= bounds.x && noiseValue <= bounds.y) texture.SetPixel(x, y, Color.white);
                    else texture.SetPixel(x, y, Color.black);
                }
            }
        }
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture,
            new Rect(Vector2.zero,
                new Vector2(width, height)),
            new Vector2(0.5f, 0.5f));
    }
    
    
    
    
    
}
