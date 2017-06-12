using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace cam
{
    public class cccam : PartModule
    {
        public RenderTexture camTex;
        public Camera myCam;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "FoV"),
          UI_FloatRange(minValue = 45.0f, maxValue = 90.0f, stepIncrement = 1.0f)]
        public float camFoV = 60.0f;

        [KSPField(isPersistant = false)]
        int speed = 30;

        [KSPField(isPersistant = false)]
        public string cameraBody = "cambody";

        [KSPField(isPersistant = false)]
        public string cameraDir = "camdir";

        [KSPField]
        public string cameraTransformName = "";

        [KSPField(isPersistant = false)]
        public Vector3 cameraForward = Vector3.zero;
        [KSPField(isPersistant = false)]
        public Vector3 cameraUp = Vector3.up;
        [KSPField(isPersistant = false)]
        public Vector3 cameraPosition = Vector3.zero;
        [KSPField]
        public float cameraFoVMax = 90;
        [KSPField]
        public float cameraFoVMin = 45;



#if false
        private void UpdateLabels()
        {
            if (Events != null)
            {
                Debug.Log("UpdateLabels 1");
                //if (Events["toggleCameraForwardX"] != null)
                    Events["toggleCameraForwardX"].guiName = "cameraForward.x:" + cameraForward.x.ToString();
                //if (Events["toggleCameraForwardY"] != null)
                    Events["toggleCameraForwardY"].guiName = "cameraForward.y:" + cameraForward.y.ToString();
                //if (Events["toggleCameraForwardZ"] != null)
                    Events["toggleCameraForwardZ"].guiName = "cameraForward.z:" + cameraForward.z.ToString();
                //if (Events["togglecameraUpX"] != null)
                    Events["togglecameraUpX"].guiName = "cameraUp.x:" + cameraUp.x.ToString();
                //if (Events["togglecameraUpY"] != null)
                    Events["togglecameraUpY"].guiName = "cameraUp.y:" + cameraUp.y.ToString();
                //if (Events["togglecameraUpZ"] != null)
                    Events["togglecameraUpZ"].guiName = "cameraUp.z:" + cameraUp.z.ToString();
            }
        } 
        [KSPEvent(name = "cameraForward.x", guiActive = true)]
        public void toggleCameraForwardX()
        {
            cameraForward.x++;
            if (cameraForward.x > 1)
                cameraForward.x = -1;
            UpdateLabels();
        }
        [KSPEvent(name = "cameraForward.y", guiActive = true)]
        public void toggleCameraForwardY()
        {
            cameraForward.y++;
            if (cameraForward.y > 1)
                cameraForward.y = -1;
            UpdateLabels();
        }
        [KSPEvent(name = "cameraForward.z", guiActive = true)]
        public void toggleCameraForwardZ()
        {
            cameraForward.z++;
            if (cameraForward.z > 1)
                cameraForward.z = -1;
            UpdateLabels();
        }


        [KSPEvent(name = "cameraUp.x", guiActive = true)]
        public void togglecameraUpX()
        {
            cameraUp.x++;
            if (cameraUp.x > 1)
                cameraUp.x = -1;
            UpdateLabels();
        }
        [KSPEvent(name = "cameraUp.y", guiActive = true)]
        public void togglecameraUpY()
        {
            cameraUp.y++;
            if (cameraUp.y > 1)
                cameraUp.y = -1;
            UpdateLabels();
        }
        [KSPEvent(name = "cameraUp.z", guiActive = true)]
        public void togglecameraUpZ()
        {
            cameraUp.z++;
            if (cameraUp.z > 1)
                cameraUp.z = -1;
            UpdateLabels();
        }
#endif

        int tempSpeed = 0;

        public bool isOn = false;
        public bool isLocked = true;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            GameScenes scene = HighLogic.LoadedScene;
            if (scene == GameScenes.FLIGHT)
            {
                camTex = new RenderTexture(1024, 768, 1);
                camTex.isPowerOfTwo = true;
                camTex.anisoLevel = 4;
                camTex.antiAliasing = 4;
                camTex.filterMode = FilterMode.Trilinear;
                camTex.Create();

                while (!camTex.IsCreated()) ;

               

                if (cameraForward != Vector3.zero)
                {
                    var t = new GameObject();
                    t.transform.SetParent((cameraTransformName.Length > 0) ? part.FindModelTransform(cameraTransformName) : part.transform);
                    
                    myCam = t.AddComponent<Camera>();

                    isOn = false;
                    Events["toggleOnOff"].active = false;

                    var camFoVRange = Fields["camFoV"].uiControlEditor as UI_FloatRange;
                    camFoVRange.maxValue = cameraFoVMax;
                    camFoVRange.minValue = cameraFoVMin;


                    myCam.transform.localPosition = cameraPosition;

                    myCam.transform.localRotation = Quaternion.LookRotation(cameraForward, cameraUp);
                    
                    myCam.Render();
                }
                else
                    myCam = (Camera)this.gameObject.GetChild(cameraDir).AddComponent<Camera>();

                myCam.nearClipPlane = 0.001f;
                myCam.farClipPlane = 1000000000f;
                myCam.fieldOfView = camFoV;
                myCam.cullingMask = (1 << 0) | (1 << 1) | (1 << 4) | (1 << 9) | (1 << 10) | (1 << 15) | (1 << 18) | (1 << 20) | (1 << 23);
                
                myCam.targetTexture = camTex;
                
                myCam.enabled = false;
#if false
                UpdateLabels();
#endif
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            GameScenes scene = HighLogic.LoadedScene;
            if (scene == GameScenes.FLIGHT)
            {                
                myCam.fieldOfView = camFoV;

                if (cameraForward != Vector3.zero)
                {
                    myCam.transform.localRotation = Quaternion.LookRotation(cameraForward, cameraUp);
                }
                else
                {
                    if (isOn && !isLocked)
                    {
                        tempSpeed = speed;

                        if (Vector3.Angle(this.gameObject.transform.forward, this.gameObject.GetChild(cameraBody).transform.forward) >= 30)
                        {
                            tempSpeed *= -2;
                            speed *= -1;
                        }

                        this.gameObject.GetChild(cameraBody).transform.Rotate(Vector3.up, Time.deltaTime * tempSpeed);

                        Events["toggleLock"].active = false;

                    }


                    if (!isLocked)
                        Events["toggleLock"].guiName = "Lock cameras";
                    else
                    {
                        Events["toggleLock"].guiName = "Unlock cameras";
                        resetRot();
                    }
                }
            }
        }

        public void resetRot()
        {
            if (cameraForward == Vector3.zero)
                this.gameObject.GetChild(cameraBody).transform.rotation = this.gameObject.transform.rotation;
        }

        public void switchOff()
        {
            var green = gameObject.GetChild("green");
            var red = gameObject.GetChild("red");
            if (green != null)
                gameObject.GetChild("green").GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
            if (red != null)
                gameObject.GetChild("red").GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.red);

            this.myCam.enabled = false;
            if (cameraForward == Vector3.zero)
                this.gameObject.GetChild(cameraBody).transform.rotation = this.gameObject.transform.rotation;
        }

        public void switchOn()
        {
            var green = gameObject.GetChild("green");
            var red = gameObject.GetChild("red");
            if (green != null)
                green.GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.green);
            if (red != null)
                red.GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
            
            this.myCam.enabled = true;
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Unlock cameras")]
        public void toggleLock()
        {
            isLocked = !isLocked;
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Switch On")]
        public void toggleOnOff()
        {
            isOn = !isOn;

            if (isOn)
            {
                switchOn();
                Events["toggleOnOff"].guiName = "Switch Off";
            }
            else
            {
                switchOff();
                Events["toggleOnOff"].guiName = "Switch On";
            }
        }
    }
}
