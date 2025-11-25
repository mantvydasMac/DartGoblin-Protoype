using UnityEngine;
using UnityEngine.InputSystem;

public class DeathPlane : MonoBehaviour
{

    public Stage stage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            stage.OnDeath();
        }
    }
}