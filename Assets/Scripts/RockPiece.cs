using UnityEngine;

public class RockPiece : MonoBehaviour
{

    [SerializeField]
    private float destroyDelay = 0f;

    private int bulletLayer;

    private void Start()
    {
        bulletLayer = LayerMask.NameToLayer("Bullet");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == bulletLayer)
        {
            DestroyRockPiece();
        }
    }

    private void DestroyRockPiece()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        Destroy(gameObject, destroyDelay);
    }
}
