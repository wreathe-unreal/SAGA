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
    public class LinearFade : AFixedDurationTextAnimation
    {
        private readonly byte m_iBeginAlpha;
        private readonly byte m_iEndAlpha;

        public LinearFade(TMP_Text rTextMesh, string sText, 
                float fTotDuration, byte iBeginAlpha, byte iEndAlpha) 
                : base(rTextMesh, sText, fTotDuration) {
            m_iBeginAlpha = iBeginAlpha;
            m_iEndAlpha = iEndAlpha;
        }

        protected override void updateVisualization() {
            m_rTextMesh.setAlphaBegin();

            float f01Progress = 
                Mathf.InverseLerp(0, m_fTotDuration, m_fAccumulatedDelta);
            byte iAlpha =
                (byte)Mathf.Lerp(m_iBeginAlpha, m_iEndAlpha, f01Progress);
            m_rTextMesh.setTextAlpha(iAlpha);

            m_rTextMesh.setAlphaEnd();
        }
    }
}