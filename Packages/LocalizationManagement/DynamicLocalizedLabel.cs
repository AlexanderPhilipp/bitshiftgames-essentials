using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BitshiftGames.Essentials
{
    public class DynamicLocalizedLabel : MonoBehaviour
    {
        public TextMeshProUGUI label;
        public TMP_Text worldSpaceLabel;
        public string labelLocalizationKey;

        private void Start()
        {
            if (LocalizationManager.singleton != null)
                LocalizationManager.singleton.OnRefreshElements.AddListener(RefreshLabel);

            RefreshLabel();
        }

        public void RefreshLabel()
        {
            if(label != null)
            {
                label.text = LocalizationManager.singleton.GetLocalizedString(labelLocalizationKey);
                label.font = LocalizationManager.singleton.GetLocalizedFont(labelLocalizationKey);
            }

            if(worldSpaceLabel != null)
            {
                worldSpaceLabel.text = LocalizationManager.singleton.GetLocalizedString(labelLocalizationKey);
                worldSpaceLabel.font = LocalizationManager.singleton.GetLocalizedFont(labelLocalizationKey);
            }
        }
    }
}
