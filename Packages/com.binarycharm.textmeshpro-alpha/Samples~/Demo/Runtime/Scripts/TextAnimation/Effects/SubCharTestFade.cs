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
    public class SubCharTestFade : AFixedDurationTextAnimation
    {
        private const byte iMIN_ALPHA = 0;
        private const byte iMAX_ALPHA = 255;
        private readonly float m_fStepDuration;

        public SubCharTestFade(TMP_Text rTextMesh, string sText, 
                float fTotDuration) : base(rTextMesh, sText, fTotDuration) {
            m_fStepDuration = m_fTotDuration / 4f;
        }

        protected override void updateVisualization() {
            int iStep = Mathf.FloorToInt(m_fAccumulatedDelta / m_fStepDuration);
            float fStepTime = m_fAccumulatedDelta - (iStep * m_fStepDuration);
            float fFracPart = Mathf.InverseLerp(0f, m_fStepDuration, fStepTime);

            if (iStep % 2 == 1) {
                fFracPart = 1f - fFracPart;
            }

            m_rTextMesh.setAlphaBegin();
            if (iStep < 2) { // horiz 
                for (int iCharIdx = 0; iCharIdx < m_sText.Length; ++iCharIdx) {
                    m_rTextMesh.setCharAlphaFadeHorizEx(iCharIdx,
                        fFracPart, 0.5f, iMIN_ALPHA, iMAX_ALPHA);
                }
            } else { // vert
                for (int iCharIdx = 0; iCharIdx < m_sText.Length; ++iCharIdx) {
                    m_rTextMesh.setCharAlphaFadeVertEx(iCharIdx,
                        fFracPart, 0.5f, iMIN_ALPHA, iMAX_ALPHA);
                }
            }
            m_rTextMesh.setAlphaEnd();
        }
    }
}