//using System.Collections;
//using System.Collections.Generic;
////using Unity.VisualScripting;
//using UnityEngine;

//public class Rock : MonoBehaviour
//{
//    [SerializeField]
//    private RockDestroyer manager;

    
//    [SerializeField]
//    private Hitpoints hitpoints;

//    [SerializeField]
//    private float currentSpeed;

//    [SerializeField]private Rigidbody rb;

//    public void RayCastMadeContact () {
//        hitpoints.Damage();
//    }


//    void Start()
//    {
//    AsteroidSpawner spawner = FindObjectOfType<AsteroidSpawner>();
//        if (spawner != null)
//        {
//            currentSpeed = Random.Range(spawner.minSpeed, spawner.maxSpeed);
//        } //set up a random speed between minSpeed and maxSpeed
//    }

//    void Update()
//    {
//        AsteroidSpawner spawner = FindObjectOfType<AsteroidSpawner>();
//        if (spawner != null)
//        {
//            if (currentSpeed < spawner.maxSpeed)
//            {
//                currentSpeed += spawner.acceleration * Time.deltaTime; //increase the speed if below maxSpeed
//                currentSpeed = Mathf.Min(currentSpeed, spawner.maxSpeed); //clamp to the maxSpeed
//            }

//        rb = GetComponent<Rigidbody>();
//        if (rb != null)
//        {
//            Vector3 direction = rb.velocity.normalized;
//            rb.velocity = direction * currentSpeed;
//        }
//        Debug.Log($"Asteroid current speed: {currentSpeed}");
//        }
//    }

//    void ReduceSpeed()
//    {   
//    AsteroidSpawner spawner = FindObjectOfType<AsteroidSpawner>();
//        if (spawner != null) {
//            currentSpeed = Mathf.Max(currentSpeed - spawner.slowAmount, spawner.minSpeed); //decrease the speed by slowAmount but not below minSpeed
//            if (rb != null) {
//                Vector3 oppositeForce = -rb.velocity.normalized * spawner.slowAmount;
//                rb.AddForce(oppositeForce, ForceMode.VelocityChange);
//            }
//            Debug.Log($"Asteroid current speed after reduction: {currentSpeed}");
            
//        }
//    }

//    void OnEnable()
//    {
//        ShipCollision spaceship = FindObjectOfType<ShipCollision>();  //enable to the event on the spaceship
//        spaceship?.OnShipHitByAsteroid.AddListener(ReduceSpeed);
//    }

//    void OnDisable()
//    {
//        ShipCollision spaceship = FindObjectOfType<ShipCollision>();
//        spaceship?.OnShipHitByAsteroid.RemoveListener(ReduceSpeed);
//    }


//    void OnCollisionEnter (Collision other) 
//    {
//        Debug.Log($"Collision detected with: {other.gameObject.name}");
//        if (other.gameObject.tag == "Bullet"){
//            hitpoints.Damage();
//        }
//        else if (other.gameObject.tag =="Player") {    
//        //onDeath();
//            Debug.Log("Collision with Player detected");
//            hitpoints.Damage();
//            ReplaceRock();
//        }
//        else if (other.gameObject.tag =="Enemy") {
//            ReplaceRock();
//        }
//    }

//    void ReplaceRock()
//    {
//        this.gameObject.SetActive(false);
//        manager.ChangeRock();
//    }
//}

