using System;

using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using cam;
using remotecam;

namespace monitor
{
    public class cctv : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "Size"),
          UI_FloatRange(minValue = 0.5f, maxValue = 3f, stepIncrement = 0.1f)]
        public float monitorSize = 1f;

        class PartContainer
        {
            public int idx = 0;
            public Part part = null;
            public cccam cam = null;
            public ccrcam wifiCam = null;

            public PartContainer(int i, Part p, cccam c)
            {
                idx = i;
                part = p;
                cam = c;
            }
            public PartContainer(int i, Part p, ccrcam c)
            {
                idx = i;
                part = p;
                wifiCam = c;
            }
            public string pcIndex { get { return part.name + idx.ToString(); } }
        }
        // private List<Part> myCams = new List<Part>();

        private Dictionary<string, PartContainer> myCams = new Dictionary<string, PartContainer>();

        private Texture blankScreen = new Texture2D(2,2);

        public int selectedCam = 0;

        public bool isOn = false;

        private bool isInitialised = false;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            GameScenes scene = HighLogic.LoadedScene;
            if (scene == GameScenes.FLIGHT)
                blankScreen = gameObject.GetChild("Screen").GetComponent<Renderer>().material.GetTexture("_MainTex"); ;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();

            GameScenes scene = HighLogic.LoadedScene;
            if (scene == GameScenes.FLIGHT)
            {
                if (!isInitialised && !this.vessel.packed)
                {
                    Detect();
                    Switch();
                    isInitialised = true;
                }

                if (myCams.Count > 0)
                {
                    if (myCams.Values.ElementAt(selectedCam).cam)
                    {
                        Events["toggleLock"].guiName = ((cccam)myCams.Values.ElementAt(selectedCam).part.Modules["cccam"]).Events["toggleLock"].guiName;
                    }
                    else if (myCams.Values.ElementAt(selectedCam).wifiCam)
                    {
                        Events["toggleLock"].guiName = ((ccrcam)myCams.Values.ElementAt(selectedCam).part.Modules["ccrcam"]).Events["toggleLock"].guiName;
                    }
                }

                this.gameObject.transform.localScale = new Vector3(monitorSize, monitorSize, monitorSize);
            }
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Next Cam")]
        public void nextCamEvent()
        {
            if (myCams.Count > 0)
            {
                if (isOn)
                {
                    selectedCam++;

                    if (selectedCam > myCams.Count - 1)
                        selectedCam = 0;

                    if (myCams.Values.ElementAt(selectedCam).cam)
                    {
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", ((cccam)myCams.Values.ElementAt(selectedCam).cam).camTex);
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", ((cccam)myCams.Values.ElementAt(selectedCam).cam).camTex);
                    }
                    else if (myCams.Values.ElementAt(selectedCam).wifiCam)
                    {
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", ((cccam)myCams.Values.ElementAt(selectedCam).wifiCam).camTex);
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", ((cccam)myCams.Values.ElementAt(selectedCam).wifiCam).camTex);
                    }
                }
            }
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Prev Cam")]
        public void prevCamEvent()
        {
            if (myCams.Count > 0)
            {
                if (isOn)
                {
                    selectedCam--;

                    if (selectedCam < 0)
                        selectedCam = myCams.Count - 1;

                    if (myCams.Values.ElementAt(selectedCam).cam)
                    {
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", ((cccam)myCams.Values.ElementAt(selectedCam).cam).camTex);
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", ((cccam)myCams.Values.ElementAt(selectedCam).cam).camTex);
                    }
                    else if (myCams.Values.ElementAt(selectedCam).wifiCam)
                    {
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", ((cccam)myCams.Values.ElementAt(selectedCam).wifiCam).camTex);
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", ((cccam)myCams.Values.ElementAt(selectedCam).wifiCam).camTex);
                    }
                }
            }
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Toggle monitor")]
        public void Switch()
        {
            if (myCams.Count > 0)
            {
                isOn = !isOn;

                if (isOn)
                {
                    if (myCams.Values.ElementAt(selectedCam).cam != null)
                    {
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", ((cccam)myCams.Values.ElementAt(selectedCam).cam).camTex);
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", ((cccam)myCams.Values.ElementAt(selectedCam).cam).camTex);
                    }
                    else if (myCams.Values.ElementAt(selectedCam).wifiCam)
                    {
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", ((cccam)myCams.Values.ElementAt(selectedCam).wifiCam).camTex);
                        gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", ((cccam)myCams.Values.ElementAt(selectedCam).wifiCam).camTex);
                    }
                    gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.gray);
                }
                else
                {
                    gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", blankScreen);
                    this.gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", blankScreen);
                    this.gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
                }
            }
            else
            {
                this.gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_MainTex", blankScreen);
                this.gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetTexture("_Emissive", blankScreen);
                this.gameObject.GetChild("Screen").GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
            }
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Detect cameras")]
        public void Detect()
        {
            foreach (Part myCam in this.vessel.parts)
            {
                int cnt = 0;
                
                if (myCam.Modules != null)
                {
                    for (int i = 0; i < myCam.Modules.Count; i++)
                    {
                        if (myCam.Modules[i] is cccam)
                        {
                            cccam myCamMod = (cccam)myCam.Modules[i];
                            
                            if (!myCams.ContainsKey(myCam.name + cnt.ToString()))
                            {
                                myCamMod.switchOn();
                                myCamMod.isOn = true;
                                myCamMod.Events["toggleOnOff"].guiName = "Switch Off";
                                PartContainer pc = new PartContainer(cnt, myCam, myCamMod);

                                myCams.Add(pc.pcIndex, pc);
                            }
                            cnt++;
                        }
                    }
                }
                else
                    Debug.Log("Modules is null");
            }

            foreach (Vessel ship in FlightGlobals.Vessels)
            {
                if (ship.loaded)
                {
                    foreach (Part myCam in ship.parts)
                    {
                        int cnt = 0;

                        if (myCam.Modules != null)
                        {
                            for (int i = 0; i < myCam.Modules.Count; i++)
                            {
                                if (myCam.Modules[i] is ccrcam)
                                {
                                    ccrcam myRemoteCamMod = (ccrcam)myCam.Modules[i];
                                  
                                    if (!myCams.ContainsKey(myCam.name + cnt.ToString()))
                                    {
                                        myRemoteCamMod.switchOn();
                                        myRemoteCamMod.isOn = true;
                                        myRemoteCamMod.Events["toggleOnOff"].guiName = "Switch Off";
                                        PartContainer pc = new PartContainer(cnt, myCam, myRemoteCamMod);

                                        myCams.Add(pc.pcIndex, pc);
                                    }
                                    cnt++;
                                }
                            }
                        }
                        else
                            Debug.Log("Modules is null");

                    }
                }
            }

        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Unlock camera")]
        public void toggleLock()
        {
            if (myCams.Count > 0)
            {
                if (myCams.Values.ElementAt(selectedCam).part.Modules.Contains("cccam"))
                    ((cccam)myCams.Values.ElementAt(selectedCam).part.Modules["cccam"]).toggleLock();
                else if (myCams.Values.ElementAt(selectedCam).part.Modules.Contains("ccrcam"))
                    ((ccrcam)myCams.Values.ElementAt(selectedCam).part.Modules["ccrcam"]).toggleLock();
            }
        }
    }
}
