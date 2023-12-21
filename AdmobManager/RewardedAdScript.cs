using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardedAdScript : MonoBehaviour
{
    [SerializeField]
    private string adID = "";

    private RewardedAd _rewardedAd;


    private Action onSuccesWatch = null;
    private Action onFailedWatch = null;

    private bool rewardForUser;
    private bool noRewardForUser;
    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(adID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;

                RegisterEventHandlers(ad);
            });
    }
    public bool IsRewardedAdAvaiable() 
    {
        return _rewardedAd.CanShowAd();
    }
    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAd(Action _onSuccess,Action _onFailed)
    {
        const string rewardMsg =
        "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            onSuccesWatch = _onSuccess;
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                rewardForUser = true;
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });

        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
            noRewardForUser = true;
            onFailedWatch = _onFailed;
            LoadAd();
        }


    }

    private void Update()
    {
        if (rewardForUser) 
        {
            rewardForUser = false;
            onSuccesWatch?.Invoke();
        }
        if (noRewardForUser)
        {
            noRewardForUser = false;
            onFailedWatch?.Invoke();
        }
    }
    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (_rewardedAd != null)
        {
            var responseInfo = _rewardedAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");

            LoadAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                + error);
            LoadAd();
        };
    }
}
