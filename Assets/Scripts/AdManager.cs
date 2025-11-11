using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AdsInitializer adsInitializer;
    public InterstitialAds interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;
    private bool firstAdShown = false;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;  

    public static AdManager Instance { get; private set; }


    private void Awake()
    {
        if(adsInitializer == null)
        {
            adsInitializer = FindFirstObjectByType<AdsInitializer>();
        }
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        adsInitializer.OnAdsInitialized += HandleAdsInitialized;
    }

    private void HandleAdsInitialized()
    {
        
        if (!turnOffInterstitialAd || interstitialAd.isReady)
        {
            interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
            interstitialAd.LoadAd();
        }
        if (!turnOffRewardedAds)
        {
            rewardedAds.LoadAd();
        }
    }
    private void HandleInterstitialReady()
    {

        if (!firstAdShown)
        {
            Debug.Log("[AdManager] Showing first time interstitial ad automatically!");
            interstitialAd.ShowAd();
            firstAdShown = true;
        }
        else
        {
            Debug.Log("[AdManager] Next interstitial ad is ready for manual show!");
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        

    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private bool firstSceneLoad = false;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        firstAdShown = false;
        
        if (interstitialAd == null)
        {
            interstitialAd = FindFirstObjectByType<InterstitialAds>();
        }
        Button interstitialButton = GameObject.FindGameObjectWithTag("InterstitialAdButton")?.GetComponent<Button>();
        if(interstitialAd!=null && interstitialButton != null)
        {
            interstitialAd.SetButton(interstitialButton);
        }
        if (!firstSceneLoad)
        {
            firstSceneLoad = true;
            Debug.Log("First time scene loaded!");
            return;
        }
        Debug.Log("Scene loaded!");
        HandleAdsInitialized();
        if (rewardedAds == null)
        {
            rewardedAds = FindFirstObjectByType<RewardedAds>();
        }
        Button rewardedAdButton = GameObject.FindGameObjectWithTag("RewardedAdButton")?.GetComponent<Button>();
        if(rewardedAds!=null && rewardedAdButton!= null)
        {
            rewardedAds.SetButton(rewardedAdButton); 
        }
    }
}
