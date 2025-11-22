using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class Focusable : MonoBehaviour
{
    [SerializeField] [CanBeNull] private Material focusedMaterial;
    [SerializeField] [CanBeNull] private Material originalMaterial;
    [SerializeField] [CanBeNull] private GameObject focusLight;
    [SerializeField] [CanBeNull] private AudioClip focusSfx;
    
    public bool IsFocused { get; private set; }

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer.material = originalMaterial;
        focusLight?.SetActive(false);
    }

    public void Focus()
    {
        if (IsFocused) return;
        IsFocused = true;
        spriteRenderer.material = focusedMaterial;
        focusLight?.SetActive(true);
        audioSource.PlayOneShot(focusSfx);
    }

    public void Unfocus()
    {
        if (!IsFocused) return;
        IsFocused = false;
        spriteRenderer.material = originalMaterial;
        focusLight?.SetActive(false);
    }
}
