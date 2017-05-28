

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

// http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
// search for "Mod integration into Stock Settings

namespace NRAP
{
    public class NRAPCustomParams : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "NRAP"; } }
        public override string DisplaySection { get { return "NRAP"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }


        [GameParameters.CustomFloatParameterUI("Size 0", minValue = 0.625f, maxValue = 5f, displayFormat = "N3", toolTip ="3 decimals")]
        public float size0 = 0.625f;
        [GameParameters.CustomFloatParameterUI("Size 1", minValue = 0.625f, maxValue = 5f, displayFormat = "N3", toolTip = "3 decimals")]
        public float size1 = 0;
        [GameParameters.CustomFloatParameterUI("Size 2", minValue = 0.625f, maxValue = 5f, displayFormat = "N2", toolTip = "2 decimals")]
        public float size2 = 1.25f;
        [GameParameters.CustomFloatParameterUI("Size 3", minValue = 0.625f, maxValue = 5f, displayFormat = "N2", toolTip = "2 decimals")]
        public float size3 = 0;
        [GameParameters.CustomFloatParameterUI("Size 4", minValue = 0.625f, maxValue = 5f, displayFormat = "N2", toolTip = "2 decimals")]
        public float size4 = 0;
        [GameParameters.CustomFloatParameterUI("Size 5", minValue = 0.625f, maxValue = 5f, displayFormat = "N1", toolTip = "1 decimal")]
        public float size5 = 2.5f;
        [GameParameters.CustomFloatParameterUI("Size 6", minValue = 0.625f, maxValue = 5f, displayFormat = "N1", toolTip = "1 decimal")]
        public float size6 = 0;
        [GameParameters.CustomFloatParameterUI("Size 7", minValue = 0.625f, maxValue = 5f, displayFormat = "N1", toolTip = "1 decimal")]
        public float size7 = 3.75f;
        [GameParameters.CustomFloatParameterUI("Size 8", minValue = 0.625f, maxValue = 5f, displayFormat = "N1", toolTip = "1 decimal")]
        public float size8 = 0;
        [GameParameters.CustomFloatParameterUI("Size 9", minValue = 0.625f, maxValue = 5f, displayFormat = "N1", toolTip = "1 decimal")]
        public float size9 = 5;


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        { }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}