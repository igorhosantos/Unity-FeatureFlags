using System.Collections;
using FeatureFlags.Services;
using UnityEngine;
using Utils;
using TMPro;

public class SampleStartup : MonoBehaviour
{
    [SerializeField] private RectTransform hudA;
    [SerializeField] private RectTransform hudB;
    [SerializeField] private RectTransform featureX;
    [SerializeField] private RectTransform featureY;

    private RectTransform _runtimeHud;
    
    void Start()
    {
        StartCoroutine(ProcessInitialization());
    }

    private IEnumerator ProcessInitialization()
    {
        IFeatureFlagService service = ServiceLocator.Get<IFeatureFlagService>();
        
        var newHud = service.IsFlagEnabled("hud_b");
        _runtimeHud  = newHud ?  hudB : hudA;
        _runtimeHud.gameObject.SetActive(true);

        var labelScore = _runtimeHud.GetComponentInChildren<TextMeshProUGUI>();
        labelScore.text = $"Score:{1235}";
        
        
        featureX.gameObject.SetActive(service.IsFlagEnabled("feature_x"));
        featureY.gameObject.SetActive(service.IsFlagEnabled("feature_y"));
        
        yield return null;
    }
}
