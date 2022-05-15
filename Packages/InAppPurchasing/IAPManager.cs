using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using System;

namespace BitshiftGames.Essentials
{
    public class IAPManager : MonoBehaviour, IStoreListener
    {
        public static IAPManager singleton;
        IStoreController m_StoreController;
        Func<bool, int> cachedPurchaseCompleteCallback;

        [Header("Data")]
        public IapElement[] purchasableProducts;

        #region Singleton
        void Awake()
        {
            if (IAPManager.singleton == null)
                singleton = this;
            else
                Destroy(this);

            Initialize();
        }
        #endregion
        #region Initialization
        public void Initialize()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach(IapElement product in purchasableProducts)
            {
                builder.AddProduct(product.productId, product.productType);
            }

            UnityPurchasing.Initialize(this, builder);
        }
        #endregion
        #region PublicFunctions
        public void BuyProduct(string productId, Func<bool, int> OnPurchaseCompleteCallback)
        {
            IapElement foundProduct = null;
            foreach(IapElement product in purchasableProducts)
            {
                if (product.productId == productId)
                    foundProduct = product;
            }

            if (foundProduct == null)
            {
                OnPurchaseCompleteCallback.Invoke(false);
                return;
            }


            cachedPurchaseCompleteCallback = OnPurchaseCompleteCallback;
            m_StoreController.InitiatePurchase(foundProduct.productId);
        }
        public IapElement GetProductById(string productId)
        {
            IapElement foundProduct = null;
            foreach (IapElement product in purchasableProducts)
            {
                if (product.productId == productId)
                    foundProduct = product;
            }

            return foundProduct;
        }
        public void RefreshPriceTags()
        {
            foreach(IapElement element in purchasableProducts)
            {
                Product foundProduct = null;

                if (m_StoreController == null)
                    continue;

                foreach(Product p in m_StoreController.products.all)
                {
                    if (p.definition.id == element.productId)
                        foundProduct = p;
                }

                if (foundProduct == null)
                    continue;

                element.priceTag.text = foundProduct.metadata.localizedPriceString;
            }
        }
        public bool IapAvailable()
        {
            if(m_StoreController == null)
                return false;

            if (m_StoreController.products.all.Length <= 0)
                return false;

            foreach(Product product in m_StoreController.products.all)
            {
                if (product.definition.id == "")
                    return false;
            }

            return true;
        }
        #endregion
        #region Interface Functions
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"In-App Purchasing initialize failed: {error}");
        }
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            foreach (IapElement element in purchasableProducts)
            {
                if (element.productId == product.definition.id)
                {
                    foreach (IapElement.SaveEntry saveE in element.saveEntries)
                    {
                        if (saveE.entryType == IapElement.SaveEntry.SaveEntryType.ConsumableVal)
                        {
                            PlayerPrefs.SetInt(saveE.valueSavePlayerPref,
                                PlayerPrefs.GetInt(saveE.valueSavePlayerPref, 0) + saveE.valueAddValue);
                        }
                        else if (saveE.entryType == IapElement.SaveEntry.SaveEntryType.NonConsumable)
                        {
                            PlayerPrefs.SetInt(saveE.valueSavePlayerPref, 1);
                        }
                    }

                    if (element.productType == ProductType.NonConsumable)
                    {
                        PlayerPrefs.SetInt(element.productPlayerPrefSaveKey, 1);
                    }
                }
            }
            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            if (cachedPurchaseCompleteCallback != null)
                cachedPurchaseCompleteCallback.Invoke(true);

            return PurchaseProcessingResult.Complete;
        }
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");

            if (cachedPurchaseCompleteCallback != null)
                cachedPurchaseCompleteCallback.Invoke(false);
        }
        #endregion
        #region Classes
        [System.Serializable]
        public class IapElement
        {
            [Header("General Settings")]
            public string productName;
            public string productId;
            [Space(5)]
            public ProductType productType;

            [Header("UI Components")]
            public TextMeshProUGUI priceTag;

            [Header("Save Settings")]
            public SaveEntry[] saveEntries;
            public string productPlayerPrefSaveKey;

            [System.Serializable]
            public class SaveEntry
            {
                public enum SaveEntryType
                {
                    ConsumableVal, NonConsumable
                }

                public SaveEntryType entryType;
                public string valueSavePlayerPref;
                [Space(5)]
                public int valueAddValue;
            }
        }
        #endregion
    }
}
