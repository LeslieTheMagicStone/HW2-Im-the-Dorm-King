using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class VignetteLogic : MonoBehaviour
{
    public static VignetteLogic Instance => instance;
    private static VignetteLogic instance;

    private Volume volume;
    private Vignette vignette;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }

    public IEnumerator Start()
    {
        float timer = 0.5f;
        while (timer > 0)
        {
            vignette.intensity.Override(timer / 0.5f);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        vignette.intensity.Override(0);
    }

    public IEnumerator Outro()
    {
        float timer = 0;
        while (timer < 2f)
        {
            vignette.intensity.Override(timer / 2f);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        vignette.intensity.Override(1);
    }
}
