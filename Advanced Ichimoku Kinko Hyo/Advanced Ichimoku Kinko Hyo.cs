
using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;


namespace cAlgo
{
    [Cloud("Seukou Span B", "Seukou Span A", Opacity = 0.2, FirstColor = "B22222", SecondColor = "FF7CFC00")]
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class AdvancedIchimokuKinkoHyo : Indicator
    {
        [Parameter(DefaultValue = 9, Group = "Ichimoku")]
        public int TenkanSenPeriods { get; set; }

        [Parameter(DefaultValue = 26, Group = "Ichimoku")]
        public int KijunSenPeriods { get; set; }

        [Parameter(DefaultValue = 52, Group = "Ichimoku")]
        public int SeukouSpanBPeriods { get; set; }

        [Output("Tenkan Sen", LineColor = "#FFFF0000")]
        public IndicatorDataSeries TenkanSen { get; set; }
        [Output("Kijun Sen", LineColor = "#FF0000FF")]
        public IndicatorDataSeries KijunSen { get; set; }
        [Output("Chikou Span", LineColor = "#FFFFFF00")]
        public IndicatorDataSeries ChikouSpan { get; set; }

        [Output("Seukou Span B", LineColor = "#FFFF0000", LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries SeukouSpanB { get; set; }
        [Output("Seukou Span A", LineColor = "FF008000", LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries SeukouSpanA { get; set; }

        //[Parameter(DefaultValue = 26)]
        public int DisplacementChikou { get; set; }

        //[Parameter(DefaultValue = 26)]
        public int DisplacementKumo { get; set; }

        [Parameter("Depth", DefaultValue = 26, Group = "Waves - Primary")]
        public int Depth { get; set; }

        [Parameter("Deviation", DefaultValue = 17, Group = "Waves - Primary")]
        public int Deviation { get; set; }

        [Parameter("BackStep", DefaultValue = 9, Group = "Waves - Primary")]
        public int BackStep { get; set; }

        [Parameter("Depth", DefaultValue = 101, Group = "Waves - Secondary")]
        public int Depth2 { get; set; }

        [Parameter("Deviation", DefaultValue = 26, Group = "Waves - Secondary")]
        public int Deviation2 { get; set; }

        [Parameter("BackStep", DefaultValue = 17, Group = "Waves - Secondary")]
        public int BackStep2 { get; set; }

        [Output("Waves - Primary", LineColor = "White")]
        public IndicatorDataSeries Result { get; set; }

        [Output("Waves - Secondary", LineColor = "Yellow")]
        public IndicatorDataSeries Result2 { get; set; }

        [Parameter("Color", DefaultValue = "Blue", Group = "KihonSuchiSeries")]
        public string Col { get; set; }
        [Parameter("Transparency %", DefaultValue = 100, Group = "KihonSuchiSeries")]
        public int Trsp { get; set; }
        //[Parameter("Magic Number", DefaultValue = 0)]
        public int mgc = 0;
        public double maxfast, minfast, maxmedium, minmedium, maxslow, minslow;

        #region Waves
        public IndicatorDataSeries ZigZag { get; set; }
        public IndicatorDataSeries Idx { get; set; }

        public IndicatorDataSeries ZigZag2 { get; set; }
        public IndicatorDataSeries Idx2 { get; set; }

        public IndicatorDataSeries ZigZagHighs { get; set; }
        public IndicatorDataSeries ZigZagLows { get; set; }


        private double _lastLow;
        private double _lastHigh;
        private double _low;
        private double _high;
        private int _lastHighIndex;
        private int _lastLowIndex;
        private int _type;
        private double _point;
        private double _currentLow;
        private double _currentHigh;

        private int prevIdx;
        private int idx;
        private int tf;

        private IndicatorDataSeries _highZigZags;
        private IndicatorDataSeries _lowZigZags;

        private double _lastLow2;
        private double _lastHigh2;
        private double _low2;
        private double _high2;
        private int _lastHighIndex2;
        private int _lastLowIndex2;
        private int _type2;
        private double _point2;
        private double _currentLow2;
        private double _currentHigh2;

        private int prevIdx2;
        private int idx2;

        private IndicatorDataSeries _highZigZags2;
        private IndicatorDataSeries _lowZigZags2;

        #region KihonSuchi
        private int x1;
        private string selected = "none";
        private Color alphaColor;
        private Color betaColor;
        private Color gammaColor;
        private string name;
        private IchimokuKinkoHyo IchimokuKinkoHyo;
        #endregion
        #endregion

        protected override void Initialize()
        {
            DisplacementChikou = KijunSenPeriods;
            DisplacementKumo = KijunSenPeriods;
            //Waves
            #region Waves
            _highZigZags = CreateDataSeries();
            _lowZigZags = CreateDataSeries();
            ZigZag = CreateDataSeries();
            Idx = CreateDataSeries();
            ZigZagHighs = CreateDataSeries();
            ZigZagLows = CreateDataSeries();
            _point = Symbol.TickSize;

            idx = 0;
            prevIdx = -1;

            _highZigZags2 = CreateDataSeries();
            _lowZigZags2 = CreateDataSeries();
            ZigZag2 = CreateDataSeries();
            Idx2 = CreateDataSeries();

            _point2 = Symbol.TickSize;

            idx2 = 0;
            prevIdx2 = -1;

            string tfString = Bars.TimeFrame.ToString();
            switch (tfString)
            {
                case "Minute":
                    tf = 1;
                    break;
                case "Minute5":
                    tf = 5;
                    break;
                case "Minute10":
                    tf = 10;
                    break;
                case "Minute15":
                    tf = 15;
                    break;
                case "Minute30":
                    tf = 30;
                    break;
                case "Minute45":
                    tf = 45;
                    break;
                case "Hour":
                    tf = 60;
                    break;
                case "Hour2":
                    tf = 60 * 2;
                    break;
                case "Hour3":
                    tf = 60 * 3;
                    break;
                case "Hour4":
                    tf = 60 * 4;
                    break;
                case "Hour6":
                    tf = 60 * 6;
                    break;
                case "Hour8":
                    tf = 60 * 8;
                    break;
                case "Hour12":
                    tf = 60 * 12;
                    break;
                case "Daily":
                    tf = 60 * 24;
                    break;
                case "Day2":
                    tf = 60 * 24 * 2;
                    break;
                case "Day3":
                    tf = 60 * 24 * 3;
                    break;
                case "Weekly":
                    tf = 60 * 24 * 7;
                    break;
                case "Monthly":
                    tf = 60 * 24 * 7 * 4;
                    break;
                default:
                    Print("Error! Not Correct TimeFrame");
                    break;
            }
            #endregion

            #region KihonSuchi
            IchimokuKinkoHyo = Indicators.IchimokuKinkoHyo(TenkanSenPeriods, KijunSenPeriods, SeukouSpanBPeriods);
            name = "" + mgc.ToString();
            Color color = Color.FromName(Col);
            alphaColor = Color.FromArgb((int)(Trsp * 2.55), color.R, color.G, color.B);
            betaColor = Color.FromName("Green");
            gammaColor = Color.FromName("Red");
            DrawCursors();
            DrawCycles(x1, alphaColor);
            Chart.ObjectHoverChanged += OnChartObjectHoverChanged;
            Chart.MouseUp += OnChartMouseUp;
            Bars.BarOpened += Bars_BarOpened;
            #endregion
            var stackPanel = new StackPanel 
            {
                HorizontalAlignment = cAlgo.API.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                BackgroundColor = Color.DarkCyan,
                Opacity = 0.7
            };

            var button = new cAlgo.API.Button 
            {
                Text = "Subscribe to Pro",
                Margin = 3
            };
            button.Click += Button_Click;
            stackPanel.AddChild(button);
            var button2 = new cAlgo.API.Button 
            {
                Text = "Buy Pro",
                Margin = 3
            };
            button2.Click += Button2_Click;
            stackPanel.AddChild(button2);
            
            Chart.AddControl(stackPanel);
        }

        private void Button_Click(ButtonClickEventArgs obj)
        {
            System.Diagnostics.Process.Start("https://profitislander.gumroad.com/l/advichipro");
        }
        private void Button2_Click(ButtonClickEventArgs obj)
        {
            System.Diagnostics.Process.Start("https://profitislander.gumroad.com/l/rnpve");
        }
        public override void Calculate(int index)
        {
            //Ichimoku 5 lines
            #region Ichimoku 5 lines
            if ((index < TenkanSenPeriods) || (index < SeukouSpanBPeriods))
            {
                return;
            }

            maxfast = Bars.HighPrices[index];
            minfast = Bars.LowPrices[index];
            maxmedium = Bars.HighPrices[index];
            minmedium = Bars.LowPrices[index];
            maxslow = Bars.HighPrices[index];
            minslow = Bars.LowPrices[index];

            for (int i = 0; i < TenkanSenPeriods; i++)
            {
                if (maxfast < Bars.HighPrices[index - i])
                {
                    maxfast = Bars.HighPrices[index - i];
                }
                if (minfast > Bars.LowPrices[index - i])
                {
                    minfast = Bars.LowPrices[index - i];
                }
            }
            for (int i = 0; i < KijunSenPeriods; i++)
            {
                if (maxmedium < Bars.HighPrices[index - i])
                {
                    maxmedium = Bars.HighPrices[index - i];
                }
                if (minmedium > Bars.LowPrices[index - i])
                {
                    minmedium = Bars.LowPrices[index - i];
                }
            }
            for (int i = 0; i < SeukouSpanBPeriods; i++)
            {
                if (maxslow < Bars.HighPrices[index - i])
                {
                    maxslow = Bars.HighPrices[index - i];
                }
                if (minslow > Bars.LowPrices[index - i])
                {
                    minslow = Bars.LowPrices[index - i];
                }
            }
            TenkanSen[index] = (maxfast + minfast) / 2;
            KijunSen[index] = (maxmedium + minmedium) / 2;
            ChikouSpan[index - DisplacementChikou] = Bars.ClosePrices[index];

            SeukouSpanA[index + DisplacementKumo] = (TenkanSen[index] + KijunSen[index]) / 2;
            SeukouSpanB[index + DisplacementKumo] = (maxslow + minslow) / 2;
            #endregion

            #region Waves
            if (index < Depth)
            {
                Result[index] = 0;
                _highZigZags[index] = 0;
                _lowZigZags[index] = 0;
                ZigZagHighs[idx] = 0;
                ZigZagLows[idx] = 0;
                ZigZag[idx] = 0;
                Idx[idx] = 0;

                return;
            }
            if (index < Depth2)
            {
                Result2[index] = 0;
                _highZigZags2[index] = 0;
                _lowZigZags2[index] = 0;
                ZigZag2[idx2] = 0;
                Idx2[idx2] = 0;

                return;
            }

            _currentLow = Functions.Minimum(Bars.LowPrices, Depth);
            if (Math.Abs(_currentLow - _lastLow) < double.Epsilon)
            {
                _currentLow = 0.0;
            }
            else
            {
                _lastLow = _currentLow;

                if ((Bars.LowPrices[index] - _currentLow) > (Deviation * _point))
                {
                    _currentLow = 0.0;
                }
                else
                {
                    for (int i = 1; i <= BackStep; i++)
                    {
                        if (Math.Abs(_lowZigZags[index - i]) > double.Epsilon && _lowZigZags[index - i] > _currentLow)
                        {
                            _lowZigZags[index - i] = 0.0;
                        }
                    }
                }
            }
            if (Math.Abs(Bars.LowPrices[index] - _currentLow) < double.Epsilon)
            {
                _lowZigZags[index] = _currentLow;
            }
            else
            {
                _lowZigZags[index] = 0.0;
            }

            _currentHigh = Bars.HighPrices.Maximum(Depth);

            if (Math.Abs(_currentHigh - _lastHigh) < double.Epsilon)
            {
                _currentHigh = 0.0;
            }
            else
            {
                _lastHigh = _currentHigh;

                if ((_currentHigh - Bars.HighPrices[index]) > (Deviation * _point))
                {
                    _currentHigh = 0.0;
                }
                else
                {
                    for (int i = 1; i <= BackStep; i++)
                    {
                        if (Math.Abs(_highZigZags[index - i]) > double.Epsilon && _highZigZags[index - i] < _currentHigh)
                        {
                            _highZigZags[index - i] = 0.0;
                        }
                    }
                }
            }

            if (Math.Abs(Bars.HighPrices[index] - _currentHigh) < double.Epsilon)
            {
                _highZigZags[index] = _currentHigh;
            }
            else
            {
                _highZigZags[index] = 0.0;
            }

            switch (_type)
            {
                case 0:
                    if (Math.Abs(_low - 0) < double.Epsilon && Math.Abs(_high - 0) < double.Epsilon)
                    {
                        if (Math.Abs(_highZigZags[index]) > double.Epsilon)
                        {
                            _high = Bars.HighPrices[index];
                            _lastHighIndex = index;
                            _type = -1;
                            Result[index] = _high;
                            ZigZag[idx] = _high;
                            ZigZagHighs[idx] = _high;
                            Idx[idx] = index;
                            idx++;
                            Print("High Values : Results {0},ZigZag {1},IndexBar {2}", Result, ZigZag, Idx);
                        }
                        if (Math.Abs(_lowZigZags[index]) > double.Epsilon)
                        {
                            _low = Bars.LowPrices[index];
                            _lastLowIndex = index;
                            _type = 1;
                            Result[index] = _low;
                            ZigZagLows[idx] = _low;
                            ZigZag[idx] = _low;
                            Idx[idx] = index;
                            idx++;
                            Print("Low Values : Results {0},ZigZag {1},IndexBar {2}", Result, ZigZag, Idx);
                        }
                    }
                    break;
                case 1:
                    if (Math.Abs(_lowZigZags[index]) > double.Epsilon && _lowZigZags[index] < _low && Math.Abs(_highZigZags[index] - 0.0) < double.Epsilon)
                    {
                        Result[_lastLowIndex] = double.NaN;
                        _lastLowIndex = index;
                        _low = _lowZigZags[index];
                        Result[index] = _low;
                        ZigZag[idx - 1] = _low;
                        ZigZagLows[idx - 1] = _low;
                        Idx[idx - 1] = index;
                        Print("Low Values : Results {0},ZigZag {1},IndexBar {2}", Result, ZigZag, Idx);
                    }
                    if (Math.Abs(_highZigZags[index] - 0.0) > double.Epsilon && Math.Abs(_lowZigZags[index] - 0.0) < double.Epsilon)
                    {
                        _high = _highZigZags[index];
                        _lastHighIndex = index;
                        Result[index] = _high;
                        ZigZag[idx] = _high;
                        ZigZagHighs[idx] = _high;
                        Idx[idx] = index;
                        idx++;
                        _type = -1;
                        Print("High Values : Results {0},ZigZag {1},IndexBar {2}", Result, ZigZag, Idx);
                    }
                    break;
                case -1:
                    if (Math.Abs(_highZigZags[index]) > double.Epsilon && _highZigZags[index] > _high && Math.Abs(_lowZigZags[index] - 0.0) < double.Epsilon)
                    {
                        Result[_lastHighIndex] = double.NaN;
                        _lastHighIndex = index;
                        _high = _highZigZags[index];
                        Result[index] = _high;
                        ZigZagHighs[idx - 1] = _high;
                        ZigZag[idx - 1] = _high;
                        Idx[idx - 1] = index;
                        Print("High Values : Results {0},ZigZag {1},IndexBar {2}", Result, ZigZag, Idx);
                    }
                    if (Math.Abs(_lowZigZags[index]) > double.Epsilon && Math.Abs(_highZigZags[index]) <= double.Epsilon)
                    {
                        _low = _lowZigZags[index];
                        _lastLowIndex = index;
                        Result[index] = _low;
                        ZigZag[idx] = _low;
                        ZigZagLows[idx] = _low;
                        Idx[idx] = index;
                        idx++;
                        _type = 1;
                        Print("Low Values : Results {0},ZigZag {1},IndexBar {2}", Result, ZigZag, Idx);
                    }
                    break;
                default:
                    return;
            }

            _currentLow2 = Functions.Minimum(Bars.LowPrices, Depth2);
            if (Math.Abs(_currentLow2 - _lastLow2) < double.Epsilon)
            {
                _currentLow2 = 0.0;
            }
            else
            {
                _lastLow2 = _currentLow2;

                if ((Bars.LowPrices[index] - _currentLow2) > (Deviation2 * _point2))
                {
                    _currentLow2 = 0.0;
                }
                else
                {
                    for (int i = 1; i <= BackStep2; i++)
                    {
                        if (Math.Abs(_lowZigZags2[index - i]) > double.Epsilon && _lowZigZags2[index - i] > _currentLow2)
                        {
                            _lowZigZags2[index - i] = 0.0;
                        }
                    }
                }
            }
            if (Math.Abs(Bars.LowPrices[index] - _currentLow2) < double.Epsilon)
            {
                _lowZigZags2[index] = _currentLow2;
            }
            else
            {
                _lowZigZags2[index] = 0.0;
            }

            _currentHigh2 = Bars.HighPrices.Maximum(Depth2);

            if (Math.Abs(_currentHigh2 - _lastHigh2) < double.Epsilon)
            {
                _currentHigh2 = 0.0;
            }
            else
            {
                _lastHigh2 = _currentHigh2;

                if ((_currentHigh2 - Bars.HighPrices[index]) > (Deviation2 * _point2))
                {
                    _currentHigh2 = 0.0;
                }
                else
                {
                    for (int i = 1; i <= BackStep2; i++)
                    {
                        if (Math.Abs(_highZigZags2[index - i]) > double.Epsilon && _highZigZags2[index - i] < _currentHigh2)
                        {
                            _highZigZags2[index - i] = 0.0;
                        }
                    }
                }
            }

            if (Math.Abs(Bars.HighPrices[index] - _currentHigh2) < double.Epsilon)
            {
                _highZigZags2[index] = _currentHigh2;
            }
            else
            {
                _highZigZags2[index] = 0.0;
            }

            switch (_type2)
            {
                case 0:
                    if (Math.Abs(_low2 - 0) < double.Epsilon && Math.Abs(_high2 - 0) < double.Epsilon)
                    {
                        if (Math.Abs(_highZigZags2[index]) > double.Epsilon)
                        {
                            _high2 = Bars.HighPrices[index];
                            _lastHighIndex2 = index;
                            _type2 = -1;
                            Result2[index] = _high2;
                            ZigZag2[idx] = _high2;
                            Idx2[idx2] = index;
                            idx2++;
                        }
                        if (Math.Abs(_lowZigZags2[index]) > double.Epsilon)
                        {
                            _low2 = Bars.LowPrices[index];
                            _lastLowIndex2 = index;
                            _type2 = 1;
                            Result2[index] = _low2;
                            ZigZag2[idx] = _low2;
                            Idx2[idx2] = index;
                            idx2++;
                        }
                    }
                    break;
                case 1:
                    if (Math.Abs(_lowZigZags2[index]) > double.Epsilon && _lowZigZags2[index] < _low2 && Math.Abs(_highZigZags2[index] - 0.0) < double.Epsilon)
                    {
                        Result2[_lastLowIndex2] = double.NaN;
                        _lastLowIndex2 = index;
                        _low2 = _lowZigZags2[index];
                        Result2[index] = _low2;
                        ZigZag2[idx2 - 1] = _low2;
                        Idx2[idx2 - 1] = index;
                    }
                    if (Math.Abs(_highZigZags2[index] - 0.0) > double.Epsilon && Math.Abs(_lowZigZags2[index] - 0.0) < double.Epsilon)
                    {
                        _high2 = _highZigZags2[index];
                        _lastHighIndex2 = index;
                        Result2[index] = _high2;
                        ZigZag2[idx2] = _high2;
                        Idx2[idx2] = index;
                        idx2++;
                        _type2 = -1;
                    }
                    break;
                case -1:
                    if (Math.Abs(_highZigZags2[index]) > double.Epsilon && _highZigZags2[index] > _high2 && Math.Abs(_lowZigZags2[index] - 0.0) < double.Epsilon)
                    {
                        Result2[_lastHighIndex2] = double.NaN;
                        _lastHighIndex2 = index;
                        _high2 = _highZigZags2[index];
                        Result2[index] = _high2;
                        ZigZag2[idx2 - 1] = _high2;
                        Idx2[idx2 - 1] = index;
                    }
                    if (Math.Abs(_lowZigZags2[index]) > double.Epsilon && Math.Abs(_highZigZags2[index]) <= double.Epsilon)
                    {
                        _low2 = _lowZigZags2[index];
                        _lastLowIndex2 = index;
                        Result2[index] = _low2;
                        ZigZag2[idx2] = _low2;
                        Idx2[idx2] = index;
                        idx2++;
                        _type2 = 1;
                    }
                    break;
                default:
                    return;
            }

            if (prevIdx != index && (DateTime.UtcNow - Bars.OpenTimes[index]).TotalMinutes < tf)
            {
                Print("I am today ", tf);
            }
            prevIdx = index;
            prevIdx2 = index;

            #endregion
        }

        #region Kihon Suchi
        private void Bars_BarOpened(BarOpenedEventArgs obj)
        {

            if (IsSenBullish() == true && BullishChikouPriceCrossing() == true)
            {
                int start = GetRecentLowestPriceIndex(SeukouSpanBPeriods, TimeFrame);
                DrawCycles(start, betaColor);
            }
            if (IsSenBearish() == true && BearishChikouPriceCrossing() == true)
            {
                int start = GetRecentHighestPriceIndex(SeukouSpanBPeriods, TimeFrame);
                DrawCycles(start, gammaColor);
            }
        }


        private void OnChartMouseUp(ChartMouseEventArgs obj)
        {
            if (selected == "kihon1")
            {
                x1 = (int)obj.BarIndex;
                DrawCycles(x1, alphaColor);
            }
            //if (selected == "cycle2")
            //{
            //    x2 = (int)obj.BarIndex;
            //    drawCycles();
            //}
        }

        private void OnChartObjectHoverChanged(ChartObjectHoverChangedEventArgs obj)
        {
            if (!obj.IsObjectHovered)
            {
                selected = "none";
            }
            else
            {
                try
                {
                    selected = obj.ChartObject.Name == "kihon1" + name ? "kihon1" : "none";
                } catch (Exception e)
                {
                    Print(e.Message);
                    return;
                }
            }
        }

        private void DrawCursors()
        {
            int start;
            Color color;
            if (IsSenBullish() == true && BullishChikouPriceCrossing() == true)
            {
                start = GetRecentLowestPriceIndex(SeukouSpanBPeriods, TimeFrame);
                color = betaColor;
            }
            else
            {
                start = GetRecentHighestPriceIndex(SeukouSpanBPeriods, TimeFrame);
                color = gammaColor;
            }
            int index = start;
            Chart.DrawVerticalLine("kihon1" + name, index, color, 2);
            //Chart.DrawVerticalLine("cycle2" + name, index, alphaColor, 2, LineStyle.DotsRare);
            Chart.FindObject("kihon1" + name).IsInteractive = true;
            //Chart.FindObject("cycle2" + name).IsInteractive = true;
            x1 = index;
            //x2 = index;
        }

        private void DrawCycles(int StartPoint, Color color)
        {
            for (int i = 0; i < Chart.BarsTotal + 1000; i++)
            {
                Chart.RemoveObject("KIHON_SUCHI_series" + i + " " + name);
            }

            int coo1 = StartPoint - 1;
            for (int i = 0; i < KihonValues.Length; ++i)
            {
                Chart.DrawVerticalLine("KIHON_SUCHI_series" + i + " " + name, coo1 + KihonValues[i], color);
                double conjCoord = ((Chart.TopY - Chart.BottomY) * 0.75) + Chart.BottomY;
                //Chart.DrawTrendLine("Conj", x1, conjCoord, x2, conjCoord, alphaColor);
                Chart.DrawText("kihonSuchiValue" + KihonValues[i], "" + KihonValues[i], coo1 + KihonValues[i] + 1, conjCoord, color);
            }
        }

        public readonly int[] KihonValues = new int[17] 
        {
            1,
            9,
            17,
            26,
            33,
            42,
            51,
            65,
            76,
            83,
            97,
            101,
            129,
            151,
            172,
            226,
            257
        };

        /// <summary>
        /// Retrieves the recent Highest price and its periods from now
        /// </summary>
        /// <param name="Periods"></param>
        /// <param name="timeFrame"></param>
        /// <returns></returns>
        public int GetRecentHighestPriceIndex(int Periods, TimeFrame timeFrame)
        {
            //double maxHigh = MarketData.GetBars(timeFrame, SymbolName).HighPrices.Maximum(Periods);
            int Index = new int();
            double maxHigh = MarketData.GetBars(timeFrame, SymbolName).HighPrices.Last(1);
            for (int i = 0; i < Periods * 2; i++)
            {
                if (maxHigh < MarketData.GetBars(timeFrame, SymbolName).HighPrices.Last(i))
                {
                    maxHigh = MarketData.GetBars(timeFrame, SymbolName).HighPrices.Last(i);
                    DateTime dateTime = MarketData.GetBars(timeFrame, SymbolName).OpenTimes.Last(i);
                    //minLow
                    Index = Bars.OpenTimes.GetIndexByTime(dateTime);
                }
            }
            return Index;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Periods"></param>
        /// <param name="timeFrame"></param>
        /// <returns></returns>
        public int GetRecentLowestPriceIndex(int Periods, TimeFrame timeFrame)
        {
            int Index = new int();
            double minLow = MarketData.GetBars(timeFrame, SymbolName).LowPrices.Last(1);
            for (int i = 0; i < Periods * 2; i++)
            {
                if (minLow > MarketData.GetBars(timeFrame, SymbolName).LowPrices.Last(i))
                {
                    minLow = MarketData.GetBars(timeFrame, SymbolName).LowPrices.Last(i);
                    DateTime dateTime = MarketData.GetBars(timeFrame, SymbolName).OpenTimes.Last(i);
                    //minLow
                    Index = Bars.OpenTimes.GetIndexByTime(dateTime);
                }
            }
            //double minLow = MarketData.GetBars(timeFrame, SymbolName).LowPrices.Minimum(Periods);
            return Index;
        }

        /// <summary>
        /// Check if tenkanSen-KijunSen crossing is bearish
        /// That is the TenkanSen is below KijunSen
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>bool</returns>
        private bool IsSenBearish(int Value = 1)
        {
            int newValue = Value + 1;
            bool r = ((IchimokuKinkoHyo.TenkanSen.Last(Value) < IchimokuKinkoHyo.KijunSen.Last(Value)) == true) && ((IchimokuKinkoHyo.TenkanSen.IsFalling() == true) || IchimokuKinkoHyo.TenkanSen.Last(newValue) == IchimokuKinkoHyo.TenkanSen.Last(newValue + 1) == true) == true;
            bool EqualAndFalling = (IchimokuKinkoHyo.TenkanSen.Last(Value) == IchimokuKinkoHyo.KijunSen.Last(Value) == true) && (IchimokuKinkoHyo.TenkanSen.IsFalling() == true);
            bool StrongBear = r == true && IchimokuKinkoHyo.ChikouSpan.IsFalling() == true;
            return (r == true) || (EqualAndFalling == true) || (StrongBear == true);
        }
        /// <summary>
        /// Check if tenkanSen-KijunSen crossing is Bullish
        /// That is TenkanSen is above KijunSen
        /// </summary>
        /// <returns>bool</returns>
        /// <param name="Value"></param>
        private bool IsSenBullish(int Value = 1)
        {
            bool r = ((IchimokuKinkoHyo.TenkanSen.Last(Value) > IchimokuKinkoHyo.KijunSen.Last(Value)) == true) && (IchimokuKinkoHyo.TenkanSen.IsRising() == true || (IchimokuKinkoHyo.TenkanSen.Last(Value + 1) == IchimokuKinkoHyo.TenkanSen.Last(Value + 2)) == true) == true;
            bool EqualAndRising = (IchimokuKinkoHyo.TenkanSen.Last(Value) == IchimokuKinkoHyo.KijunSen.Last(Value) == true) && (IchimokuKinkoHyo.TenkanSen.IsRising() == true);
            bool StrongBull = r == true && IchimokuKinkoHyo.ChikouSpan.IsRising() == true;
            return (r == true) || (EqualAndRising == true) || (StrongBull == true);
        }
        /// <summary>
        /// Check to see if Chikou Span has crossed price below.
        /// This is used to confirm a weak bearish signal
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        private bool BearishChikouPriceCrossing(int Value = 1)
        {
            return IchimokuKinkoHyo.ChikouSpan.Last(Value) < Bars.ClosePrices.Last(KijunSenPeriods);

        }
        /// <summary>
        /// Check to see if Chikou Span has crossed the price to the upside
        /// This is used to confirm a weak bullish signal
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        private bool BullishChikouPriceCrossing(int Value = 1)
        {
            return IchimokuKinkoHyo.ChikouSpan.Last(Value) > Bars.ClosePrices.Last(KijunSenPeriods);
        }
        #endregion
    }
}
