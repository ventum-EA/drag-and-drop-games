using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    string _adUnitId;

    [SerializeField] string _adUnitId2;
    [SerializeField] Button _rewardedAdButton;
    public FlyingObjectManager flyingObjectManager;
    void Awake()
    {
        _adUnitId = _androidAdUnitId;
        if(flyingObjectManager == null)
        {
                      flyingObjectManager = FindFirstObjectByType<FlyingObjectManager>();
        }
    }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        
    }
    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("[RewardedAds] Tried to load rewarded ad before Unity ads was initialized!");
            return;
        }
        Debug.Log("[RewardedAds] Loading rewarded ad...");
        Advertisement.Load(_adUnitId, this);

    }
        // Update is called once per frame
        void Update()
    {
        
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("[RewardedAds] Rewarded ad loaded and ready to show!");
        if(placementId.Equals(_adUnitId))
        {
            if (_rewardedAdButton != null)
            {
                _rewardedAdButton.interactable = true;
            }
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
       Debug.LogWarning($"[RewardedAds] Failed to load rewarded ad: {error.ToString()} - {message}");
        StartCoroutine(WaitAndLoad(5f));

    }
    public IEnumerator WaitAndLoad(float delay)
    {
               yield return new WaitForSeconds(delay);
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"[RewardedAds] Failed to show rewarded ad: {error.ToString()} - {message}");
        StartCoroutine(WaitAndLoad(5f));
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("[RewardedAds] Rewarded ad clicked!");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if(placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsCompletionState.COMPLETED))
        {
           
                Debug.Log("[RewardedAds] Rewarded ad completed! Granting reward to player.");
            flyingObjectManager.DestroyAllFlyingObjects();
            _rewardedAdButton.interactable = false;
            StartCoroutine(WaitAndLoad(10f));


            
        }
        Time.timeScale = 1f;
        
    }
    public void SetButton(Button button)
    {
        if (button == null)
        {
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowAd);
        _rewardedAdButton = button;
        _rewardedAdButton.interactable = false;
    }
    public void ShowAd()
    {
        _rewardedAdButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }
}
