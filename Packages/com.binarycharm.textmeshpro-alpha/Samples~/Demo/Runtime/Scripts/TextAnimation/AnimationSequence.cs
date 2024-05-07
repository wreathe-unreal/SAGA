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

namespace BinaryCharm.Samples.TextMeshProAlpha.TextAnimation
{
    public class AnimationSequence : IAnimation
    {
        private readonly IAnimation[] m_rAnimations;
        private readonly float m_fPauseBetweenAnimsSecs;

        private int m_iCurrAnimation;
        private float m_fAccumulatedPauseSecs;

        public AnimationSequence(IAnimation[] rAnimations,
                float fPauseBetweenAnimsSecs = 0.0f) {
            m_rAnimations = rAnimations;
            m_fPauseBetweenAnimsSecs = fPauseBetweenAnimsSecs;
        }

        public void start() {
            m_iCurrAnimation = 0;
            m_rAnimations[m_iCurrAnimation].start();
            m_fAccumulatedPauseSecs = 0f;
        }

        public bool isComplete() {
            return m_iCurrAnimation == m_rAnimations.Length - 1 &&
                m_rAnimations[m_iCurrAnimation].isComplete();
        }

        public void update(float fDT) {
            if (m_rAnimations[m_iCurrAnimation].isComplete()) {
                m_fAccumulatedPauseSecs += fDT;
                if (m_fAccumulatedPauseSecs >= m_fPauseBetweenAnimsSecs) {
                    if (m_iCurrAnimation < m_rAnimations.Length - 1) {
                        ++m_iCurrAnimation;
                        m_rAnimations[m_iCurrAnimation].start();

                        fDT = // remaining delta for new anim update
                            m_fAccumulatedPauseSecs - m_fPauseBetweenAnimsSecs;
                        m_fAccumulatedPauseSecs = 0f;
                    }
                }
            }
            m_rAnimations[m_iCurrAnimation].update(fDT);
        }
    }
}