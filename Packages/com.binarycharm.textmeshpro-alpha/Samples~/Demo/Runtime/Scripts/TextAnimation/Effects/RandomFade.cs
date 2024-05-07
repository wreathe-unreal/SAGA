/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2020 Binary Charm - All Rights Reserved
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@                  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@                        @@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@                           @@@@@@@@@@@@@@@@@@
             @@@@@@@@@   @@@@@@@@@@@  @@@@@        @@@@@@@@@@@@@@@
             @@@@@@@@@@@  @@@@@@@@@  @@@@@@@@@@       (@@@@@@@@@@@
             @@@@@@@@@@@@  @@@@@@@@& @@@@@@@@@@ @@@@     @@@@@@@@@
             @@@@@@@@@@@@@ @@@@@@@@@@ *@@@@@@@ @@@@@@@@@*   @@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@      @@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
*/

using UnityEngine;

using TMPro;

using BinaryCharm.TextMeshProAlpha;

namespace BinaryCharm.Samples.TextMeshProAlpha.TextAnimation.Effects
{
    public class RandomFade : AFixedDurationTextAnimation
    {
        private readonly byte m_iBeginAlpha;
        private readonly byte m_iEndAlpha;
        private readonly float[] m_rDelays;
        private readonly float m_fMaxRandomDelay;

        public RandomFade(TMP_Text rTextMesh, string sText, 
                float fTotDuration,
                byte iBeginAlpha,
                byte iEndAlpha,
                float fMaxDelay = 0.4f
                ) : base(rTextMesh, sText, fTotDuration) {
            m_iBeginAlpha = iBeginAlpha;
            m_iEndAlpha = iEndAlpha;
            m_rDelays = new float[sText.Length];
            m_fMaxRandomDelay = 0f;
            for (int i = 0; i < sText.Length; ++i) {
                float fRandomDelay = Random.Range(0f, fMaxDelay);
                m_rDelays[i] = fRandomDelay;
                if (fRandomDelay > m_fMaxRandomDelay) {
                    m_fMaxRandomDelay = fRandomDelay;
                }
            }
        }

        public override bool isComplete() {
            return m_fAccumulatedDelta > m_fTotDuration + m_fMaxRandomDelay;
        }

        protected override void updateVisualization() {
            m_rTextMesh.setAlphaBegin();
            for (int i = 0; i < m_sText.Length; ++i) {
                float fDelay = m_rDelays[i];
                float fTime = m_fAccumulatedDelta - fDelay;
                float f01Progress =
                    Mathf.InverseLerp(0f, m_fTotDuration, fTime);
                byte iAlpha =
                    (byte)Mathf.Lerp(m_iBeginAlpha, m_iEndAlpha, f01Progress);

                // single alpha value for all four corners of the character:
                // m_rTextMesh.setCharAlpha(i, iAlpha); 

                // different alpha for each of the four corners
                // maps 0 -> 1 to 0 -> 1 -> 0 with 1 at half anim
                float f01MidProgress = 1f - Mathf.Abs((f01Progress - 0.5f) * 2f);
                m_rTextMesh.setCharAlphaCornersFade(i,
                    iAlpha,
                    (byte)Mathf.Clamp(iAlpha - f01MidProgress * 50, 0, 255),
                    (byte)Mathf.Clamp(iAlpha - f01MidProgress * 100, 0, 255),
                    (byte)Mathf.Clamp(iAlpha - f01MidProgress * 75, 0, 255)
                );
            }
            m_rTextMesh.setAlphaEnd();
        }
    }
}