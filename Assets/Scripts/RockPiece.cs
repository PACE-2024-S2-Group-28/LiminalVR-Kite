using UnityEngine;

public class RockPiece : MonoBehaviour
{

    [SerializeField]
    private float destroyDelay = 0f;

    private static int? bulletLayer = null;

    private void Start()
    {
        if(bulletLayer==null) bulletLayer = LayerMask.NameToLayer("Bullet");
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == bulletLayer.Value) {
    //        DestroyRockPiece();
    //    }
    //}

    public void DestroyRockPiece()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        Destroy(gameObject, destroyDelay);
    }
}
