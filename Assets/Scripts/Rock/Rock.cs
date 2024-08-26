using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private RockDestroyer manager;

    [SerializeField]
    private Hitpoints hitpoints;
    
    void Start() {

    }

    void Update() {

    }

    public void RayCastMadeContact () {
        hitpoints.Damage();
    }

    void OnCollisionEnter (Collision other) {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") {
        ReplaceRock();   
        }
    }

    public void ReplaceRock () {
        this.gameObject.SetActive(false);
        manager.ChangeRock();
        /*foreach(Rigidbody body in rigidbodies) {
            StartCoroutine(FadeOutRenderers(rigidbodies));
        } */
    }
        /* foreach(Transform child in fractObj.transform) {
            Color rend = child.transform.GetComponent<Renderer>().material.color;
            while (fadeOut == false) {
                float fadeAmount = rend.a - (fadeSpeed * Time.deltaTime);
                Color rockColour = new Color(rend.r, rend.g, rend.b, fadeAmount);
                rend = rockColour;
                if (fadeAmount <= 0) {
                    fadeOut = true;
                }
                Debug.Log(fadeAmount);
            }
        } */
    

    /*private IEnumerator FadeOutRenderers(Rigidbody[] rigids) {
        float time = 0;
        Renderer[] renderers = new Renderer[rigids.Length];
        for (int i = 0; i < rigids.Length; i++) {
            renderers[i] = rigids[i].GetComponent<Renderer>();
        }

        foreach(Rigidbody thisBody in rigids) {
            Destroy(thisBody.GetComponent<Collider>());
            Destroy(thisBody);
        }

        while (time < 2) {
            float step = Time.deltaTime * fadeSpeed;
            foreach(Renderer rend in renderers) {
                Color rendColour = rend.material.color;
                float fadeAmount = rendColour.a - (fadeSpeed * Time.deltaTime);
                rendColour.a -= fadeAmount;
                rend.material.color = rendColour;
            }
            
            time += step;
            Debug.Log(renderers[0].material.color.a);
            yield return null;
        }

        foreach(Renderer renderer in renderers) {
            Destroy(renderer.gameObject);
        }

        Destroy(gameObject);
    } */

    /*private Renderer GetRendererFromRigidbody(Rigidbody rigidbody) {
        return Rigidbody.GetComponent<Renderer>();
    }*/
}
