using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaSprite.SinusoidAnimator
{
    public class SinusoidAnimator : MonoBehaviour
    {
        public bool ScaledTime = true;
        float _deltaTime => ScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;

        #region Sine Classes
        [System.Serializable]
        public class SinusoidVector
        {
            public Vector3 Vector;
            public Sinusoid Sinusoid;

            public Vector3 GetValue(float t)
            {
                return Sinusoid.GetValue(t) * Vector.normalized;
            }
        }

        [System.Serializable]
        public class Sinusoid
        {
            public float Amplitude = 1f;
            public float Frequency = 1f;
            public float Phase = 0f;
            public bool RandomizePhase;
            bool _hasPhaseBeenRandomized = false;

            public float GetValue(float t)
            {
                if (!_hasPhaseBeenRandomized && RandomizePhase) DoRandomizePhase();

                return Amplitude * Mathf.Sin(t * Frequency + Phase);
            }

            void DoRandomizePhase()
            {
                Phase = Random.Range(0f, 360f);
                _hasPhaseBeenRandomized = true;
            }
        }

        [System.Serializable]
        public class SineVectorConfig
        {
            public float Amp = 0f;
            public float Freq = 0f;
            public List<SinusoidVector> Sines = new List<SinusoidVector>();
        }
        #endregion

        #region Public Variables
        public float Amp = 1f;
        public float Freq = 1f;
        [Header("--------------------------------------------------------------------------------------------------------------------------------")]
        [Space]
        public SineVectorConfig Position;
        [Header("--------------------------------------------------------------------------------------------------------------------------------")]
        [Space]
        public SineVectorConfig Rotation;
        [Header("--------------------------------------------------------------------------------------------------------------------------------")]
        [Space]
        public SineVectorConfig Scale;

        #endregion

        #region Private Variables
        float _t;
        List<float> timeList = new List<float>();
        Vector3 _initPos;
        Vector3 _initRot;
        Vector3 _initScale;
        #endregion

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            //Init();
        }

        void Init()
        {
            _initPos = transform.localPosition;
            _initRot = transform.localRotation.eulerAngles;
            _initScale = transform.localScale;
        }

        private void Update()
        {
            _t += _deltaTime * Freq;

            // Position
            Vector3 targetPos = _initPos + GetValue(Position);
            transform.localPosition = targetPos;

            // Rotation
            if (Rotation.Sines.Count > 0)
            {
                Vector3 targetRot = _initRot + GetValue(Rotation);
                transform.localRotation = Quaternion.Euler(targetRot);
            }

            // Scale
            if (Scale.Sines.Count > 0)
            {
                Vector3 targetScale = _initScale + GetValue(Scale);
                transform.localScale = targetScale;
            }
        }

        public Vector3 GetValue(SineVectorConfig sineVectorConfig)
        {
            Vector3 value = new Vector3();
            for(int i = 0; i < sineVectorConfig.Sines.Count; i++)
            {
                SinusoidVector x = sineVectorConfig.Sines[i];
                value += Amp * sineVectorConfig.Amp * x.GetValue(GetAnimTime(i, sineVectorConfig));
            }
            /*
            sineVectorConfig.Sines.ForEach(x =>
            {
                value += Amp * sineVectorConfig.Amp * x.GetValue(_t * sineVectorConfig.Freq);
            });
            */
            return value;
        }

        public float GetAnimTime(int index, SineVectorConfig sineVectorConfig)
        {
            float t = _t;
            if (index >= timeList.Count) timeList.Add(0f);
            timeList[index] += _deltaTime * sineVectorConfig.Freq * Freq;
            return t + timeList[index];
        }

        /*
        public void Pulse(float time = 0.25f, float startAmp = 0, float endAmp = 1, float startFreq = 0, float endFreq = 1)
        {
            Amp = startAmp;
            Freq = startFreq;
            float timer = 0;
            ExtensionMethods.RepeatingPredicateInvoke(this, 
                () => timer < time,
                () =>
                {
                    timer += _deltaTime;
                    if (timer < time * 0.5f)
                    {
                        Amp = ExtensionMethods.Remap(timer, 0, time*0.5f, startAmp, endAmp);
                        Freq = ExtensionMethods.Remap(timer, 0, time*0.5f, startFreq, endFreq);
                    }
                    else
                    {
                        Amp = ExtensionMethods.Remap(timer, time * 0.5f, time, endAmp, startAmp);
                        Freq = ExtensionMethods.Remap(timer, time * 0.5f, time, endFreq, startFreq);
                    }
                },
                () =>
                {
                    
                }
            );
        }
        */
    }
}
