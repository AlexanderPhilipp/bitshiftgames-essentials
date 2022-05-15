using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BitshiftGames.Essentials
{
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager s_singleton;

        [Header("Data")]
        public Achievement[] AvailableAchievements;

        [Header("Save Settings")]
        public string PlayerPrefBaseKey;

        [Header("Events")]
        public UnityEvent<Achievement> OnAchievementUnlocked = new UnityEvent<Achievement>();


        private void Awake()
        {
            if (s_singleton == null)
                s_singleton = this;
            else
                Destroy(this.gameObject);
        }

        public void SetAchievementValue(string achievementId, int newValue)
        {
            Achievement? targetAchievement = GetAchievement(achievementId);
            if (targetAchievement == null)
                return;

            if (targetAchievement.Value.ValueType != Achievement.AchievementValueType.IntegerValue)
                return;

            int prevValue = 0;
            GetAchievementValue(achievementId, out prevValue);
            if(prevValue < targetAchievement.Value.RequiredIntFloatValue &&
                newValue >= targetAchievement.Value.RequiredIntFloatValue)
            {
                Debug.Log("Called Event");
                OnAchievementUnlocked.Invoke(targetAchievement.Value);
            }

            string playerPrefKey = PlayerPrefBaseKey + targetAchievement.Value.AchievementId;
            PlayerPrefs.SetInt(playerPrefKey, newValue);
        }
        public void SetAchievementValue(string achievementId, float newValue)
        {
            Achievement? targetAchievement = GetAchievement(achievementId);
            if (targetAchievement == null)
                return;

            if (targetAchievement.Value.ValueType != Achievement.AchievementValueType.FloatValue)
                return;

            float prevValue = 0;
            GetAchievementValue(achievementId, out prevValue);
            if (prevValue < targetAchievement.Value.RequiredIntFloatValue &&
                newValue >= targetAchievement.Value.RequiredIntFloatValue)
            {
                Debug.Log("Called Event");
                OnAchievementUnlocked.Invoke(targetAchievement.Value);
            }

            string playerPrefKey = PlayerPrefBaseKey + targetAchievement.Value.AchievementId;
            PlayerPrefs.SetFloat(playerPrefKey, newValue);
        }
        public void SetAchievementValue(string achievementId, bool newValue)
        {
            Achievement? targetAchievement = GetAchievement(achievementId);
            if (targetAchievement == null)
                return;

            if (targetAchievement.Value.ValueType != Achievement.AchievementValueType.BoolValue)
                return;


            bool wasUnlocked = false;
            GetAchievementValue(achievementId, out wasUnlocked);
            if(!wasUnlocked && newValue)
            {
                OnAchievementUnlocked.Invoke(targetAchievement.Value);
                Debug.Log("Called Event");
            }

            string playerPrefKey = PlayerPrefBaseKey + targetAchievement.Value.AchievementId;
            PlayerPrefs.SetInt(playerPrefKey, newValue ? 1 : 0);
        }

        public Achievement? GetAchievement(string achievementId)
        {
            foreach(Achievement achievement in AvailableAchievements)
            {
                if(achievement.AchievementId == achievementId)
                {
                    return achievement;
                }
            }

            return null;
        }

        public void GetAchievementValue(string achievementId, out int storeValue)
        {
            Achievement? targetAchievement = GetAchievement(achievementId);
            if(targetAchievement == null)
            {
                storeValue = 0;
                return;
            }

            string playerPrefKey = PlayerPrefBaseKey + targetAchievement.Value.AchievementId;
            storeValue = PlayerPrefs.GetInt(playerPrefKey, 0);
            return;
        }
        public void GetAchievementValue(string achievementId, out float storeValue)
        {
            Achievement? targetAchievement = GetAchievement(achievementId);
            if (targetAchievement == null)
            {
                storeValue = 0;
                return;
            }

            string playerPrefKey = PlayerPrefBaseKey + targetAchievement.Value.AchievementId;
            storeValue = PlayerPrefs.GetFloat(playerPrefKey, 0);
            return;
        }
        public void GetAchievementValue(string achievementId, out bool storeValue)
        {
            Achievement? targetAchievement = GetAchievement(achievementId);
            if (targetAchievement == null)
            {
                storeValue = false;
                return;
            }

            string playerPrefKey = PlayerPrefBaseKey + targetAchievement.Value.AchievementId;
            storeValue = PlayerPrefs.GetInt(playerPrefKey, 0) == 0 ? false : true;
            return;
        }

        public bool GetAchievementUnlockState(string achievementId)
        {
            Achievement? targetAchievement = GetAchievement(achievementId);
            if (targetAchievement == null)
                return false;

            string playerPrefKey = PlayerPrefBaseKey + targetAchievement.Value.AchievementId;
            switch (targetAchievement.Value.ValueType)
            {
                case Achievement.AchievementValueType.IntegerValue:
                    return PlayerPrefs.GetInt(playerPrefKey, 0) >= targetAchievement.Value.RequiredIntFloatValue;
                case Achievement.AchievementValueType.FloatValue:
                    return PlayerPrefs.GetFloat(playerPrefKey, 0) >= targetAchievement.Value.RequiredIntFloatValue;
                case Achievement.AchievementValueType.BoolValue:
                    return PlayerPrefs.GetInt(playerPrefKey, 0) == 1;
                default:
                    return false;
            }
        }
    }

    [System.Serializable]
    public struct Achievement
    {
        public enum AchievementValueType
        {
            IntegerValue, FloatValue, BoolValue
        }

        [Header("General")]
        public string AchievementTitle_LocalizationKey;
        public string AchievementId;
        public string AchievementDescription_LocalizationKey;

        [Header("Visuals")]
        public Sprite AchievementIcon;

        [Header("Saving")]
        public AchievementValueType ValueType;

        [Header("Unlock Settings")]
        public int RequiredIntFloatValue;
        public bool ShowRequiredActionInMenu;

        public Achievement(string titleLocalizationKey, string id,
            string descriptionLocalizationKey, Sprite icon,
            AchievementValueType type, int requiredValue, bool showRequirements)
        {
            AchievementTitle_LocalizationKey = titleLocalizationKey;
            AchievementId = id;
            AchievementDescription_LocalizationKey = descriptionLocalizationKey;
            AchievementIcon = icon;
            ValueType = type;
            RequiredIntFloatValue = requiredValue;
            ShowRequiredActionInMenu = showRequirements;
        }
    }
}
