using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using cam;

namespace remotecam
{
    public class ccrcam : cccam
    {
        private float red = 1f;
        private bool redSwitch = false;

        public bool isDetected = false;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            gameObject.GetChild("antennaball").GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.black);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            GameScenes scene = HighLogic.LoadedScene;
            if (scene == GameScenes.FLIGHT)
            {
                if (isOn && isDetected)
                {
                    if (redSwitch)
                    {
                        red -= 0.05f;
                        if (red <= 0)
                            redSwitch = false;
                    }
                    else
                    {
                        red += 0.05f;
                        if (red >= 1)
                            redSwitch = true;
                    }

                    gameObject.GetChild("antennaball").GetComponent<Renderer>().material.SetColor("_EmissiveColor", Color.red);
                }
            }
        }
    }
}
