using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BitshiftGames.Essentials
{
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Bitshift Games/Localization/LocalizationData")]
    public class LocalizationDataContainer : ScriptableObject
    {
        [Header("String Localization")]
        public LocalizationEntry[] localizationEntries;

        public LocalizationEntry? GetEntry(string entryKey)
        {
            foreach(LocalizationEntry entry in localizationEntries)
            {
                if (entry.entryKey == entryKey)
                    return entry;
            }

            return null;
        }
    }

    [System.Serializable]
    public struct LocalizationEntry
    {
        public string entryKey;
        public EntryTranslation[] translations;

        public EntryTranslation? GetTranslation(string countryCode)
        {
            foreach(EntryTranslation translation in translations)
            {
                if (translation.countryCode == countryCode)
                    return translation;
            }
            return null;
        }


        [System.Serializable]
        public struct EntryTranslation
        {
            public string countryCode;
            [TextArea] public string localizedString;
            public TMP_FontAsset localizedFont;
        }
    }
}
