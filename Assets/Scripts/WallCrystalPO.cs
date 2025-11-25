using UnityEngine;
using System.Collections;

public class WallCrystalPO : MonoBehaviour, IKickable, ISwappable
{
    
    private float explosionRadius = 2f;
    private float explosionLaunchSpeed = 20f;
    private float explosionDelay = 0.15f;

    public AudioClip explosionSound;
    public ParticleSystem explosionParticles;

    [SerializeField] public Focusable focusable;

    public Focusable Focusable => focusable;

    void Start()
    {
        hitstopDuration = explosionDelay;
        var main = explosionParticles.main;
        main.startDelay = explosionDelay;
    }

    // kickable
    public float hitstopDuration { get; set;}

    public void kick(Vector2 vel) 
    {
        explode();
    }

    public void swap(Vector3 pos)
    {
        transform.position = pos;
        explode();
    }


    void explode()
    {
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        PlayAtPoint(explosionSound, transform.position, 0.55f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Player","Object", "TransparentGround"));

        foreach(Collider2D col in colliders)
        {
            var breakable = col.gameObject.GetComponent<IBreakable>();
            if(breakable == null)
            {
                Vector2 direction = new Vector2(col.gameObject.transform.position.x - transform.position.x, col.gameObject.transform.position.y - transform.position.y);
                direction.Normalize();

                col.attachedRigidbody.linearVelocity += direction * explosionLaunchSpeed;
            }
            else 
            {
                breakable.Break();
            }
            
        }

        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public static void PlayAtPoint(AudioClip clip, Vector3 pos, float volume = 1.0f)
    {
        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = pos;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;

        src.spatialBlend = 1f;      // 3D sound
        src.minDistance = 0.2f;     // MUCH louder up close
        src.maxDistance = 30f;
        src.rolloffMode = AudioRolloffMode.Linear;

        src.Play();
        GameObject.Destroy(go, clip.length / src.pitch);
    }
}