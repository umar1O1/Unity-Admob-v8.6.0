using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAdScript : MonoBehaviour
{
    [SerializeField]
    private string adID = "";


    // App open ads can be preloaded for up to 4 hours.
    //private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
    //private DateTime _expireTime;
    private AppOpenAd appOpenAd;
 
    

    //private void OnEnable()
    //{
    //    // Use the AppStateEventNotifier to listen to application open/close events.
    //    // This is used to launch the loaded ad when we open the app.
    //    AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    //}

    //private void OnDestroy()
    //{
    //    // Always unlisten to events when complete.
    //    AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    //}

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd()
    {
        // Clean up the old ad before loading a new one.
        // Clean up the old ad before loading a new one.
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        Debug.Log("Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(adID, adRequest,
        (AppOpenAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.LogError("app open ad failed to load an ad " +
                                "with error : " + error);
                return;
            }

            Debug.Log("App open ad loaded with response : "
                        + ad.GetResponseInfo());

            appOpenAd = ad;
            RegisterEventHandlers(ad);
        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAd()
    {
        // App open ads can be preloaded for up to 4 hours.
        if (appOpenAd != null && appOpenAd.CanShowAd() /*&& DateTime.Now < _expireTime*/)
        {
            Debug.Log("Showing app open ad.");
            appOpenAd.Show();
        }
        else
        {
            Debug.LogError("App open ad is not ready yet.");
        }

    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (appOpenAd != null)
        {
            Debug.Log("Destroying app open ad.");
            appOpenAd.Destroy();
            appOpenAd = null;
        }


    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (appOpenAd != null)
        {
            var responseInfo = appOpenAd.GetResponseInfo();
            Debug.Log(responseInfo);
        }
    }

    //private void OnAppStateChanged(AppState state)
    //{
    //    Debug.Log("App State changed to : " + state);

    //    // If the app is Foregrounded and the ad is available, show it.
    //    if (state == AppState.Foreground)
    //    {
    //        ShowAd();
    //    }
    //}

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("App open ad full screen content opened.");

   
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("App open ad full screen content closed.");

            // It may be useful to load a new ad when the current one is complete.
            LoadAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("App open ad failed to open full screen content with error : "
                            + error);
        };
    }
}
