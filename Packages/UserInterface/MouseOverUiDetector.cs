using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BitshiftGames.Essentials.UI
{
    [RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
    public class MouseOverUiDetector : MonoBehaviour
    {
        #region Singleton
        public static MouseOverUiDetector singleton;
        #endregion
        #region Runtime
        public bool PointerIsOverUi = false;
        #endregion

        private void Awake()
        {
            #region Singleton
            if (singleton != null)
                Destroy(this);
            else
                singleton = this;
            #endregion
        }

        public void PointerEnterUi()
        {
            PointerIsOverUi = true;
        }
        public void PointerExitUi()
        {
            PointerIsOverUi = false;
        }
    }
}
