using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    string _adUnitId;
    public event Action OnInterstitialAdReady;
    public bool isReady = false;
    [SerializeField] Button _interstitialAdButton;
    void Awake()
    {
        _adUnitId = _androidAdUnitId;
    }
    private void Update()
    {
        if (AdManager.Instance != null && AdManager.Instance.interstitialAd != null) {
            _interstitialAdButton.interactable = isReady;
        }
    }
    public void OnInterstitialAdButtonClicked()
    {
        Debug.Log("[InterstitialAds] Interstitial ad button clicked!");
        ShowInterstitial();
    }
    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("[InterstitialAds] Tried to load interstitial ad before Unity ads was initialized!");
            return;
        }
        Debug.Log("[InterstitialAds] Loading interstitial ad...");
        Advertisement.Load(_adUnitId, this);
    }
    public void ShowInterstitial()
    {
     if(AdManager.Instance.interstitialAd!=null && isReady)
        {
            Debug.Log("[InterstitialAds] Showing interstitial ad manually...");
            ShowAd();

        }
        else
        {
            Debug.Log("[InterstitialAds] Interstitial ad not ready yet, loading again!");
            LoadAd();
        }
    }
    public void ShowAd()
    {
        if (isReady)
        {
            //if(AdManager.Instance)
            Advertisement.Show(_adUnitId, this);
            isReady = false;
        }
        else
        {
            Debug.LogWarning("[InterstitialAds] Tried to show interstitial ad before it was ready!");
            LoadAd();
        }
    }
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("[InterstitialAds] Interstitial ad loaded!");
        _interstitialAdButton.interactable = true;
        isReady = true;
        OnInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"[InterstitialAds] Interstitial ad failed to load: {error.ToString()} - {message}");
        LoadAd();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("[InterstitialAds] Interstitial ad clicked!");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Time.timeScale = 1f; // Ensure time scale is normal after ad
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("[InterstitialAds] Interstitial ad completed successfully!");
            StartCoroutine(SlowDownTimeTemporarily(30f));
                        LoadAd();
        }
        else
        {
            Debug.Log("[InterstitialAds] Interstitial ad was not completed.");
            LoadAd();
        }
    }
    private IEnumerator SlowDownTimeTemporarily(float slowDuration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.4f; // Slow down time to half speed\
        Debug.Log("[InterstitialAds] Time slowed down for "+slowDuration+" seconds!");
        yield return new WaitForSecondsRealtime(slowDuration);
        Time.timeScale = originalTimeScale; // Restore original time scale
        Debug.Log("[InterstitialAds] Time restored to normal!");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"[InterstitialAds] Interstitial ad failed to show: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("[InterstitialAds] Showing interstitial ad at this moment");
        Time.timeScale = 0f; // Ensure time scale is normal when ad starts
    }
    public void SetButton(Button button)
    {
        if (button == null)
        {
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnInterstitialAdButtonClicked);
        _interstitialAdButton = button;
        _interstitialAdButton.interactable = false;

    }

}
