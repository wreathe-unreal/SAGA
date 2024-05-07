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
    public class VerticalRandomFade : AFixedDurationTextAnimation
    {
        private readonly byte m_iBeginAlpha;
        private readonly byte m_iEndAlpha;
        private readonly float[] m_rDelays;
        private readonly float m_fMaxRandomDelay;

        public VerticalRandomFade(TMP_Text rTextMesh, string sText, 
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

            TMP_TextInfo rTI = m_rTextMesh.textInfo;

            for (int iCharIdx = 0; iCharIdx < m_sText.Length; ++iCharIdx) {
                float fDelay = m_rDelays[iCharIdx];
                float fTime = m_fAccumulatedDelta - fDelay;
                float f01Progress = 
                    Mathf.InverseLerp(0f, m_fTotDuration, fTime);

                bool bEvenLine =
                    rTI.characterInfo[iCharIdx].lineNumber % 2 == 0;

                if (bEvenLine) {
                    m_rTextMesh.setCharAlphaFadeVertEx(iCharIdx,
                        1f - f01Progress, 0.5f, m_iBeginAlpha, m_iEndAlpha);
                } else {
                    m_rTextMesh.setCharAlphaFadeVertEx(iCharIdx,
                        f01Progress, 0.5f, m_iEndAlpha, m_iBeginAlpha);
                }
            }
            m_rTextMesh.setAlphaEnd();
        }
    }
}