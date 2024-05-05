using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideOnPlay : MonoBehaviour
{
    [SerializeField] private TilemapRenderer tilemapRenderer;
    [SerializeField] private SpriteRenderer sprite;
    void Start()
    {
        if (tilemapRenderer != null) 
        {
            tilemapRenderer.enabled = false;
        }
        if(sprite != null)
        {
            sprite.enabled = false;
        }      
    }
}
