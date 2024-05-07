/*
             @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
             Copyright (C) 2022 Binary Charm - All Rights Reserved
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

namespace BinaryCharm.Samples.TextMeshProAlpha.TextAnimation {

    public class AnimationSet : IAnimation {

        private readonly IAnimation[] m_rAnimations;

        public AnimationSet(IAnimation[] rAnimations) {
            m_rAnimations = rAnimations;
        }

        public void start() {
            foreach (IAnimation rAnim in m_rAnimations) {
                rAnim.start();
            }
        }

        public bool isComplete() {
            foreach (IAnimation rAnim in m_rAnimations) {
                if (!rAnim.isComplete()) return false;
            }
            return true;
        }

        public void update(float fDT) {
            foreach (IAnimation rAnim in m_rAnimations) rAnim.update(fDT);
        }

    }

}
