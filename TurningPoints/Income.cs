using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TurningPoints.Enums;

namespace ATAS.Indicators.Technical
{
    ///// <summary>
    ///// Типы разворотных сигналов
    ///// </summary>
    //public enum ReversalSignals
    //{
    //    WM, normal, TrendBreakout
    //}

    [DisplayName("Income")]
    public class Income : Indicator
    {

        #region Filds

        private readonly RangeDataSeries _band = new RangeDataSeries("Background");
        private readonly StdDev _dev = new StdDev();
        private readonly SMA _sma = new SMA();
        private decimal _width;

        private Color _signalWM_Up_Color;
        private bool _isSignalWM_Up_Visible;
        private Color _signalWM_Down_Color;
        private bool _isSignalWM_Down_Visible;
        private Color _signalNormal_Up_Color;
        private bool _isSignalNormal_Up_Visible;
        private Color _signalNormal_Down_Color;
        private bool _isSignalNormal_Down_Visible;
        private Color _signalBreakout_Up_Color;
        private bool _isSignalBreakout_Up_Visible;
        private Color _signalBreakout_Down_Color;
        private bool _isSignalBreakout_Down_Visible;

        //private ValueDataSeries _signalWM_Up;
        //private ValueDataSeries _signalWM_Down;

        //private ValueDataSeries _signalNormal_Up;
        //private ValueDataSeries _signalNormal_Down;

        //private ValueDataSeries _signalBreakout_Up;
        //private ValueDataSeries _signalBreakout_Down;

        #endregion

        #region Properties

        [Display(Name = "Period", GroupName = "Common", Order = 20)]
        public int Period
        {
            get => _sma.Period;
            set
            {
                if (value <= 0)
                    return;

                _sma.Period = _dev.Period = value;
                RecalculateValues();
            }
        }

        [Display(Name = "BBandsWidth", GroupName = "Common", Order = 22)]
        public decimal Width
        {
            get => _width;
            set
            {
                if (value <= 0)
                    return;

                _width = value;
                RecalculateValues();
            }
        }

        #region Signal WM 

        [Display(Name = "Arrow Up Color", GroupName = "Signal WM")]
        public Color SignalWM_Up_Color
        {
            get => _signalWM_Up_Color;
            set => SetColorValue(ref _signalWM_Up_Color, value);
        }


        [Display(Name = "Arrow Up Visibility", GroupName = "Signal WM")]
        public bool IsSignalWM_Up_Visible
        {
            get => _isSignalWM_Up_Visible;
            set => SetVisibilityValue(ref _isSignalWM_Up_Visible, value);
        }


        [Display(Name = "Arrow Down Color", GroupName = "Signal WM")]
        public Color SignalWM_Down_Color
        {
            get => _signalWM_Down_Color;
            set => SetColorValue(ref _signalWM_Down_Color, value);
        }


        [Display(Name = "Arrow Down Visibility", GroupName = "Signal WM")]
        public bool IsSignalWM_Down_Visible
        {
            get => _isSignalWM_Down_Visible;
            set => SetVisibilityValue(ref _isSignalWM_Down_Visible, value);
        }

        #endregion

        #region Signal Normal 

        [Display(Name = "Arrow Up Color", GroupName = "Signal Normal")]
        public Color SignalNormal_Up_Color
        {
            get => _signalNormal_Up_Color;
            set => SetColorValue(ref _signalNormal_Up_Color, value);
        }


        [Display(Name = "Arrow Up Visibility", GroupName = "Signal Normal")]
        public bool IsSignalNormal_Up_Visible
        {
            get => _isSignalNormal_Up_Visible;
            set => SetVisibilityValue(ref _isSignalNormal_Up_Visible, value);
        }


        [Display(Name = "Arrow Down Color", GroupName = "Signal Normal")]
        public Color SignalNormal_Down_Color
        {
            get => _signalNormal_Down_Color;
            set => SetColorValue(ref _signalNormal_Down_Color, value);
        }


        [Display(Name = "Arrow Down Visibility", GroupName = "Signal Normal")]
        public bool IsSignalNormal_Down_Visible
        {
            get => _isSignalNormal_Down_Visible;
            set => SetVisibilityValue(ref _isSignalNormal_Down_Visible, value);
        }

        #endregion

        #region Signal Breakout

        [Display(Name = "Arrow Up Color", GroupName = "Signal Breakout")]
        public Color SignalBreakout_Up_Color
        {
            get => _signalBreakout_Up_Color;
            set => SetColorValue(ref _signalBreakout_Up_Color, value);
        }


        [Display(Name = "Arrow Up Visibility", GroupName = "Signal Breakout")]
        public bool IsSignalBreakout_Up_Visible
        {
            get => _isSignalBreakout_Up_Visible;
            set => SetVisibilityValue(ref _isSignalBreakout_Up_Visible, value);
        }


        [Display(Name = "Arrow Down Color", GroupName = "Signal Breakout")]
        public Color SignalBreakout_Down_Color
        {
            get => _signalBreakout_Down_Color;
            set => SetColorValue(ref _signalBreakout_Down_Color, value);
        }


        [Display(Name = "Arrow Down Visibility", GroupName = "Signal Breakout")]
        public bool IsSignalBreakout_Down_Visible 
        {
            get => _isSignalBreakout_Down_Visible;
            set => SetVisibilityValue(ref _isSignalBreakout_Down_Visible, value);
        }
        #endregion
        #endregion

        #region ctor
        public Income()
        {
            ((ValueDataSeries)DataSeries[0]).Color = Colors.Green;
            DataSeries[0].Name = "Bollinger Bands";

            DataSeries.Add(new ValueDataSeries("Up")
            {
                VisualType = VisualMode.Line
            });

            DataSeries.Add(new ValueDataSeries("Down")
            {
                VisualType = VisualMode.Line
            });

            DataSeries.Add(_band);
            Period = 10;
            Width = 1;


            SetSignalInDataSet("Reversal Signal WM Up", VisualMode.UpArrow, SignalWM_Up_Color, IsSignalWM_Up_Visible);
            SetSignalInDataSet("Reversal Signal WM Down", VisualMode.DownArrow, SignalWM_Down_Color, IsSignalWM_Down_Visible);

            SetSignalInDataSet("Reversal Signal Normal Up", VisualMode.UpArrow, SignalNormal_Up_Color, IsSignalNormal_Up_Visible);
            SetSignalInDataSet("Reversal Signal Normal Down", VisualMode.DownArrow, SignalNormal_Down_Color, IsSignalNormal_Down_Visible);

            SetSignalInDataSet("Reversal Signal Breakout Up", VisualMode.UpArrow, SignalBreakout_Up_Color, IsSignalBreakout_Up_Visible);
            SetSignalInDataSet("Reversal Signal Breakout Down", VisualMode.DownArrow, SignalBreakout_Down_Color, IsSignalBreakout_Down_Visible);
        }


        private void SetSignalInDataSet(string name, VisualMode mode, Color color, bool isVisible)
        {
            var signal = new ValueDataSeries(name)
            {
                VisualType = mode,
                Width = 10,
                IsHidden = !isVisible,
                Color = color
            };
            DataSeries.Add(signal);
        }
        #endregion

        #region Protected methods

        protected override void OnCalculate(int bar, decimal value)
        {

            var sma = _sma.Calculate(bar, value);
            var dev = _dev.Calculate(bar, value);

            this[bar] = sma;

            DataSeries[1][bar] = _band[bar].Upper = sma + dev * Width;
            DataSeries[2][bar] = _band[bar].Lower = sma - dev * Width;

           

        }

        #endregion

        private void SetColorValue(ref Color signalColor, Color value)
        {
            signalColor = value;
            RecalculateValues();
        }

        private void SetVisibilityValue(ref bool isVisible, bool value)
        {
            isVisible = value;
            RecalculateValues();
        }

    }
}
