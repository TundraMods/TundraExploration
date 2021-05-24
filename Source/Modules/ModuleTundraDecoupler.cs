using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TundraExploration.Modules
{
    public class ModuleTundraDecoupler : ModuleDecouple
    {
        private const string MODULENAME = "ModuleJMOAnimatedDecoupler";

        [KSPField]
        public string animationName;

        [KSPField]
        public float animSpeed = 1.0f;

        [KSPField]
        public bool playAnimationFirst;

        [KSPField(isPersistant = true)]
        private DeployState deployState = DeployState.RETRACTED;

        private enum DeployState
        {
            RETRACTED,
            EXTENDING,
            EXTENDED
        }

        private Animation anim;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (part.stagingIcon == string.Empty && overrideStagingIconIfBlank)
                part.stagingIcon = "DECOUPLER_VERT";

            anim = part.FindModelAnimator(animationName);
            if (anim == null)
            {
                Debug.LogError($"[{MODULENAME}] Could not find animation {animationName} on part {part.name}");
            }
            else
            {
                StartFSM();
            }
        }

        public new void DecoupleAction(KSPActionParam param)
        {
            if (playAnimationFirst)
            {
                PlayAnimation();
            }
            else
            {
                OnDecouple();
                PlayAnimation();
            }
        }

        public new void Decouple()
        {
            if (playAnimationFirst)
            {
                PlayAnimation();
            }
            else
            {
                OnDecouple();
                PlayAnimation();
            }
        }

        public override void OnActive()
        {
            if (staged)
                Decouple();
        }

        private void PlayAnimation()
        {
            if (anim != null)
            {
                anim[animationName].speed = animSpeed;
                anim[animationName].normalizedTime = 0.0f;
                anim[animationName].enabled = true;
                anim.Play(animationName);
                deployState = DeployState.EXTENDING;
            }
        }

        private void StartFSM()
        {
            switch (deployState)
            {
                case DeployState.RETRACTED:
                    anim[animationName].wrapMode = WrapMode.ClampForever;
                    anim[animationName].normalizedTime = 0.0f;
                    anim[animationName].enabled = true;
                    anim[animationName].weight = 1.0f;
                    this.anim.Stop(animationName);
                    break;
                case DeployState.EXTENDED:
                    anim[animationName].wrapMode = WrapMode.ClampForever;
                    anim[animationName].normalizedTime = 1.0f;
                    anim[animationName].enabled = true;
                    anim[animationName].weight = 1.0f;
                    break;
            }
        }

        private void UpdateFSM()
        {
            if (!HighLogic.LoadedSceneIsFlight || deployState != DeployState.EXTENDING) return;

            if (deployState == DeployState.EXTENDING)
            {
                if (anim[animationName].normalizedTime >= 1.0f)
                {
                    anim.Stop(animationName);
                    deployState = DeployState.EXTENDED;

                    if (playAnimationFirst)
                        OnDecouple();
                }
            }
        }

        public void Update()
        {
            UpdateFSM();
        }
    }
}
