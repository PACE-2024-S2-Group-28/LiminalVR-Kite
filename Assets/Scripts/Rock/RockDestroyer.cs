using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

/// <summary>
/// script that handles the destruction of rock asteroids. Switching from whole to pieces and fading the pieces out.
/// </summary>
public class RockDestroyer : MonoBehaviour
{
    [SerializeField]
    private GameObject rockPiecesParentGO;

    [SerializeField]
    private GameObject wholeRockGO;

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
        
        StartCoroutine(FadeInScale());
    }

    float scaleInTime = 1f;

    private float easeInOutQuad(float x) {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    private IEnumerator FadeInScale()
    {
        Vector3 startScale = wholeRockGO.transform.localScale;

        float timer = 0f;
        while (timer <= scaleInTime) {
            wholeRockGO.transform.localScale = Vector3.Lerp(Vector3.zero, startScale, easeInOutQuad(timer/scaleInTime));
            timer += Time.deltaTime;
            yield return null;
        }

        wholeRockGO.transform.localScale = startScale;
    }

    [Button]
    private void GetRigidBodies()
    {
        //loop through rock pieces, build list of rigidbodies so i dont have to get them more than once
        rockPiecesParentGO.SetActive(true);

        var rockRBList = new List<Rigidbody>();
        foreach (Transform t in rockPiecesParentGO.transform)
        {
            rockRBList.Add(t.GetComponent<Rigidbody>());
        }
        rockRbs = rockRBList.ToArray();

        rockPiecesParentGO.SetActive(false);
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

        var ogRB = wholeRockGO.GetComponent<Rigidbody>();

        rockPiecesParentGO.transform.position = wholeRockGO.transform.position;
        rockPiecesParentGO.transform.rotation = Quaternion.Euler(wholeRockGO.transform.rotation.eulerAngles + Vector3.right * 90f);
        wholeRockGO.SetActive(false);
        rockPiecesParentGO.SetActive(true);

        //force optional 
        if (!forceDir.HasValue) forceDir = Vector3.zero;
        if (!hitPos.HasValue) hitPos = transform.position;
        forceDir = forceDir.Value.normalized;

        //apply force
        foreach (var rb in rockRbs)
        {
            Vector3 finalForce = ogRB.velocity;
            finalForce += (rb.transform.position - rockPiecesParentGO.transform.position).normalized * outwardBreakForce;
            rb.AddForce(finalForce, ForceMode.Impulse);
        }

        RockParticleManager.PlayRockParticlesAt(wholeRockGO.transform.position);

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
            //yield return new WaitForSecondsRealtime(1f / (float)tickRate);
            yield return null;
        }
    
        SEvent_RockDestroyed?.Invoke();

        GameObject.Destroy(this.gameObject);
    }


    public static UnityEvent SEvent_RockDestroyed = new UnityEvent();
}
