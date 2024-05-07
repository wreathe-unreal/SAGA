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

using BinaryCharm.TextMeshProAlpha;

namespace BinaryCharm.Samples.TextMeshProAlpha.TextAnimation
{
    public abstract class ATextAnimation : IAnimation
    {
        protected TMP_Text m_rTextMesh;
        protected string m_sText;

        public ATextAnimation(TMP_Text rTextMesh, string sText) {
            m_rTextMesh = rTextMesh;
            m_sText = sText;
        }

        public void start() {
            m_rTextMesh.setAlphaTextDontRender(m_sText);

            onStart();
            updateVisualization();

            m_rTextMesh.setAlphaTextRender();
        }

        public void update(float fDT) {
            onUpdate(fDT);
            updateVisualization();
        }

        public abstract bool isComplete();

        protected abstract void onStart();
        protected abstract void onUpdate(float fDT);
        protected abstract void updateVisualization();
    }
}