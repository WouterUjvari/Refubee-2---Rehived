using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideOnPlay : MonoBehaviour
{
    [SerializeField] private TilemapRenderer tilemapRenderer;
    void Start()
    {
        tilemapRenderer.enabled = false;
    }
}
