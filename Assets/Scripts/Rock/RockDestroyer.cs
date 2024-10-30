using DG.Tweening;
using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

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

    private bool calledDestroyEvent = false;

    // Start is called before the first frame update
    void Start()
    {
        if (rockRbs == null) {
            GetRigidBodies();
        }
    }

    [Button]
    private void GetRigidBodies()
    {
        //loop through rock pieces, build list of rigidbodies so i dont have to get them more than once
        fracturedRock.SetActive(true);

        var rockRBList = new List<Rigidbody>();
        foreach (Transform t in fracturedRock.transform)
        {
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

        var ogRB = rock.GetComponent<Rigidbody>();

        fracturedRock.transform.position = rock.transform.position;
        fracturedRock.transform.rotation = Quaternion.Euler(rock.transform.rotation.eulerAngles + Vector3.right * 90f);
        rock.SetActive(false);
        fracturedRock.SetActive(true);

        //force optional 
        if (!forceDir.HasValue) forceDir = Vector3.zero;
        if (!hitPos.HasValue) hitPos = transform.position;
        forceDir = forceDir.Value.normalized;

        //apply force
        foreach (var rb in rockRbs)
        {
            Vector3 finalForce = ogRB.velocity;
            //finalForce += forceDir.Value * forceMag * Vector3.Dot(forceDir.Value, rb.transform.position - hitPos.Value);
            finalForce += (rb.transform.position - fracturedRock.transform.position).normalized * outwardBreakForce;
            //finalForce += (rb.transform.position - hitPos.Value).normalized * outwardBreakForce;
            rb.AddForce(finalForce, ForceMode.Impulse);
        }

        //if (rockBreakParticlesObj != null)
        //{
        //    Transform particles = GameObject.Instantiate(rockBreakParticlesObj, transform).transform;
        //    particles.position = rock.transform.position;
        //}
        RockParticleManager.PlayRockParticlesAt(rock.transform.position);

        //fadeOut = true;
        Invoke(nameof(StartRockFade), rockLifetime);
    }

    const int tickRate = 30;

    private float fadeStartTime = 0;
    private IEnumerator RockFadingSinking()
    {
        var activeRockRbs = new List<Rigidbody>(rockRbs);

        while (true) {
            float t = (Time.time - fadeStartTime) / rockFadeTime;

            for (int i = activeRockRbs.Count - 1; i >= 0; i--) {
                Rigidbody rb = activeRockRbs[i];

                if (rb == null || rb.gameObject == null) {
                    activeRockRbs.RemoveAt(i);
                    continue;
                }

                rb.transform.localScale = Vector3.one * 100f * (1f - t);
            }

            if (activeRockRbs.Count == 0) break;

            if (t >= 1f) break;
            yield return new WaitForSecondsRealtime(1f / (float)tickRate);
        }

        SEvent_RockDestroyed?.Invoke();
        GameObject.Destroy(this.gameObject);
    }


    public static UnityEvent SEvent_RockDestroyed = new UnityEvent();
}
