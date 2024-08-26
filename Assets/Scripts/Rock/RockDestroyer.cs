using DG.Tweening;
using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDestroyer : MonoBehaviour
{
    [SerializeField]
    private GameObject fracturedRock;

    private GameObject fractRock;

    [SerializeField]
    private GameObject rock;

    Vector3 scale;
    private Color alphaColor;

    private float timer = rockLifetime;
    public float fadeSpeed = 1.0f;
    private bool fadeOut = false;

    [SerializeField]
    private SoundScripObj rockBreakSFX;

    [SerializeField]
    private GameObject rockBreakParticlesObj;

    [SerializeField]
    private GameObject healthPackPrefab;

    [SerializeField]
    private float healthDropChance;

    // Start is called before the first frame update
    void Start()
    {
        fracturedRock.SetActive(true);
        //loop through rock pieces, build list of rigidbodies so i dont have to get them more than once
        //this could be serialized reference
        foreach (Transform t in fracturedRock.transform)
            rockRBs.Add(t.GetComponent<Rigidbody>());
        fracturedRock.SetActive(false);



        //scale = this.transform.localScale;
        //fractRock = Instantiate(fracturedRock, this.transform.position, this.transform.rotation);
        //fractRock.transform.parent = this.transform;
        //fractRock.transform.localScale = scale;
        //fractRock.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartCoroutine(RockFading());
                fadeOut = false;
            }
        }
    }

    public void ChangeRock()
    {
        rockBreakSFX?.Play(wPos: transform.position);

        // Health Pack dropping logic
        float randomNumber = Random.Range(0f, 100f);
        if (randomNumber < healthDropChance)
        {
            Debug.Log("spawned healthpack");
            GameObject healthPack = Instantiate(healthPackPrefab);
            healthPack.transform.position = this.transform.position;
        }



        fracturedRock.SetActive(true);

        if (rockBreakParticlesObj != null)
        {
            Transform particles = GameObject.Instantiate(rockBreakParticlesObj).transform;
            particles.position = transform.position;
            particles.parent = transform;
        }

        CameraShake.camShake?.StartShake(transform.position, 10f, 1f, 0.5f, true);
        fadeOut = true;
    }


    const float rockLifetime = 6;
    const float rockFadeTime = 10;
    private List<Rigidbody> rockRBs = new List<Rigidbody>();

    private IEnumerator RockFading()
    {
        //yield return null to space out loops over multiple frames
        int count = rockRBs.Count;
        while (true)
        {
            //loop through all rock pieces
            foreach (Rigidbody rb in rockRBs)
            {
                if (rb == null || rb.isKinematic) continue;

                //check if rock has settled
                if (rb.velocity.magnitude > .5f)
                {
                    float decay = Mathf.Pow(.5f, Time.deltaTime);
                    rb.velocity = Vector3.Scale(rb.velocity, new Vector3(decay, 1, decay));
                    yield return null;
                    continue;
                }

                //set to kinematic and tween under ground before destroying
                rb.isKinematic = true;
                rb.transform.GetComponentInChildren<Collider>().enabled = false;
                count--;
                rb.transform.DOMoveY(-10f, rockFadeTime).SetRelative().SetEase(Ease.InSine)
                    .OnComplete(() => { Destroy(rb.transform.gameObject, rockFadeTime); });
                yield return null;
            }

            if (count == 0) break;
            yield return null;
        }
        //int rockChildren = fractRock.transform.childCount;
        //for (int i = 0; i < rockChildren; i++) {
        //    Destroy(fractRock.transform.GetChild(i).gameObject);
        //}
    }
}
