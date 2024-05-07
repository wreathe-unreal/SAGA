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

namespace BinaryCharm.TextMeshProAlpha
{
    /// <summary>
    /// `TMP_Text` extension methods that allow to easily implement
    /// sophisticated text fading effects.
    /// </summary>
    public static class TMP_Text_AlphaExtensions 
    {
        ///////////////////////////////////////////////////////////////////////
        #region PUBLIC_UTILS

        /// <summary>
        /// `ForceMeshUpdate()` wrapper, to be called before any method doing
        /// alpha change operations.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        public static void setAlphaBegin(this TMP_Text rText) {
            rText.ForceMeshUpdate();
        }

        /// <summary>
        /// `UpdateVertexData()` wrapper, to be called at the end of a series
        /// of alpha change operations.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        public static void setAlphaEnd(this TMP_Text rText) {
            rText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        /// <summary>
        /// Utility method to automatically wrap alpha change modifications
        /// between the needed calls. Looks neat, but generates garbage.
        /// Prefer calling manually `setAlphaBegin()`/`setAlphaEnd()`.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="rOperations">Array of `Actions` to execute between the
        /// `setAlphaBegin()` and `setAlphaEnd()` calls.</param>
        public static void setAlphaExecOps(this TMP_Text rText,
                System.Action[] rOperations) {
            rText.setAlphaBegin();
            foreach (System.Action rOp in rOperations) {
                rOp();
            }
            rText.setAlphaEnd();
        }

        /// <summary>
        /// Utilty method to setup text preventing it to be rendered.
        /// Needed to avoid a glitch where after setting a text it gets 
        /// rendered at full alpha for one frame even if you set it differently
        /// using `setCharAlpha*()`/`setTextAlpha*()` methods.
        /// </summary>
        /// <remark>
        /// WARNING: you might want to restore the rendermode if you later set
        /// non-alpha fading texts on the same `TMP_Text`.
        /// </remark>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="sText">Text to be assigned.</param>
        public static void setAlphaTextDontRender(this TMP_Text rText, 
                string sText) {
            rText.renderMode = TextRenderFlags.DontRender;
            rText.text = sText;
        }

        /// <summary>
        /// Utility method to complete the setup of a text mesh, after 
        /// "preparing" it (with `setAlphaTextDontRender()` and some 
        /// `setCharAlpha*()`/`setTextAlpha*()` calls).
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        public static void setAlphaTextRender(this TMP_Text rText) {
            Mesh rMesh = rText.mesh;
            TMP_TextInfo rTI = rText.textInfo;
            rMesh.vertices = rTI.meshInfo[0].vertices;
            rMesh.uv = rTI.meshInfo[0].uvs0;
            rMesh.uv2 = rTI.meshInfo[0].uvs2;
            rMesh.colors32 = rTI.meshInfo[0].colors32;
            rMesh.RecalculateBounds();
        }

        /// <summary>
        /// Utility method for the common case where in the text setup frame
        /// you simply want to set an uniform alpha (e.g.: 0, fully invisible).
        /// In this case you can simply call this method and avoid
        /// `setAlphaTextDontRender()`/`setAlphaTextRender()`.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="sText">Text to be assigned.</param>
        /// <param name="iAlpha">Alpha level to set. The value passed must be
        /// in the [0, 255] range.</param>
        public static void setAlphaText(this TMP_Text rText,
                string sText, byte iAlpha) {
            rText.setAlphaTextDontRender(sText);

            rText.setAlphaBegin();
            rText.setTextAlpha(iAlpha);
            rText.setAlphaEnd();

            rText.setAlphaTextRender();
        }

        /// <summary>
        /// Utility method to automatically wrap the text setup operations 
        /// between the needed calls. Looks neat, but generates garbage.
        /// Prefer using `setAlphaText()` (if suits your needs) or call 
        /// `setAlphaTextDontRender()`/`setAlphaTextRender()` manually.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="sText">Text to be assigned.</param>
        /// <param name="rOperations">Array of text setup operations.</param>
        public static void setAlphaTextExecOps(this TMP_Text rText,
                string sText, System.Action[] rOperations) {
            rText.setAlphaTextDontRender(sText);

            rText.setAlphaExecOps(rOperations);

            rText.setAlphaTextRender();
        }

        /// <summary>
        /// Utility method for the common operation of setting alpha 
        /// <paramref name="iAlpha"/> for the range of characters
        /// `[<paramref name="iCharBeginIdx"/>, <paramref name="iCharEndIdx"/>[``.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharBeginIdx">Start index of the range of characters 
        /// to be affected.
        /// </param>
        /// <param name="iCharEndIdx">End index of the range of characters to
        /// be affected.
        /// </param>
        /// <param name="iAlpha">Alpha level to set. The value passed must be
        /// in the [0, 255] range.
        /// </param>
        public static void setAlphaForCharsRange(this TMP_Text rText,
                int iCharBeginIdx, int iCharEndIdx, byte iAlpha) {
            for (int i = iCharBeginIdx; i < iCharEndIdx; ++i) {
                setCharAlpha(rText, i, iAlpha);
            }
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region CORE_OPS

        /// <summary>
        /// Sets the color alpha of the <paramref name="iCharIdx"/> character 
        /// vertices so that it fades according to the alpha values specified 
        /// for the four corners of the character.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharIdx">Index of the character to set.</param>
        /// <param name="iTopLeftAlpha">Alpha level for the top left corner.
        /// </param>
        /// <param name="iTopRightAlpha">Alpha level for the top right corner.
        /// </param>
        /// <param name="iBottomRightAlpha">Alpha level for the bottom right
        /// corner.</param>
        /// <param name="iBottomLeftAlpha">Alpha level for the bottom left
        /// corner.</param>
        public static void setCharAlphaCornersFade(this TMP_Text rText,
                int iCharIdx,
                byte iTopLeftAlpha, byte iTopRightAlpha,
                byte iBottomRightAlpha, byte iBottomLeftAlpha) {

            TMP_TextInfo rTI = rText.textInfo;
            TMP_CharacterInfo rCI = rTI.characterInfo[iCharIdx];
            if (!rCI.isVisible) return;

            int iMaterialIndex = rCI.materialReferenceIndex;
            Color32[] rVertColors = rTI.meshInfo[iMaterialIndex].colors32;
            int iVertexIndex = rCI.vertexIndex;

            rVertColors[iVertexIndex + 0].a = iBottomLeftAlpha;
            rVertColors[iVertexIndex + 1].a = iTopLeftAlpha;
            rVertColors[iVertexIndex + 2].a = iTopRightAlpha;
            rVertColors[iVertexIndex + 3].a = iBottomRightAlpha;
        }

        /// <summary>
        /// Sets the vertices color alpha for all the characters so that the 
        /// whole text fades according to the alpha values specified for the
        /// four corners of the text mesh.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iTopLeftAlpha">Alpha level for the top left corner.
        /// </param>
        /// <param name="iTopRightAlpha">Alpha level for the top right corner.
        /// </param>
        /// <param name="iBottomRightAlpha">Alpha level for the bottom right
        /// corner.</param>
        /// <param name="iBottomLeftAlpha">Alpha level for the bottom left
        /// corner.</param>
        public static void setTextAlphaCornersFade(this TMP_Text rText,
                byte iTopLeftAlpha, byte iTopRightAlpha,
                byte iBottomRightAlpha, byte iBottomLeftAlpha) {

            Bounds textBounds = rText.textBounds;
            float fLeft = textBounds.min.x;
            float fRight = textBounds.max.x;
            float fTop = textBounds.max.y;
            float fBottom = textBounds.min.y;

            TMP_TextInfo rTI = rText.textInfo;

            for (int i = 0; i < rTI.characterCount; ++i) {
                TMP_CharacterInfo currCI = rTI.characterInfo[i];
                if (!currCI.isVisible) continue;

                float fCharLeft = currCI.bottomLeft.x;
                float fCharRight = currCI.bottomRight.x;
                float fCharTop = currCI.topLeft.y;
                float fCharBottom = currCI.bottomLeft.y;

                Vector2 vCharTopLeft = new Vector2(
                    Mathf.InverseLerp(fLeft, fRight, fCharLeft),
                    Mathf.InverseLerp(fBottom, fTop, fCharTop)
                );
                Vector2 vCharBottomLeft = new Vector2(
                    Mathf.InverseLerp(fLeft, fRight, fCharLeft),
                    Mathf.InverseLerp(fBottom, fTop, fCharBottom)
                );
                Vector2 vCharTopRight = new Vector2(
                    Mathf.InverseLerp(fLeft, fRight, fCharRight),
                    Mathf.InverseLerp(fBottom, fTop, fCharTop)
                );
                Vector2 vCharBottomRight = new Vector2(
                    Mathf.InverseLerp(fLeft, fRight, fCharRight),
                    Mathf.InverseLerp(fBottom, fTop, fCharBottom)
                );

                byte topLeftAlpha = bilinearInterpolation(iTopLeftAlpha,
                    iTopRightAlpha, iBottomRightAlpha, iBottomLeftAlpha,
                    vCharTopLeft);
                byte bottomLeftAlpha = bilinearInterpolation(iTopLeftAlpha,
                    iTopRightAlpha, iBottomRightAlpha, iBottomLeftAlpha,
                    vCharBottomLeft);
                byte topRightAlpha = bilinearInterpolation(iTopLeftAlpha,
                    iTopRightAlpha, iBottomRightAlpha, iBottomLeftAlpha,
                    vCharTopRight);
                byte bottomRightAlpha = bilinearInterpolation(iTopLeftAlpha,
                    iTopRightAlpha, iBottomRightAlpha, iBottomLeftAlpha,
                    vCharBottomRight);

                setCharAlphaCornersFade(rText, i, topLeftAlpha, topRightAlpha,
                    bottomRightAlpha, bottomLeftAlpha);
            }
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region SINGLECHAR_SHORTCUTS

        /// <summary>
        /// Sets the color alpha of the <paramref name="iCharIdx"/> character
        /// vertices so that it fades according to the alpha values specified
        /// for the four corners of the character through the 
        /// <paramref name="rCornersAlpha"/> array.
        /// The order of the corners is clockwise starting from top-left:
        /// top-left, top-right, bottom-right, bottom-left.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharIdx">Index of the character to set.</param>
        /// <param name="rCornersAlpha">Alpha levels for the four character 
        /// corners, stored in a `byte` array.</param>
        public static void setCharAlphaCornersFade(this TMP_Text rText,
                int iCharIdx, byte[] rCornersAlpha) {
            setCharAlphaCornersFade(rText, iCharIdx,
                rCornersAlpha[0], rCornersAlpha[1],
                rCornersAlpha[2], rCornersAlpha[3]);
        }

        /// <summary>
        /// Sets the the <paramref name="iCharIdx"/> character vertices color so
        /// that it has alpha value <paramref name="iAlpha"/>.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharIdx">Index of the character to set.</param>
        /// <param name="iAlpha">Alpha level for the character.</param>
        public static void setCharAlpha(this TMP_Text rText, int iCharIdx,
                byte iAlpha) {
            setCharAlphaCornersFade(rText, iCharIdx,
                iAlpha, iAlpha, iAlpha, iAlpha);
        }

        /// <summary>
        /// Sets the the <paramref name="iCharIdx"/> character vertices color so
        /// that it fades horizontally between <paramref name="iLeftAlpha"/> and
        /// <paramref name="iRightAlpha"/>.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharIdx">Index of the character to set.</param>
        /// <param name="iLeftAlpha">Alpha level for the left side of the 
        /// character.</param>
        /// <param name="iRightAlpha">Alpha level for the right side of the 
        /// character.</param>
        public static void setCharAlphaHorizFade(this TMP_Text rText,
                int iCharIdx, byte iLeftAlpha, byte iRightAlpha) {
            setCharAlphaCornersFade(rText, iCharIdx,
                iLeftAlpha, iRightAlpha, iRightAlpha, iLeftAlpha);
        }

        /// <summary>
        /// Sets the the <paramref name="iCharIdx"/> character vertices color so
        /// that it fades vertically between <paramref name="iTopAlpha"/> and
        /// <paramref name="iBottomAlpha"/>.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharIdx">Index of the character to set.</param>
        /// <param name="iTopAlpha">Alpha level for the top side of the 
        /// character.</param>
        /// <param name="iBottomAlpha">Alpha level for the bottom side of the 
        /// character.</param>
        public static void setCharAlphaVertFade(this TMP_Text rText,
                int iCharIdx, byte iTopAlpha, byte iBottomAlpha) {
            setCharAlphaCornersFade(rText, iCharIdx,
                iTopAlpha, iTopAlpha, iBottomAlpha, iBottomAlpha);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region WHOLETEXT_SHORTCUTS

        /// <summary>
        /// Sets the vertices color alpha for all the characters so that the 
        /// whole text fades according to the alpha values specified for the 
        /// four corners of the text mesh through the 
        /// <paramref name="rCornersAlpha"/> array.
        /// The order of the corners is clockwise starting from top-left:
        /// top-left, top-right, bottom-right, bottom-left.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="rCornersAlpha">Alpha levels for the four character 
        /// corners, stored in a `byte` array.</param>
        public static void setTextAlphaCornersFade(this TMP_Text rText,
                byte[] rCornersAlpha) {
            setTextAlphaCornersFade(rText,
                rCornersAlpha[0], rCornersAlpha[1],
                rCornersAlpha[2], rCornersAlpha[3]);
        }

        /// <summary>
        /// Sets the vertices color alpha to <paramref name="iAlpha"/> for all 
        /// the characters of the text mesh.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iAlpha">Alpha level for the character.</param>
        public static void setTextAlpha(this TMP_Text rText, byte iAlpha) {
            setTextAlphaCornersFade(rText, iAlpha, iAlpha, iAlpha, iAlpha);
        }

        /// <summary>
        /// Sets the vertices color alpha for all the characters so that the
        /// whole text fades horizontally between <paramref name="iLeftAlpha"/>
        /// and <paramref name="iRightAlpha"/>.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iLeftAlpha">Alpha level for the left side of the text.
        /// </param>
        /// <param name="iRightAlpha">Alpha level for the right side of the 
        /// text./param>
        public static void setTextAlphaHorizFade(this TMP_Text rText,
                byte iLeftAlpha, byte iRightAlpha) {
            setTextAlphaCornersFade(rText,
                iLeftAlpha, iRightAlpha, iRightAlpha, iLeftAlpha);
        }

        /// <summary>
        /// Sets the vertices color alpha for all the characters so that the
        /// whole text fades vertically between <paramref name="iTopAlpha"/> and
        /// <paramref name="iBottomAlpha"/>.
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iTopAlpha">Alpha level for the top side of the text.
        /// </param>
        /// <param name="iBottomAlpha">Alpha level for the bottom side of the
        /// text.</param>
        public static void setTextAlphaVertFade(this TMP_Text rText,
                byte iTopAlpha, byte iBottomAlpha) {
            setTextAlphaCornersFade(rText,
                iTopAlpha, iTopAlpha, iBottomAlpha, iBottomAlpha);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region ADVANCED_OPS

        /// <summary>
        /// Calculates and sets the alpha of character 
        /// <paramref name="iCharIdx"/> vertices such that it nicely fades 
        /// horizontally between <paramref name="iLeftAlpha"/> and 
        /// <paramref name="iRightAlpha"/>.
        /// The fade starts at horizontal offset <paramref name="f01FadeStart"/>
        /// and has width <paramref name="f01FadeWidth"/>, where both values are
        /// expected to be in [0f, 1f] and refer to the width of the character 
        /// (e.g. 0.3f means "a fade with width equal to 30% of the width of the
        /// <paramref name="iCharIdx"/> character).
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharIdx">Index of the character to set.</param>
        /// <param name="f01FadeStart">Horizontal fade offset.</param>
        /// <param name="f01FadeWidth">Horizontal fade width.</param>
        /// <param name="iLeftAlpha">Alpha level for character at the start of 
        /// the fade.</param>
        /// <param name="iRightAlpha">Alpha level for character at the end of
        /// the fade.</param>
        public static void setCharAlphaFadeHorizEx(this TMP_Text rText, 
                int iCharIdx, float f01FadeStart, float f01FadeWidth = 0.5f,
                byte iLeftAlpha = 255, byte iRightAlpha = 0) {

            TMP_TextInfo rTI = rText.textInfo;
            TMP_CharacterInfo ci = rTI.characterInfo[iCharIdx];

            float fCharLeft = ci.bottomLeft.x;
            float fCharRight = ci.bottomRight.x;
            float fCharWidth = fCharRight - fCharLeft;

            float fFadeWidth = f01FadeWidth * fCharWidth;
            float fFadeOffsetSpan = fCharWidth + fFadeWidth;

            float fFadeOffset = f01FadeStart * fFadeOffsetSpan;

            float fBeginX = fCharLeft - fFadeWidth + fFadeOffset;
            float fEndX = fBeginX + fFadeWidth;

            float fLeftAlpha = Mathf.InverseLerp(fBeginX, fEndX, fCharLeft);
            float fRightAlpha = Mathf.InverseLerp(fBeginX, fEndX, fCharRight);

            byte iLeftVertexAlpha = (byte)Mathf.RoundToInt(
                Mathf.LerpUnclamped(iLeftAlpha, iRightAlpha, fLeftAlpha));
            byte iRightVertexAlpha = (byte)Mathf.RoundToInt(
                Mathf.LerpUnclamped(iLeftAlpha, iRightAlpha, fRightAlpha));

            setCharAlphaHorizFade(rText, iCharIdx,
                iLeftVertexAlpha, iRightVertexAlpha);
        }

        /// <summary>
        /// Calculates and sets the alpha of character 
        /// <paramref name="iCharIdx"/> vertices such that it nicely fades 
        /// vertically between <paramref name="iBottomAlpha"/> and 
        /// <paramref name="iTopAlpha"/>.
        /// The fade starts at vertical offset <paramref name="f01FadeStart"/>
        /// and has height <paramref name="f01FadeHeight"/>, where both values 
        /// are expected to be in [0f, 1f] and refer to the height of the
        /// character (e.g. 0.3f means "a fade with height equal to 30% of the
        /// height of the <paramref name="iCharIdx"/> character).
        /// </summary>
        /// <param name="rText">Reference to `TMP_Text` instance.</param>
        /// <param name="iCharIdx">Index of the character to set.</param>
        /// <param name="f01FadeStart">Vertical fade offset.</param>
        /// <param name="f01FadeHeight">Vertical fade height.</param>
        /// <param name="iTopAlpha">Alpha level for character at the end of the 
        /// fade.</param>
        /// <param name="iBottomAlpha">Alpha level for character at the start of 
        /// the fade.</param>
        public static void setCharAlphaFadeVertEx(this TMP_Text rText,
                int iCharIdx, float f01FadeStart, float f01FadeHeight = 0.5f,
                byte iTopAlpha = 255, byte iBottomAlpha = 0) {

            TMP_TextInfo rTI = rText.textInfo;
            TMP_CharacterInfo ci = rTI.characterInfo[iCharIdx];

            float fCharBottom = ci.bottomLeft.y;
            float fCharTop = ci.topLeft.y;
            float fCharHeight = fCharTop - fCharBottom;

            float fFadeHeight = f01FadeHeight * fCharHeight;
            float fFadeOffsetSpan = fCharHeight + fFadeHeight;

            float fFadeOffset = (1f-f01FadeStart) * fFadeOffsetSpan;

            float fBeginY = fCharBottom - fFadeHeight + fFadeOffset;
            float fEndY = fBeginY + fFadeHeight;

            float fBottomAlpha = Mathf.InverseLerp(fBeginY, fEndY, fCharBottom);
            float fTopAlpha = Mathf.InverseLerp(fBeginY, fEndY, fCharTop);

            byte iBottomVertexAlpha = (byte)Mathf.RoundToInt(
                Mathf.LerpUnclamped(iBottomAlpha, iTopAlpha, fBottomAlpha));
            byte iTopVertexAlpha = (byte)Mathf.RoundToInt(
                Mathf.LerpUnclamped(iBottomAlpha, iTopAlpha, fTopAlpha));

            setCharAlphaVertFade(rText, iCharIdx, 
                iTopVertexAlpha, iBottomVertexAlpha);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region PRIVATE_UTILS

        /// <summary>
        /// Calculates the alpha value at normalized coordinates 
        /// <paramref name="vCoords"/>, according to the alpha values specified 
        /// for the four corners of the text mesh.
        /// </summary>
        /// <param name="iTopLeftAlpha">Alpha level for the top left corner.
        /// </param>
        /// <param name="iTopRightAlpha">Alpha level for the top right corner.
        /// </param>
        /// <param name="iBottomRightAlpha">Alpha level for the bottom right
        /// corner.</param>
        /// <param name="iBottomLeftAlpha">Alpha level for the bottom left
        /// corner.</param>
        /// <param name="vCoords">Coordinates at which we want to calculate the
        /// alpha value.</param>
        /// <returns></returns>
        private static byte bilinearInterpolation(
                byte iTopLeftAlpha, byte iTopRightAlpha,
                byte iBottomRightAlpha, byte iBottomLeftAlpha,
                Vector2 vCoords) {
            float fTop = Mathf.Lerp(iTopLeftAlpha, iTopRightAlpha, vCoords.x);
            float fBottom = Mathf.Lerp(iBottomLeftAlpha, iBottomRightAlpha,
                vCoords.x);
            float cUV = Mathf.Lerp(fBottom, fTop, vCoords.y);

            return (byte)Mathf.RoundToInt(cUV);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
    }
}