using Backdrop.Properties;
using System.Diagnostics;

namespace Backdrop
{
    public partial class Form1 : Form
    {
        private bool isFullscreen = false;
        private bool helpVisible = false;
        double hue = 180d;
        double saturation = 0.5d;
        double brightness = 0.5d;
        double brightnessStep = 0.01d;
        double saturationStep = 0.01d;
        double hueStep = 1d;
        float ColorR = 0f;
        float ColorG = 0f;
        float ColorB = 0f;
        bool updateColor = false;

        public Form1()
        {
            InitializeComponent();
            Color pC = LoadColorPreset(0);
            SetColorValues(pC);
            SetBackColor(hue, saturation, brightness);
            UpdateColorInfo();
        }

        private void Fullscreen()
        {
            isFullscreen = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            panelHelp.Visible = false;
        }

        private void NormalScreen()
        {
            isFullscreen = false;
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*
            Debug.WriteLine("Key pressed: " + e.KeyChar);
            if (e.KeyChar == (char)Keys.Up)
            {
                brightness = Math.Min(brightness + brightnessStep, 1d);
                updateColor = true;
            }
            if (e.KeyChar == (char)Keys.Down)
            {
                brightness = Math.Max(brightness - brightnessStep, 0d);
                updateColor = true;
            }*/
        }

        private void ToggleFullScreen()
        {
            if (isFullscreen)
            {
                NormalScreen();
            }
            else
            {
                Fullscreen();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("Key pressed: " + e.KeyCode);
            if (e.KeyCode == Keys.F1)
            {
                HelpToggle();
            }
            if (e.KeyCode == Keys.F2)
            {
                ColoInfoToggle();
            }
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullScreen();
            }
            if (e.KeyCode == Keys.Escape)
            {
                NormalScreen();
            }
            if (e.KeyCode == Keys.Q)
            {
                this.Close();
            }

            if (e.KeyCode == Keys.Up)
            {
                brightness = Math.Min(brightness + brightnessStep, 1d);
                updateColor = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                brightness = Math.Max(brightness - brightnessStep, 0d);
                updateColor = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                hue += hueStep;
                if (hue > 355d) hue = 0;
                updateColor = true;
            }
            if (e.KeyCode == Keys.Left)
            {
                hue -= hueStep;
                if (hue < 0d) hue = 355d;
                updateColor = true;
            }

            if (e.KeyCode == Keys.OemPeriod)
            {
                saturation = Math.Min(saturation + saturationStep, 1d);
                updateColor = true;
            }
            if (e.KeyCode == Keys.Oemcomma)
            {
                saturation = Math.Max(saturation - saturationStep, 0d);
                updateColor = true;
            }

            if (NumberFromKey(e) != -1)
            {
                if (e.Modifiers == Keys.Control)
                {
                    SaveColorPreset(NumberFromKey(e), this.BackColor);
                }
                else
                {
                    Color pC = LoadColorPreset(NumberFromKey(e));
                    SetColorValues(pC);
                    updateColor = true;
                }
            }

            if (updateColor)
            {
                SetBackColor(hue, saturation, brightness);
                UpdateColorInfo();
            }
        }

        private int NumberFromKey(KeyEventArgs e)
        {
            int value = -1;
            if (e.KeyValue >= 48 && e.KeyValue <= 57)
                value = e.KeyValue - 48;
            return value;
        }

        private Color LoadColorPreset(int presetNumber)
        {
            Debug.WriteLine("Loading color preset " + presetNumber);
            try
            {
                Color presetColor = (Color)Settings.Default["ColorPreset" + presetNumber];
                return presetColor;
            }
            catch
            {
                Debug.WriteLine("Invalid setting ColorPreset" + presetNumber);
                return Color.Black;
            }
        }

        private void SaveColorPreset(int presetNumber, Color color)
        {
            Debug.WriteLine("Saving color preset " + presetNumber);
            try
            {
                Settings.Default["ColorPreset" + presetNumber] = color;
                Settings.Default.Save();
            }
            catch
            {
                Debug.WriteLine("Invalid setting ColorPreset" + presetNumber);
            }
        }

        /*protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    hue -= hueStep;
                    if (hue < 0d) hue = 355d;
                    updateColor = true;
                    return true;
                case Keys.Right:
                    hue += hueStep;
                    if (hue > 355d) hue = 0;
                    updateColor = true;
                    return true;
                case Keys.Up:
                    brightness = Math.Min(brightness + brightnessStep, 1d);
                    updateColor = true;
                    return true;
                case Keys.Down:
                    brightness = Math.Max(brightness - brightnessStep, 0d);
                    updateColor = true;
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }*/



        private void SetColorHSV(double h, double s, double v)
        {
            hue = h;
            saturation = s;
            brightness = v;
            updateColor = true;
        }

        private void SetColorValues(Color color)
        {
            ColorToHSV(color, out hue, out saturation, out brightness);
            ColorR = color.R;
            ColorG = color.G;
            ColorB = color.B;
        }

        private void SetBackColor(double hue, double saturation, double value)
        {
            Color color = ColorFromHSV(hue, saturation, value);
            this.BackColor = color;
            updateColor = false;
            ColorR = color.R;
            ColorG = color.G;
            ColorB = color.B;
        }

        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            //https://stackoverflow.com/questions/1335426/is-there-a-built-in-c-net-system-api-for-hsv-to-rgb
            //https://en.wikipedia.org/wiki/HSL_and_HSV
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        private void HelpToggle()
        {
            panelHelp.Visible = !panelHelp.Visible;
        }

        private void ColoInfoToggle()
        {
            panelColorInfo.Visible = !panelColorInfo.Visible;
        }

        private void UpdateColorInfo()
        {
            Color textColor = TextColorFromBackColor(this.BackColor, 0.6f);
            labelRGB_R.Text = Math.Round(ColorR, 0).ToString();
            labelRGB_G.Text = Math.Round(ColorG, 0).ToString();
            labelRGB_B.Text = Math.Round(ColorB, 0).ToString();
            labelRGB.ForeColor = textColor;
            labelRGB_R.ForeColor = textColor;
            labelRGB_G.ForeColor = textColor;
            labelRGB_B.ForeColor = textColor;

            labelHSV_H.Text = Math.Round(hue, 0).ToString();
            labelHSV_S.Text = Math.Round(saturation, 2).ToString();
            labelHSV_V.Text = Math.Round(brightness, 2).ToString();
            labelHSV.ForeColor = textColor;
            labelHSV_H.ForeColor = textColor;
            labelHSV_S.ForeColor = textColor;
            labelHSV_V.ForeColor = textColor;
        }

        private Color TextColorFromBackColor(Color backColor, float threshold)
        {
            Color result = Color.Black;
            if (ColorValue(backColor) < threshold)
                result = Color.White;
            return result;
        }

        private float ColorValue(Color color)
        {
            // returns a value of 0-1f based on the total brightness of the input color
            //https://stackoverflow.com/questions/596216/formula-to-determine-perceived-brightness-of-rgb-color
            //https://en.wikipedia.org/wiki/Relative_luminance
            //perceived value (0.2126 * R + 0.7152 * G + 0.0722 * B)
            float pR = 0.2126f;
            float pG = 0.7152f;
            float pB = 0.0722f;
            float result = ((color.R * pR) + (color.G * pG) + (color.B * pB)) / 256f;
            return result;
        }
    }
}