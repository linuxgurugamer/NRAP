using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ClickThroughFix;

/* NRAP Test Weights is licensed under CC-BY-SA. All Rights for the original mod and for attribution 
 * go to him, excepted for this code, which is the work of Christophe Savard (stupid_chris).*/

namespace NRAP
{
    public class ModuleTestWeight : PartModule, IPartMassModifier, IPartCostModifier
    {
        #region KSPFields
        [KSPField]
        public float maxMass = 100;

        [KSPField(isPersistant = true)]
        public float minMass = 0.01f;

        [KSPField]
        public float maxHeight = 5;

        [KSPField]
        public float minHeight = 0.2f;

        [KSPField]
        public float weightCost = 0.1f;

        [KSPField(isPersistant = true)]
        public float baseDiameter = 0f;

        [KSPField(isPersistant = true, guiActive = true, guiFormat = "0.###", guiName = "Total mass", guiUnits = "t", guiActiveEditor = true)]
        public float currentMass;

        [KSPField(isPersistant = true)]
        public string mass = string.Empty;

        [KSPField(isPersistant = true)]
        private int size = 2;

        [KSPField(isPersistant = true)]
        public bool snapDiameter = true;

        [KSPField(isPersistant = true)]
        public float deltaMass;

        [KSPField(isPersistant = true)]
        private float height = 1, top, bottom;

        [KSPField(isPersistant = true)]
        private float currentBottom, currentTop;

        [KSPField(isPersistant = true)]
        public bool initiated;
        #endregion

        #region Fields
        private readonly int id = Guid.NewGuid().GetHashCode();
        private Rect window, drag;
        private bool visible;

        const float MIN_SIZE = 0.625f;
        const float MAX_SIZE = 5f;

        Dictionary<int, float> sizes = new Dictionary<int, float>();
        int sizecnt = 0;

        void SetDefaults()
        {
            sizeMassNeedsUpdating = true;
            this.deltaMass = 0;
            this.mass = this.part.partInfo.partPrefab.mass.ToString();
            this.currentMass = this.part.TotalMass();

            this.height = 1;
            this.size = 2;
            this.baseDiameter = GetSize(this.size);
            this.width = this.baseDiameter / 2.5f;
        }

        int getNodeSize(float dia)
        {
            if (dia <= 0.626f)
                return 0;
            if (dia <= 1.26f)
                return 1;
            if (dia <= 2.51f)
                return 2;
            if (dia < 3.75)
                return 3;
            return 4;
        }

        private float width, baseHeight, baseRadial;
        #endregion

        #region Part GUI
        [KSPEvent(active = true, guiActive = false, guiActiveEditor = true, guiName = "Toggle NRAP window")]
        public void GUIToggle()
        {
            CloseOpenedWindow();
            this.visible = !this.visible;
        }
        #endregion

        #region Methods
        private bool CheckParentNode(AttachNode node)
        {
            return node.attachedPart != null && node.attachedPart == this.part?.parent;
        }

        private void UpdateSize()
        {
            AttachNode bottomNode;
            AttachNode topNode;
            bool hasTopNode = this.part.TryGetAttachNodeById(Localizer.Format("#LOC_NRAP_1"), out topNode);
            bool hasBottomNode = this.part.TryGetAttachNodeById(Localizer.Format("#LOC_NRAP_2"), out bottomNode);

            float radialFactor = this.baseRadial * this.width;
            float heightFactor = this.baseHeight * this.height;
            //Transform root = this.part.transform.GetChild(0);
            //Transform root = this.part.partTransform.FindChild("model");
            Transform root = this.part.partTransform.Find(Localizer.Format("#LOC_NRAP_3"));
            float originalX = root.localScale.x;
            float originalY = root.localScale.y;
            root.localScale = new Vector3(radialFactor, heightFactor, radialFactor);
            //         this.baseDiameter = GetSize(this.size);

            //If part is root part
            if ((HighLogic.LoadedSceneIsEditor && this.part == EditorLogic.SortedShipList[0]) || (HighLogic.LoadedSceneIsFlight && this.vessel.rootPart == this.part))
            {
                if (hasTopNode)
                {
                    float originalTop = topNode.position.y;
                    topNode.position.y = this.top * this.height;
                    this.currentTop = topNode.position.y;
                    if (topNode.attachedPart != null)
                    {
                        float topDifference = this.currentTop - originalTop;
                        topNode.attachedPart.transform.Translate(0, topDifference, 0, this.part.transform);
                    }
                }

                if (hasBottomNode)
                {
                    float originalBottom = bottomNode.position.y;
                    bottomNode.position.y = this.bottom * this.height;
                    this.currentBottom = bottomNode.position.y;
                    if (bottomNode.attachedPart != null)
                    {
                        float bottomDifference = this.currentBottom - originalBottom;
                        //bottomNode.attachedPart.transform.Translate(0, bottomDifference, 0, this.part.transform);
                        bottomNode.attachedPart.transform.Translate(0, -bottomDifference, 0, bottomNode.attachedPart.transform);

                    }
                }
            }

            //If parent part is attached to bottom node
            else if (hasBottomNode && CheckParentNode(bottomNode))
            {
                float originalBottom = bottomNode.position.y;
                bottomNode.position.y = this.bottom * this.height;
                this.currentBottom = bottomNode.position.y;
                float bottomDifference = this.currentBottom - originalBottom;
                this.part.transform.Translate(0, -bottomDifference, 0, this.part.transform);

                if (hasTopNode)
                {
                    float originalTop = topNode.position.y;
                    topNode.position.y = this.top * this.height;
                    this.currentTop = topNode.position.y;
                    float topDifference = this.currentTop - originalTop;
                    topNode.attachedPart.transform.Translate(0, -(bottomDifference -  topDifference)/2, 0, this.part.transform);
                }
            }

            //If parent part is attached to top node
            else if (hasTopNode && CheckParentNode(topNode))
            {
                float originalTop = topNode.position.y;
                topNode.position.y = this.top * this.height;
                this.currentTop = topNode.position.y;
                float topDifference = this.currentTop - originalTop;
                this.part.transform.Translate(0, -topDifference, 0, this.part.transform);
                if (hasBottomNode)
                {
                    float originalBottom = bottomNode.position.y;
                    bottomNode.position.y = this.bottom * this.height;
                    this.currentBottom = bottomNode.position.y;
                    float bottomDifference = this.currentBottom - originalBottom;
                    bottomNode.attachedPart.transform.Translate(0, -(topDifference - bottomDifference)/2, 0, this.part.transform);
                }
            }

            //Surface attached parts
            if (this.part.children.Any(p => p.attachMode == AttachModes.SRF_ATTACH))
            {
                float scaleX = root.localScale.x / originalX;
                float scaleY = root.localScale.y / originalY;
                foreach (Part child in this.part.children)
                {
                    if (child.attachMode == AttachModes.SRF_ATTACH)
                    {
                        // vv  From https://github.com/Biotronic/TweakScale/blob/master/Scale.cs#L403  vv
                        Vector3 vX = (child.transform.localPosition + (child.transform.localRotation * child.srfAttachNode.position)) - this.part.transform.position;

                        Vector3 vY = child.transform.position - this.part.transform.position;
                        child.transform.Translate(vX.x * (scaleX - 1), vY.y * (scaleY - 1), vX.z * (scaleX - 1), this.part.transform);
                    }
                }
            }

            //Node size
            //int nodeSize = Math.Min(this.size, 3);
            int nodeSize = getNodeSize(this.baseDiameter);
            if (hasBottomNode) { bottomNode.size = nodeSize; }
            if (hasTopNode) { topNode.size = nodeSize; }

            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
            else if (HighLogic.LoadedSceneIsFlight)
                StartCoroutine(UpdateDragCube());
        }

        private float GetSize(int id)
        {
            if (id < 0 || id > sizes.Count() - 1)
            {
                Log.Error("Error, size not found: " + id.ToString());
                return 2.5f;
            }
            Log.Info("GetSize, id: " + id + ",   size: " + sizes[id]);
            return sizes[id];
        }

        private int GetID(float size)
        {
            for (int i = 0; i < sizes.Count() - 1; i++)
            {
                if (sizes[i] >= size)
                    return i;
            }
            return sizes.Count() - 1;
            //   ''=> this.sizes.First(p => p.Value == size).Key;
        }

        private IEnumerator<YieldInstruction> UpdateDragCube()
        {
            while (!FlightGlobals.ready || this.part.packed || !this.vessel.loaded)
            {
                yield return new WaitForFixedUpdate();
            }

            this.part.DragCubes.ClearCubes();
            this.part.DragCubes.Cubes.Add(DragCubeSystem.Instance.RenderProceduralDragCube(this.part));
            this.part.DragCubes.ResetCubeWeights();
        }

        bool sizeMassNeedsUpdating = false;
        //int oldSize;
        float oldHeight;
        float oldDiameter;


        private void Window(int id)
        {
            GUI.DragWindow(this.drag);
            GUILayout.BeginVertical();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Localizer.Format("#LOC_NRAP_4"), NRAPUtils.CanParse(this.mass) && NRAPUtils.CheckRange(float.Parse(this.mass), this.minMass, this.maxMass) ? GUI.skin.label : NRAPUtils.RedLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();


            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            this.mass = GUILayout.TextField(this.mass, 10, GUILayout.Width(125));
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            if (GUILayout.Button(Localizer.Format("#LOC_NRAP_5"), GUILayout.Width(60)))
            {
                float m;
                sizeMassNeedsUpdating = true;
                if (float.TryParse(this.mass, out m) && NRAPUtils.CheckRange(m, this.minMass, this.maxMass))
                {
                    this.deltaMass = m - this.part.partInfo.partPrefab.mass;
                    this.currentMass = this.part.TotalMass();
                    //  GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
                }
            }
            GUILayout.EndHorizontal();

            //           #LOC_NRAP_6 = \nTotal mass: {0}t ({1}t dry + {2}t resources)\n

            string formatstr = "\n" + Localizer.Format("LOC_NRAP_6a") + ": {0}t ({1}t " + Localizer.Format("LOC_NRAP_6b") + "{2}t " + Localizer.Format("LOC_NRAP_6c") + ")\n";
            StringBuilder builder = new StringBuilder().AppendFormat(formatstr, this.part.TotalMass().ToString("F2"), this.part.mass.ToString("F2"), this.part.GetResourceMass().ToString("F2"));
            builder.AppendFormat(Localizer.Format("#LOC_NRAP_7") + " {0}", GetModuleCost(0, 0).ToString("F2"));
            builder.AppendFormat(Localizer.Format("#LOC_NRAP_8")+ " {0}", GetModuleDryCost(0, 0).ToString("F2"));
            builder.AppendFormat(Localizer.Format("#LOC_NRAP_9") + " {0}", this.part.TotalCost().ToString("F2"));
            GUILayout.Label(builder.ToString());
            GUILayout.Space(10);


            //   oldSize = this.size;
            oldHeight = this.height;
            oldDiameter = this.baseDiameter;

            //this.baseDiameter = GetSize(this.size);
            snapDiameter = GUILayout.Toggle(snapDiameter, Localizer.Format("#LOC_NRAP_10"));
            GUILayout.Label($"Diameter (m):  {this.baseDiameter}");
            if (snapDiameter)
            {
                this.size = (int)GUILayout.HorizontalSlider(this.size, 0, sizes.Count() - 1);
                this.baseDiameter = GetSize(this.size);
            }
            //this.size = (int)GUILayout.HorizontalSlider(this.size, 0, 4);
            //this.width = GetSize(this.size) / 2.5f; // this.baseDiameter;
            else
            {
                this.baseDiameter = GUILayout.HorizontalSlider(this.baseDiameter, MIN_SIZE, MAX_SIZE);
            }
            this.width = this.baseDiameter / 2.5f;

            GUILayout.Label($"Height multiplier: {this.height.ToString("0.000")}");
            this.height = GUILayout.HorizontalSlider(this.height, this.minHeight, this.maxHeight);
            GUILayout.Space(10);
            //if (oldSize != this.size || oldHeight != this.height)
            if (oldDiameter != this.baseDiameter || oldHeight != this.height)
                sizeMassNeedsUpdating = true;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#LOC_NRAP_11"), GUILayout.Width(150)))
                SetDefaults();

            if (GUILayout.Button(Localizer.Format("#LOC_NRAP_12"), GUILayout.Width(150))) { this.visible = false; }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public float GetModuleDryCost(float defaultCost, ModifierStagingSituation sit)
        {
            //float fuelCost = 0f;
            AvailablePart partInfo = part.partInfo;
            float dryCost = partInfo.cost + part.GetModuleCosts(partInfo.cost, ModifierStagingSituation.CURRENT);
            for (int i = part.Resources.Count - 1; i >= 0; i--)
            {
                PartResource partResource = part.Resources[i];
                PartResourceDefinition info = partResource.info;
                dryCost -= info.unitCost * (float)partResource.maxAmount;
                //fuelCost += info.unitCost * (float)partResource.amount;
            }

            return dryCost;
        }

        public float GetModuleCost(float defaultCost, ModifierStagingSituation sit) => this.part.mass * this.weightCost;

        public ModifierChangeWhen GetModuleCostChangeWhen() => ModifierChangeWhen.FIXED;

        public float GetModuleMass(float defaultMass, ModifierStagingSituation sit) => this.deltaMass;

        public ModifierChangeWhen GetModuleMassChangeWhen() => ModifierChangeWhen.FIXED;
        #endregion

        #region Static methods
        public void CloseOpenedWindow()
        {
            foreach (Part p in EditorLogic.SortedShipList)
            {
                foreach (PartModule pm in p.Modules)
                {
                    ModuleTestWeight tw = pm as ModuleTestWeight;
                    if (tw != null && tw.visible)
                    {
                        tw.visible = false;
                        return;
                    }
                }
            }
        }
        #endregion

        #region Functions
        private bool _firstUpdate = true;
        private void LateUpdate()
        {
            if (HighLogic.LoadedSceneIsEditor && (_firstUpdate || EditorLogic.SortedShipList[0] == this.part || this.part.parent != null))
            {
                _firstUpdate = false;
                if (sizeMassNeedsUpdating)
                {
                    float m;
                    if (float.TryParse(this.mass, out m) && NRAPUtils.CheckRange(m, this.minMass, this.maxMass))
                    {
                        this.deltaMass = m - this.part.partInfo.partPrefab.mass;
                        this.currentMass = this.part.TotalMass();
                        //  GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
                    }
                    UpdateSize();
                    sizeMassNeedsUpdating = false;
                    GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
                    EditorLogic.fetch.SetBackup();
                }

                this.currentMass = this.part.TotalMass();
            }
        }

        private void OnGUI()
        {
            if (HighLogic.LoadedSceneIsEditor && this.visible)
            {
                GUI.skin = HighLogic.Skin;
                this.window = ClickThruBlocker.GUILayoutWindow(this.id, this.window, Window, Localizer.Format("#LOC_NRAP_13") + NRAPUtils.AssemblyVersion);
            }
        }
        #endregion


        void setupdictionary(float value)
        {
            if (NRAPUtils.CheckRange(value, MIN_SIZE - 0.001f, MAX_SIZE))
            {
                Log.Info("Adding : " + value.ToString());
                sizes.Add(sizecnt, value);
                sizecnt++;
            }
        }

        #region Overrides

        void initDictionary()
        {
            if (sizecnt == 0 && HighLogic.LoadedSceneIsEditor)
            {
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size0);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size1);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size2);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size3);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size4);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size5);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size6);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size7);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size8);
                setupdictionary(HighLogic.CurrentGame.Parameters.CustomParams<NRAPCustomParams>().size9);
            }
        }
        public override void OnStart(StartState state)
        {
            Log.InitLog(Localizer.Format("#LOC_NRAP_14"));
            //           part.attachRules.allowRoot = false;
            if ((!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)) { return; }
            if (HighLogic.LoadedSceneIsEditor)
            {

                initDictionary();

                if (!this.initiated)
                {

                    SetDefaults();

                    this.initiated = true;
                    this.mass = this.part.mass.ToString();

                    if (this.baseDiameter < MIN_SIZE || this.baseDiameter > MAX_SIZE)
                    {
                        Debug.LogError("[NRAP]: Invalid base diameter.");
                        if (this.baseDiameter < MIN_SIZE)
                            this.baseDiameter = MIN_SIZE;
                        else
                            this.baseDiameter = MAX_SIZE;

                    }

                    if (this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_1")) != null) { this.top = this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_1")).originalPosition.y; }
                    if (this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_2")) != null) { this.bottom = this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_2")).originalPosition.y; }

                    this.currentTop = this.top;

                    this.currentBottom = this.bottom;
                    if (this.minMass <= 0) { this.minMass = 0.01f; }
                }
                this.window = new Rect(200, 200, 300, 200);
                this.drag = new Rect(0, 0, 300, 30);

                if (this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_1")) != null) { this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_1")).originalPosition.y = this.currentTop; }
                if (this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_2")) != null) { this.part.FindAttachNode(Localizer.Format("#LOC_NRAP_2")).originalPosition.y = this.currentBottom; }


            }
            this.baseHeight = this.part.transform.GetChild(0).localScale.y;
            this.baseRadial = this.part.transform.GetChild(0).localScale.x;
            //this.width = GetSize(this.size) / 2.5f; //  this.baseDiameter;
            this.width = this.baseDiameter / 2.5f;
            this.currentMass = this.part.partInfo.partPrefab.mass + this.deltaMass;
            sizeMassNeedsUpdating = true;
            //   UpdateSize();
           if (HighLogic.LoadedSceneIsFlight) { UpdateSize(); }
        }

        public override string GetInfo()
        {


            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(Localizer.Format("#LOC_NRAP_15")+ ": {0} - {1}t\n", this.maxMass, this.minMass);
            builder.AppendFormat(Localizer.Format("#LOC_NRAP_16") + ": {0} - {1}t\n", this.minHeight, this.maxHeight);
            builder.AppendFormat(Localizer.Format("#LOC_NRAP_17") + ": {0}m\n", this.baseDiameter);
            //builder.Append("Base diameter range: 0.625m, 1.25m, 2.5m, 3.75m, 5m");

            builder.Append(Localizer.Format("#LOC_NRAP_18") + MIN_SIZE.ToString() + Localizer.Format("#LOC_NRAP_19") + MAX_SIZE.ToString() + Localizer.Format("#LOC_NRAP_20"));

            return builder.ToString();
        }
#endregion
    }
}
