using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace BitshiftGames.Essentials
{
    public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public static AdsManager singleton;

        [Header("Settings")]
        public bool IsTestMode = false;
        [Space(5)]
        public string IosAppId;
        public string AndroidAppId;
        [Space(5)]
        public string BannerAdPlacementIdIos;
        public string BannerAdPlacementIdAndroid;
        public string RewardedAdPlacementIdIos;
        public string RewardedAdPlacementIdAndroid;


        string m_gameId;
        string m_bannerId;
        string m_rewardedId;

        bool bannerAdLoading = false;

        [HideInInspector]
        public bool rewardedAdReady = false;
        [HideInInspector]
        public bool rewardedAdLoading = false;

        void Awake()
        {
            if (singleton != null)
                Destroy(this.gameObject);
            else
                singleton = this;


            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                m_gameId = IosAppId;
                m_bannerId = BannerAdPlacementIdIos;
                m_rewardedId = RewardedAdPlacementIdIos;
            }
            else
            {
                m_gameId = AndroidAppId;
                m_bannerId = BannerAdPlacementIdAndroid;
                m_rewardedId = RewardedAdPlacementIdAndroid;
            }

            Debug.Log("Initializing Advertisement Manager");
            Advertisement.Initialize(m_gameId, IsTestMode);
        }

        void Update()
        {
            if (!Advertisement.Banner.isLoaded && !bannerAdLoading)
                LoadBannerAd();

            if (!rewardedAdLoading && !rewardedAdReady)
                LoadRewardedAd();
        }

        #region BannerAds
        public void LoadBannerAd()
        {
            Debug.Log("Loading Banner Ad");
            bannerAdLoading = true;
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerLoadError
            };

            Advertisement.Banner.Load(m_bannerId, options);
        }
        public void ShowBannerAd()
        {
            if (Advertisement.Banner.isLoaded)
            {
                BannerOptions options = new BannerOptions
                {
                    clickCallback = OnBannerClicked,
                    hideCallback = OnBannerHidden,
                    showCallback = OnBannerShown
                };

                Advertisement.Banner.Show(m_bannerId, options);
            }
        }
        public void HideBannerAd()
        {
            Advertisement.Banner.Hide();
        }


        public void OnBannerLoaded()
        {
            bannerAdLoading = false;
            Debug.Log("Banner Ad Loaded");
        }
        public void OnBannerLoadError(string error)
        {
            bannerAdLoading = false;
            Debug.Log("Banner Ad failed loading");
        }

        public void OnBannerClicked() { }
        public void OnBannerHidden() { }
        public void OnBannerShown() { }
        #endregion
        #region RewardedAd
        Action<bool> cachedRewaredCompleteCallback = null;
        public void LoadRewardedAd()
        {
            if(!rewardedAdLoading && !rewardedAdReady)
            {
                Debug.Log("Loading rewarded Ad");
                rewardedAdLoading = true;
                Advertisement.Load(m_rewardedId, this);
            }
        }
        public void ShowRewarded(Action<bool> OnRewardedCompleteCallback)
        {
            if (rewardedAdReady)
            {
                cachedRewaredCompleteCallback = OnRewardedCompleteCallback;
                Advertisement.Show(m_rewardedId, this);
            }
        }
        #endregion
        #region AccessFunctions
        public void OnInitializationComplete()
        {
            Debug.Log("Advertisement Manager Initialized");
        }
        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Advertisement Manager Initialization Failed: {error.ToString()} - {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log("Loaded Ad with Id: " + placementId);
            if (placementId == m_rewardedId)
            { 
                rewardedAdReady = true;
                rewardedAdLoading = false;
            }

        }
        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log("Failed loading Ad with Id: " + placementId + " Error: " + error.ToString() + " Message: " + message);
        }
        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log("Failed showing Ad with Id: " + placementId + " Error: " + error.ToString() + " Message: " + message);
        }
        public void OnUnityAdsShowStart(string placementId) { }
        public void OnUnityAdsShowClick(string placementId) { }
        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if(placementId == m_rewardedId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
                cachedRewaredCompleteCallback.Invoke(true);
        }
        #endregion
    }
}
