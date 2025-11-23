
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParallaxLayer
{
    public SpriteRenderer parallaxRenderer;
    public float scrollSpeed;
}

public class ParallaxScroller : MonoBehaviour
{
    [Tooltip("Actual visibility layer (Order in Layer) is defined in individual Sprite Renderers")]
    [SerializeField] private List<ParallaxLayer> parallaxLayers;
    private List<SpriteRenderer> parallaxWraps;

    [SerializeField] private Camera camera;

    private void Start()
    {
        parallaxWraps = new List<SpriteRenderer>();
        foreach (ParallaxLayer parallaxLayer in parallaxLayers)
        {
            SpriteRenderer parallaxWrap = Instantiate(parallaxLayer.parallaxRenderer, parallaxLayer.parallaxRenderer.transform.parent, true);
            parallaxWrap.transform.position = parallaxLayer.parallaxRenderer.transform.position;
            parallaxWrap.transform.localScale = parallaxLayer.parallaxRenderer.transform.localScale;
            parallaxWraps.Add(parallaxWrap);
        }
    }

    private void Update()
    {
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            SpriteRenderer parallaxWrap = parallaxWraps[i];
            SpriteRenderer parallax = parallaxLayers[i].parallaxRenderer;
            float scrollSpeed = parallaxLayers[i].scrollSpeed;
            
            float offset = camera.transform.position.x * (1 - scrollSpeed);
        
            int multiplier = Mathf.FloorToInt((camera.transform.position.x - offset) / parallax.bounds.size.x);
            Vector2 newPos = parallax.transform.position;
            newPos.x = offset + multiplier * parallax.bounds.size.x;
            parallax.transform.position = newPos;

            Vector2 newPosExt = newPos;
            newPosExt.x += parallax.bounds.size.x; 
            parallaxWrap.transform.position = newPosExt;            
        }
    }
}
