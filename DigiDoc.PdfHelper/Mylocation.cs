using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;


namespace DigiDoc.PdfHelper
{
    public class myLocationTextExtractionStrategy : ITextExtractionStrategy
    {
        private float _UndercontentCharacterSpacing = 0;
        private float _UndercontentHorizontalScaling = 0;

        private SortedList<string, DocumentFont> ThisPdfDocFonts;

        public static bool DUMP_STATE = false;
       

        private List<TextChunk> locationalResult = new List<TextChunk>();
                

        public myLocationTextExtractionStrategy()
        {
            ThisPdfDocFonts = new SortedList<string, DocumentFont>();
        }

               

        public virtual void BeginTextBlock()
        {
        }

               

        public virtual void EndTextBlock()
        {
        }

               

        private bool StartsWithSpace(String str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            return str[0] == ' ';
        }

               

        private bool EndsWithSpace(String str)
        {
            if (str.Length == 0)
            {
                return false;
            }
            return str[str.Length - 1] == ' ';
        }

        public float UndercontentCharacterSpacing
        {
            get { return _UndercontentCharacterSpacing; }
            set { _UndercontentCharacterSpacing = value; }
        }

        public float UndercontentHorizontalScaling
        {
            get { return _UndercontentHorizontalScaling; }
            set { _UndercontentHorizontalScaling = value; }
        }

        public virtual String GetResultantText()
        {

            if (DUMP_STATE)
            {
                DumpState();
            }

            locationalResult.Sort();

            StringBuilder sb = new StringBuilder();
            TextChunk lastChunk = null;

            foreach (TextChunk chunk in locationalResult)
            {
                if (lastChunk == null)
                {
                    sb.Append(chunk.text);
                }
                else
                {
                    if (chunk.SameLine(lastChunk))
                    {
                        float dist = chunk.DistanceFromEndOf(lastChunk);
                        if (dist < -chunk.charSpaceWidth)
                        {
                            sb.Append(' ');
                          
                        }
                        else if (dist > chunk.charSpaceWidth / 2f && !StartsWithSpace(chunk.text) && !EndsWithSpace(lastChunk.text))
                        {
                            sb.Append(' ');
                        }

                        sb.Append(chunk.text);
                    }
                    else
                    {
                        sb.Append((char)10);
                        sb.Append(chunk.text);
                    }
                }
                lastChunk = chunk;
            }

            return sb.ToString();

        }

        public List<iTextSharp.text.Rectangle> GetTextLocations(string pSearchString, System.StringComparison pStrComp)
        {
            List<iTextSharp.text.Rectangle> FoundMatches = new List<iTextSharp.text.Rectangle>();
            StringBuilder sb = new StringBuilder();
            List<TextChunk> ThisLineChunks = new List<TextChunk>();
            bool bStart = false;
            bool bEnd = false;
            TextChunk FirstChunk = null;
            TextChunk LastChunk = null;
            string sTextInUsedChunks = null;

            foreach (TextChunk chunk in locationalResult)
            {
                if (ThisLineChunks.Count > 0 && !chunk.SameLine(ThisLineChunks[ThisLineChunks.Count - 1]))
                {
                    if (sb.ToString().IndexOf(pSearchString, pStrComp) > -1)
                    {
                        string sLine = sb.ToString();

                     
                        int iCount = 0;
                        int lPos = 0;
                        lPos = sLine.IndexOf(pSearchString, 0, pStrComp);
                        while (lPos > -1)
                        {
                            iCount += 1;
                            if (lPos + pSearchString.Length > sLine.Length)
                                break; 

                            else
                                lPos = lPos + pSearchString.Length;
                            lPos = sLine.IndexOf(pSearchString, lPos, pStrComp);
                        }

                       
                        int curPos = 0;
                        for (int i = 1; i <= iCount; i++)
                        {
                            string sCurrentText = null;
                            int iFromChar = 0;
                            int iToChar = 0;

                            iFromChar = sLine.IndexOf(pSearchString, curPos, pStrComp);
                            curPos = iFromChar;
                            iToChar = iFromChar + pSearchString.Length - 1;
                            sCurrentText = null;
                            sTextInUsedChunks = null;
                            FirstChunk = null;
                            LastChunk = null;

                            
                            foreach (TextChunk chk in ThisLineChunks)
                            {
                                sCurrentText = sCurrentText + chk.text;

                                
                                if (!bStart && sCurrentText.Length - 1 >= iFromChar)
                                {
                                    FirstChunk = chk;
                                    bStart = true;
                                }

                                
                                if (bStart & !bEnd)
                                {
                                    sTextInUsedChunks = sTextInUsedChunks + chk.text;
                                }

                               
                                if (!bEnd && sCurrentText.Length - 1 >= iToChar)
                                {
                                    LastChunk = chk;
                                    bEnd = true;
                                }

                              
                                if (bStart & bEnd)
                                {
                                    FoundMatches.Add(GetRectangleFromText(FirstChunk, LastChunk, pSearchString, sTextInUsedChunks, iFromChar, iToChar, pStrComp));
                                    curPos = curPos + pSearchString.Length;
                                    bStart = false;
                                    bEnd = false;
                                    break; 
                                }
                            }
                        }
                    }
                    sb.Clear();
                    ThisLineChunks.Clear();
                }
                ThisLineChunks.Add(chunk);
                sb.Append(chunk.text);
            }

            return FoundMatches;
        }

        private iTextSharp.text.Rectangle GetRectangleFromText(TextChunk FirstChunk, TextChunk LastChunk, string pSearchString, string sTextinChunks, int iFromChar, int iToChar, System.StringComparison pStrComp)
        {
           
            float LineRealWidth = LastChunk.PosRight - FirstChunk.PosLeft;

           
            float LineTextWidth = GetStringWidth(sTextinChunks, LastChunk.curFontSize, LastChunk.charSpaceWidth, ThisPdfDocFonts.Values[LastChunk.FontIndex]);
             float TransformationValue = LineRealWidth / LineTextWidth;

            int iStart = sTextinChunks.IndexOf(pSearchString, pStrComp);

            int iEnd = iStart + pSearchString.Length - 1;

            string sLeft = null;
            if (iStart == 0)
                sLeft = null;
            else
                sLeft = sTextinChunks.Substring(0, iStart);

            string sRight = null;
            if (iEnd == sTextinChunks.Length - 1)
                sRight = null;
            else
                sRight = sTextinChunks.Substring(iEnd + 1, sTextinChunks.Length - iEnd - 1);

            float LeftWidth = 0;
            if (iStart > 0)
            {
                LeftWidth = GetStringWidth(sLeft, LastChunk.curFontSize, LastChunk.charSpaceWidth, ThisPdfDocFonts.Values[LastChunk.FontIndex]);
                LeftWidth = LeftWidth * TransformationValue;
            }

            float RightWidth = 0;
            if (iEnd < sTextinChunks.Length - 1)
            {
                RightWidth = GetStringWidth(sRight, LastChunk.curFontSize, LastChunk.charSpaceWidth, ThisPdfDocFonts.Values[LastChunk.FontIndex]);
                RightWidth = RightWidth * TransformationValue;
            }

           float LeftOffset = FirstChunk.distParallelStart + LeftWidth;
            float RightOffset = LastChunk.distParallelEnd - RightWidth;
            return new iTextSharp.text.Rectangle(LeftOffset, FirstChunk.PosBottom, RightOffset, FirstChunk.PosTop);
        }

        private float GetStringWidth(string str, float curFontSize, float pSingleSpaceWidth, DocumentFont pFont)
        {
            char[] chars = str.ToCharArray();
            float totalWidth = 0;
            float w = 0;

            foreach (char c in chars)
            {
                w = pFont.GetWidth(c) / 1000f;
                totalWidth += (w * curFontSize + this.UndercontentCharacterSpacing) * this.UndercontentHorizontalScaling / 100;
            }

            return totalWidth;
        }

        private void DumpState()
        {
            foreach (TextChunk location in locationalResult)
            {
                location.PrintDiagnostics();
                Console.WriteLine();
            }
        }

        public virtual void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment segment = renderInfo.GetBaseline();
            TextChunk location = new TextChunk(renderInfo.GetText(), segment.GetStartPoint(), segment.GetEndPoint(), renderInfo.GetSingleSpaceWidth());

            var _with1 = location;

             _with1.PosLeft = renderInfo.GetDescentLine().GetStartPoint()[Vector.I1];
            _with1.PosRight = renderInfo.GetAscentLine().GetEndPoint()[Vector.I1];
            _with1.PosBottom = renderInfo.GetDescentLine().GetStartPoint()[Vector.I2];
            _with1.PosTop = renderInfo.GetAscentLine().GetEndPoint()[Vector.I2];
            _with1.curFontSize = _with1.PosTop - segment.GetStartPoint()[Vector.I2];
            string StrKey = renderInfo.GetFont().PostscriptFontName + _with1.curFontSize.ToString();
            if (!ThisPdfDocFonts.ContainsKey(StrKey))
                ThisPdfDocFonts.Add(StrKey, renderInfo.GetFont());
            _with1.FontIndex = ThisPdfDocFonts.IndexOfKey(StrKey);
            locationalResult.Add(location);
        }

            

        public class TextChunk : IComparable<TextChunk>
        {
           
            internal String text;
          
            internal Vector startLocation;
           
            internal Vector endLocation;
            
            internal Vector orientationVector;
           
            internal int orientationMagnitude;
           
            internal int distPerpendicular;
          
            internal float distParallelStart;
           
            internal float distParallelEnd;
           
            internal float charSpaceWidth;

            private float _PosLeft;

            private float _PosRight;

            private float _PosTop;

            private float _PosBottom;

            private float _curFontSize;

            private int _FontIndex;
            public int FontIndex
            {
                get { return _FontIndex; }
                set { _FontIndex = value; }
            }

            public float PosLeft
            {
                get { return _PosLeft; }
                set { _PosLeft = value; }
            }

            public float PosRight
            {
                get { return _PosRight; }
                set { _PosRight = value; }
            }

            public float PosTop
            {
                get { return _PosTop; }
                set { _PosTop = value; }
            }

            public float PosBottom
            {
                get { return _PosBottom; }
                set { _PosBottom = value; }
            }

            public float curFontSize
            {
                get { return _curFontSize; }
                set { _curFontSize = value; }
            }

            public TextChunk(String str, Vector startLocation, Vector endLocation, float charSpaceWidth)
            {
                this.text = str;
                this.startLocation = startLocation;
                this.endLocation = endLocation;
                this.charSpaceWidth = charSpaceWidth;

                Vector oVector = endLocation.Subtract(startLocation);
                if (oVector.Length == 0)
                {
                    oVector = new Vector(1, 0, 0);
                }
                orientationVector = oVector.Normalize();
                orientationMagnitude = Convert.ToInt32(Math.Truncate(Math.Atan2(orientationVector[Vector.I2], orientationVector[Vector.I1]) * 1000));

                Vector origin = new Vector(0, 0, 1);
                distPerpendicular = Convert.ToInt32((startLocation.Subtract(origin)).Cross(orientationVector)[Vector.I3]);

                distParallelStart = orientationVector.Dot(startLocation);
                distParallelEnd = orientationVector.Dot(endLocation);
            }

            public void PrintDiagnostics()
            {
                Console.WriteLine("Text (@" + Convert.ToString(startLocation) + " -> " + Convert.ToString(endLocation) + "): " + text);
                Console.WriteLine("orientationMagnitude: " + orientationMagnitude);
                Console.WriteLine("distPerpendicular: " + distPerpendicular);
                Console.WriteLine("distParallel: " + distParallelStart);
            }

                       

            public bool SameLine(TextChunk a)
            {
                if (orientationMagnitude != a.orientationMagnitude)
                {
                    return false;
                }
                if (distPerpendicular != a.distPerpendicular)
                {
                    return false;
                }
                return true;
            }

                      

            public float DistanceFromEndOf(TextChunk other)
            {
                float distance = distParallelStart - other.distParallelEnd;
                return distance;
            }

                       

            public int CompareTo(TextChunk rhs)
            {
                if (object.ReferenceEquals(this, rhs))
                {
                    return 0;
                }
                int rslt = 0;
                rslt = CompareInts(orientationMagnitude, rhs.orientationMagnitude);
                if (rslt != 0)
                {
                    return rslt;
                }

                rslt = CompareInts(distPerpendicular, rhs.distPerpendicular);
                if (rslt != 0)
                {
                    return rslt;
                }

                 rslt = distParallelStart < rhs.distParallelStart ? -1 : 1;

                return rslt;
            }

                       

            private static int CompareInts(int int1, int int2)
            {
                return int1 == int2 ? 0 : int1 < int2 ? -1 : 1;
            }
        }

                

        public void RenderImage(ImageRenderInfo renderInfo)
        {
           
        }
    }
}

