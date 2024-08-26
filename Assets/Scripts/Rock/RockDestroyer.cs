using DG.Tweening;
using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RockDestroyer : MonoBehaviour
{
    [SerializeField]
    private GameObject fracturedRock;

    private GameObject fractRock;

    [SerializeField]
    private GameObject rock;

    public float fadeSpeed = 1.0f;
    private bool fadeOut = false;

    [SerializeField]
    private float 
        rockLifetime = 3,
        rockFadeTime = 6;

    [SerializeField]
    private Rigidbody[] rockRbs;

    [SerializeField]
    private SoundScripObj rockBreakSFX;

    [SerializeField]
    private GameObject rockBreakParticlesObj;

    // Start is called before the first frame update
    void Start()
    {
        if (rockRbs == null) GetRigidBodies();
    }

    [Button]
    private void GetRigidBodies()
    {
        //loop through rock pieces, build list of rigidbodies so i dont have to get them more than once
        fracturedRock.SetActive(true);

        var rockRBList = new List<Rigidbody>();
        foreach (Transform t in fracturedRock.transform) {
            rockRBList.Add(t.GetComponent<Rigidbody>());
        }
        rockRbs = rockRBList.ToArray();

        fracturedRock.SetActive(false);
    }

    public void StartRockFade()
    {
        fadeStartTime = Time.time;
        StartCoroutine(RockFadingSinking());
    }

    [SerializeField]
    private float outwardBreakForce = 3f;

    [Button]
    public void ChangeRock(Vector3? forceDir = null, float forceMag = 1f, Vector3? hitPos = null)
    {
        #if UNITY_EDITOR
            if (!Application.isPlaying) return;
        #endif 

        rockBreakSFX?.Play(wPos: transform.position);

        // Health Pack dropping logic
        float randomNumber = Random.Range(0f, 100f);

        rock.SetActive(false);
        fracturedRock.SetActive(true);

        //force optional checks
        if (!forceDir.HasValue) forceDir = Vector3.zero;
        if (!hitPos.HasValue) hitPos = transform.position;
        forceDir = forceDir.Value.normalized;

        //apply force
        foreach(var rb in rockRbs) {
            Vector3 finalForce = Vector3.zero;
            finalForce += forceDir.Value * forceMag * Vector3.Dot(forceDir.Value, rb.transform.position - hitPos.Value);
            finalForce += (rb.transform.position - transform.position).normalized * outwardBreakForce;
            rb.AddForce(finalForce, ForceMode.Impulse);
        }

        if (rockBreakParticlesObj != null) {
            Transform particles = GameObject.Instantiate(rockBreakParticlesObj).transform;
            particles.position = transform.position;
            particles.parent = transform;
        }

        //fadeOut = true;
        Invoke(nameof(StartRockFade), rockLifetime);
    }

    const int tickRate = 30;

    private float fadeStartTime = 0;
    private IEnumerator RockFadingSinking()
    {
        //yield return null to space out loops over multiple frames
        int count = rockRbs.Length;
        foreach(var rb in rockRbs) {
            rb.transform.GetComponentInChildren<Collider>().enabled = false;
        }

        while (true)
        {
            float t = (Time.time - fadeStartTime) / rockFadeTime;

            //loop through all rock pieces
            foreach (Rigidbody rb in rockRbs)
            {
                if (rb == null || rb.isKinematic) continue;

                /*
                //check if rock has settled
                if (rb.velocity.magnitude > .5f)
                {
                    //float decay = Mathf.Pow(.5f, 1f/tickRate);
                    //rb.transform.localScale *= decay;
                    
                    //rb.velocity = Vector3.Scale(rb.velocity, new Vector3(decay, decay, decay));
                    //rb.velocity *= decay;
                    //yield return null;
                    continue;
                }
                */

                rb.transform.localScale = Vector3.one * 100f * (1f - t);

                //set to kinematic and tween under ground before destroying
                //rb.isKinematic = true;
                //count--;
                /*
                rb.transform.DOMoveY(-10f, rockFadeTime).SetRelative().SetEase(Ease.InSine)
                    .OnComplete(() => { Destroy(rb.transform.gameObject, rockFadeTime); });
                */
                //yield return null;
            }

            //if (count == 0) break;
            if (t >= 1f) break;
            yield return new WaitForSecondsRealtime(1f/(float)tickRate);
        }

        GameObject.Destroy(this.gameObject);
        //int rockChildren = fractRock.transform.childCount;
        //for (int i = 0; i < rockChildren; i++) {
        //    Destroy(fractRock.transform.GetChild(i).gameObject);
        //}
    }
}
