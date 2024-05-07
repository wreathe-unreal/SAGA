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

using TMPro;

namespace BinaryCharm.Samples.TextMeshProAlpha.TextAnimation
{
    public abstract class AFixedDurationTextAnimation : ATextAnimation
    {
        protected readonly float m_fTotDuration;
        protected float m_fAccumulatedDelta;

        public AFixedDurationTextAnimation(TMP_Text rTextMesh, string sText,
                float fTotDuration) : base(rTextMesh, sText) {

            m_fTotDuration = fTotDuration;
        }

        public override bool isComplete() {
            return m_fAccumulatedDelta > m_fTotDuration;
        }

        protected override void onStart() {
            m_fAccumulatedDelta = 0f;
            updateVisualization();
        }

        protected override void onUpdate(float fDT) {
            m_fAccumulatedDelta += fDT;
            updateVisualization();
        }
    }
}