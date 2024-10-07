using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private RockDestroyer manager;

    
    [SerializeField]
    private Hitpoints hitpoints;

    [SerializeField]
    private float currentSpeed;

    [SerializeField]private Rigidbody rb;

    public void RayCastMadeContact () {
        hitpoints.Damage();
    }


    void Start()
    {
    AsteroidSpawner spawner = FindObjectOfType<AsteroidSpawner>();
        if (spawner != null)
        {
            currentSpeed = Random.Range(spawner.minSpeed, spawner.maxSpeed);
        } //set up a random speed between minSpeed and maxSpeed
    }

    void Update()
    {
        AsteroidSpawner spawner = FindObjectOfType<AsteroidSpawner>();
        if (spawner != null)
        {
            if (currentSpeed < spawner.maxSpeed)
            {
                currentSpeed += spawner.acceleration * Time.deltaTime; //increase the speed if below maxSpeed
                currentSpeed = Mathf.Min(currentSpeed, spawner.maxSpeed); //clamp to the maxSpeed
            }

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = rb.velocity.normalized;
            rb.velocity = direction * currentSpeed;
        }
        Debug.Log($"Asteroid current speed: {currentSpeed}");
        }
    }

    void ReduceSpeed()
    {   
    AsteroidSpawner spawner = FindObjectOfType<AsteroidSpawner>();
        if (spawner != null)
        {
            currentSpeed = Mathf.Max(currentSpeed - spawner.slowAmount, spawner.minSpeed); //decrease the speed by slowAmount but not below minSpeed
            if (rb != null)
            {
                Vector3 oppositeForce = -rb.velocity.normalized * spawner.slowAmount;
                rb.AddForce(oppositeForce, ForceMode.VelocityChange);
            }
            Debug.Log($"Asteroid current speed after reduction: {currentSpeed}");
            
        }
    }

void OnEnable()
{
   
    ShipCollision spaceship = FindObjectOfType<ShipCollision>();  //enable to the event on the spaceship
    if (spaceship != null)
    {
        spaceship.OnShipHitByAsteroid.AddListener(ReduceSpeed);
    }
}

void OnDisable()
{
    ShipCollision spaceship = FindObjectOfType<ShipCollision>();
    if (spaceship != null)
    {
        spaceship.OnShipHitByAsteroid.RemoveListener(ReduceSpeed);
    }
}


    void OnCollisionEnter (Collision other) {
        if (other.gameObject.tag == "Bullet"){
            hitpoints.Damage();
        }

       else if (other.gameObject.CompareTag("Player"))
    {    
        //onDeath();
        hitpoints.Damage();
        ReplaceRock();
    }
    else if (other.gameObject.CompareTag("Enemy"))
    {
        ReplaceRock();
    }
    }

    void ReplaceRock () {
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

