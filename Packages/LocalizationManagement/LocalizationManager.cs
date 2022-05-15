using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace BitshiftGames.Essentials
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager singleton;

        public string currentLocaleCountryCode = "ja";

        [Header("Data")]
        public LocalizedElement[] localizedElements;
        public LocalizationDataContainer localizationData;

        [Header("Events")]
        public UnityEvent OnRefreshElements = new UnityEvent();


        private void Awake()
        {
            if (LocalizationManager.singleton == null)
                singleton = this;
            else
                Destroy(this);
        }

        string lastLocale = "";
        private void Update()
        {
            if(lastLocale != currentLocaleCountryCode)
            {
                lastLocale = currentLocaleCountryCode;
                RefreshAllElements();
            }
        }

        void RefreshAllElements()
        {
            foreach(LocalizedElement element in localizedElements)
            {
                bool translationNotFound = false;

                LocalizationEntry? linkedEntry = localizationData.GetEntry(element.entryId);
                if (linkedEntry == null)
                    translationNotFound = true;

                if (!translationNotFound)
                {
                    LocalizationEntry.EntryTranslation? targetTranslation = linkedEntry.Value.GetTranslation(currentLocaleCountryCode);
                    if(targetTranslation != null)
                    {
                        element.textComponent.text = targetTranslation.Value.localizedString;
                        element.textComponent.font = targetTranslation.Value.localizedFont;
                        continue;
                    }
                }

                element.textComponent.text = element.fallbackString;
                element.textComponent.font = element.fallbackFont;
            }

            OnRefreshElements.Invoke();
        }

        public string GetLocalizedString(string entryKey)
        {
            bool translationNotFound = false;

            LocalizationEntry? linkedEntry = localizationData.GetEntry(entryKey);
            if (linkedEntry == null)
                translationNotFound = true;

            if (!translationNotFound)
            {
                LocalizationEntry.EntryTranslation? targetTranslation = linkedEntry.Value.GetTranslation(currentLocaleCountryCode);
                if (targetTranslation != null)
                {
                    return targetTranslation.Value.localizedString;
                }
            }

            return "";
        }

        public TMP_FontAsset GetLocalizedFont(string entryKey)
        {
            bool translationNotFound = false;

            LocalizationEntry? linkedEntry = localizationData.GetEntry(entryKey);
            if (linkedEntry == null)
                translationNotFound = true;

            if (!translationNotFound)
            {
                LocalizationEntry.EntryTranslation? targetTranslation = linkedEntry.Value.GetTranslation(currentLocaleCountryCode);
                if (targetTranslation != null)
                {
                    return targetTranslation.Value.localizedFont;
                }
            }

            return null;
        }

        #region Classes
        [System.Serializable]
        public struct LocalizedElement
        {
            [Header("Settings")]
            public string entryId;
            [TextArea] public string fallbackString;
            public TMP_FontAsset fallbackFont;

            [Header("Components")]
            public TextMeshProUGUI textComponent;
        }
        #endregion
    }
}
