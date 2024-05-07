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
    public class WholeTextClockwiseFade : AFixedDurationTextAnimation
    {
        private readonly byte m_iBeginAlpha;
        private readonly byte m_iEndAlpha;
        private readonly float m_fEachCornerDelay;

        private const int iCORNERS = 4;
        private readonly float[] m_rCornersDelay = new float[iCORNERS];
        private readonly byte[] m_rCornersAlpha = new byte[iCORNERS];

        public WholeTextClockwiseFade(TMP_Text rTextMesh, string sText,
                float fTotDuration,
                byte iBeginAlpha,
                byte iEndAlpha,
                float fEachCornerDelay,
                int iStartCorner = 0 // [0-3]
                ) : base(rTextMesh, sText, fTotDuration) {

            m_iBeginAlpha = iBeginAlpha;
            m_iEndAlpha = iEndAlpha;
            m_fEachCornerDelay = fEachCornerDelay;
            
            for (int i = 0; i < iCORNERS; ++i) {
                int iCornerId = (i + iStartCorner) % iCORNERS;
                m_rCornersDelay[iCornerId] = i * m_fEachCornerDelay;
            }
        }

        public override bool isComplete() {
            return m_fAccumulatedDelta > 
                   m_fTotDuration + (iCORNERS - 1)*m_fEachCornerDelay;
        }

        protected override void updateVisualization() {
            for (int i = 0; i < iCORNERS; ++i) {
                float fTime = m_fAccumulatedDelta - m_rCornersDelay[i];
                float f01Progress =
                    Mathf.InverseLerp(0f, m_fTotDuration, fTime);
                m_rCornersAlpha[i] =
                    (byte)Mathf.Lerp(m_iBeginAlpha, m_iEndAlpha, f01Progress);
            }
            m_rTextMesh.setAlphaBegin();
            m_rTextMesh.setTextAlphaCornersFade(m_rCornersAlpha);
            m_rTextMesh.setAlphaEnd();
        }
    }
}