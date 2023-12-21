using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Api;


public class AdmobManager : MonoBehaviour
{
    [Header("Test Devices")]
    public bool useTestDevice;
    public List<string> testDeviceIdsList ;

    [Header("Ads Script")]
    [SerializeField]
    private OpenAdScript openAdScript;
    [SerializeField]
    private BannerAdScript bannerScript;
    [SerializeField]
    private RectBannerScript rectBannerScript;
    [SerializeField]
    private InterstitialAdScript interstitialScript;
    [SerializeField]
    private RewardedAdScript rewardedScript;

    ConsentForm _consentForm;

    bool adInitialized = false;

    public static AdmobManager Instance;
    [Header("Ad Canvas")]
    [SerializeField]
    private GameObject bannerBar;
    [Space(7)]
    [SerializeField]
    private GameObject rectBg;
    [Space(7)]
    [SerializeField]
    private GameObject interPanel;    
    [SerializeField]
    private Image interLoadingBar;
    [SerializeField]
    private float interLoadingTime;


    #region Singleton
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance= this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    #endregion


    private void Start()
    {
       
        InitUmpConsent();
;
    }

    #region AddingTestDevice
    void AddTestDevice() 
    {
        if (!useTestDevice)
            return;
        RequestConfiguration requestConfiguration = new();
        if (testDeviceIdsList.Count>0)
        {
            for (int i = 0; i < testDeviceIdsList.Count; i++)
            {
                requestConfiguration.TestDeviceIds.Add(testDeviceIdsList[i]);
            }          
        }
        MobileAds.SetRequestConfiguration(requestConfiguration);
    }
    #endregion

    #region UMP Consent

    private void InitUmpConsent()
    {
        ConsentRequestParameters finalRequest= new(); 
        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,

            TestDeviceHashedIds = testDeviceIdsList,
            
        };

        // Here false means users are not under age.
        ConsentRequestParameters request = new()
        {
            TagForUnderAgeOfConsent = false,
           
        };

        finalRequest = request;

        if (useTestDevice) 
        {
            ConsentRequestParameters debugRequest = new()
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings,
            };
            finalRequest = debugRequest;
            Debug.Log("UMP Debugging Consent");
        }
        
        // Check the current consent information status.
        ConsentInformation.Update(finalRequest, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    private void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
        }
    }


    private void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // Handle dismissal by reloading form.
        LoadConsentForm();
    }
    #endregion



    #region InitializeAdmob
    public void InitializeAdmob() 
    {
        
        MobileAds.Initialize(initStatus => 
        {
            adInitialized = true;
           
            InitHandler(); 
        });
        AddTestDevice();
    }


    private void InitHandler()
    {
        StartCoroutine(InitializeCoroutine());
    }
    private IEnumerator InitializeCoroutine()
    {

        yield return new WaitForSeconds(0.25f);
        LoadAppOpen();
        yield return new WaitForSeconds(0.25f);
        LoadRewadedAd();
        yield return new WaitForSeconds(0.25f);
        LoadInterstitialAd();
    }
    #endregion

    #region Ads Calling Function


    #region App Open Function

    public void LoadAppOpen()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        openAdScript.LoadAd();
    }
    public void ShowAppOpen()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        openAdScript.ShowAd();
    }
    #endregion

    #region Banner Functions

    public void ShowBannerAd()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        bannerBar.SetActive(true);
        bannerScript.LoadAd();
    }

    public void UnHideBanner() 
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        bannerBar.SetActive(true);
        bannerScript.ShowAd();
    }
    public void HideBanner()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        bannerBar.SetActive(false);
        bannerScript.HideAd();
    }
    public void DestroyBanner()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        bannerBar.SetActive(false);
        bannerScript.DestroyAd();
    }
    #endregion

    #region Rect Banner Functions

    public void LoadRectBannerAd()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
    
        rectBannerScript.LoadAd();
    }

    public void ShowRectBannerAd()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        rectBg.SetActive(true);
        rectBannerScript.ShowAd();
    }

    public void DestroyRectBanner()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        rectBg.SetActive(false);
        rectBannerScript.DestroyAd();
    }
    #endregion

    #region Inter Ad Functions
    public void LoadInterstitialAd()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        interstitialScript.LoadAd();
    }

    public bool IsInterstitialAvailable() 
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return false;
        }
        return interstitialScript.IsInterstitialAvaiable();
    }

    public void ShowInterstitial()
    {
        if (!adInitialized || StaticVariables.RemoveAds)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        if (interstitialScript.IsInterstitialAvaiable())
        {

            StartCoroutine(InterstitialRoutine());
        }
        else
        {
            LoadInterstitialAd();
        }
    }
    IEnumerator InterstitialRoutine() 
    {
        interLoadingBar.fillAmount = 0f;
        interPanel.SetActive(true);
        interLoadingBar.DOFillAmount(1, interLoadingTime).SetEase(Ease.Linear).SetUpdate(true);
        yield return new WaitForSecondsRealtime(interLoadingTime);
        interPanel.SetActive(false);
        interstitialScript.ShowAd();

    }
    #endregion

    #region Rewarded Ad Function

    public void LoadRewadedAd() 
    {
        if (!adInitialized /*|| StaticVariables.RemoveAds*/)
        {
            Debug.Log("Admob is not initialized");
            return;
        }

        rewardedScript.LoadAd();

    }
    public bool IsRewardedAdAvailable()
    {
        if (!adInitialized /*|| StaticVariables.RemoveAds*/)
        {
            Debug.Log("Admob is not initialized");
            return false;
        }
        return rewardedScript.IsRewardedAdAvaiable();

    }
    public void ShowRewadedAd(Action onSuccess=null, Action onFailed = null)
    {
        if (!adInitialized /*|| StaticVariables.RemoveAds*/)
        {
            Debug.Log("Admob is not initialized");
            return;
        }
        rewardedScript.ShowAd(onSuccess,onFailed);
    }
    #endregion

    #endregion
}
