using KSP.Localization;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;
using System.Collections.Generic;

/* NRAP Test Weights is licensed under CC-BY-SA. All Rights for the original mod and for attribution 
 * go to him, excepted for this code, which is the work of Christophe Savard (stupid_chris).*/

namespace NRAP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class NRAPIconManager : MonoBehaviour
    {
        #region Methods
        private void CorrectIcon()
        {
            List<PartCategorizer.Category> pcc = null;
            
            if (PartCategorizer.Instance != null && PartCategorizer.Instance.filters != null && PartCategorizer.Instance.iconLoader != null && PartCategorizer.Instance.filters.Count > 0 )
            {
                // The original code was throwing an error upon entering the editor (commented below), so this
                // seems to do the same thing without Linq
                for (int i = 0; i < PartCategorizer.Instance.filters.Count; i++)
                {
                    var ab = PartCategorizer.Instance.filters[i];
                    if (ab != null && ab.button != null && ab.button.categoryName != null)
                    {
                        if (ab.button.categoryName == Localizer.Format("#LOC_NRAP_21"))
                        {
                            pcc = ab.subcategories;
                            break;
                        }
                    }
                }
                //var pcc = PartCategorizer.Instance.filters.Find(f => f != null && f.button != null && f.button.categoryName != null && f.button.categoryName == "Filter by module").subcategories;
 
                if (pcc != null)
                {

                    var b1 = pcc.Single(s => s.button != null && s.button.categoryName == Localizer.Format("#LOC_NRAP_22"));

                    if (b1 != null && b1.button != null)
                    {
                        b1.button.SetIcon(PartCategorizer.Instance.iconLoader.GetIcon(Localizer.Format("#LOC_NRAP_23")));
                    }
                }
            }
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            GameEvents.onGUIEditorToolbarReady.Add(CorrectIcon);
        }
        private void OnDestroy()
        {
            GameEvents.onGUIEditorToolbarReady.Remove(CorrectIcon);
        }
        #endregion
    }
}
