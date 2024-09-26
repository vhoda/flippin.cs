using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ScriptPortal.Vegas;

public enum flipMode
{
    horizontal,
    vertical,
    both,
    none
};

public struct Options
{
    public int targetTrack;
    public List<int> flipPattern;
    public bool disableResample;
};

class EntryPoint
{
    private int videoTracksNum;
    private Dictionary<string, int> projVideoTracks = new Dictionary<string, int>();

    Vegas vegas;

    public void FromVegas(Vegas instance)
    {
        vegas = instance;

        IndexProjectTracks();

        if (videoTracksNum == 0)
        {
            MessageBox.Show("Project doesn't contain video tracks", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Options? options = Prompt.GetOptions(projVideoTracks);
        if (options.HasValue)
        {
            FlipByPattern(options.Value);
        }
    }

    public void IndexProjectTracks()
    {
        foreach (Track track in vegas.Project.Tracks)
        {
            if (track.IsVideo())
            {
                videoTracksNum++;
                projVideoTracks.Add("Track #" + (track.Index + 1).ToString() + (String.IsNullOrWhiteSpace(track.Name) ? ("") : (" | (" + track.Name + ")")), track.Index);
            }
        }
    }

    public void FlipByPattern(Options options)
    {
        Track worktrack = vegas.Project.Tracks[options.targetTrack];

        if (options.disableResample == true)
        {
            foreach (TrackEvent ev in worktrack.Events)
            {
                VideoEvent ve = (VideoEvent)ev;
                ve.ResampleMode = VideoResampleMode.Disable;
            }
        }

        int flippedElements = 0;
        int counter = 0;
        int patternLength = options.flipPattern.Count;

        foreach (TrackEvent ev in worktrack.Events)
        {
            VideoEvent ve = (VideoEvent)ev;
            int patternIndex = counter % patternLength;
            int flipOption = options.flipPattern[patternIndex];
            flipMode mode = GetFlipMode(flipOption);

            if (mode != flipMode.none)
            {
                VideoMotionKeyframe kf = ve.VideoMotion.Keyframes[0];
                VideoMotionVertex tl = kf.TopLeft;
                VideoMotionVertex tr = kf.TopRight;
                VideoMotionVertex bl = kf.BottomLeft;
                VideoMotionVertex br = kf.BottomRight;

                switch (mode)
                {
                    case flipMode.vertical:
                        VideoMotionBounds bv = new VideoMotionBounds(bl, br, tr, tl);
                        ve.VideoMotion.Keyframes[0].Bounds = bv;
                        break;
                    case flipMode.horizontal:
                        VideoMotionBounds bh = new VideoMotionBounds(tr, tl, bl, br);
                        ve.VideoMotion.Keyframes[0].Bounds = bh;
                        break;
                    case flipMode.both:
                        VideoMotionBounds bb = new VideoMotionBounds(br, bl, tl, tr);
                        ve.VideoMotion.Keyframes[0].Bounds = bb;
                        break;
                }

                flippedElements++;
            }

            counter++;
        }

        MessageBox.Show(flippedElements == 0 ? "There was nothing to flip" : string.Format("{1} elements on Track #{0} have been successfully flipped", (options.targetTrack + 1), flippedElements), "All done", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private flipMode GetFlipMode(int option)
    {
        switch (option)
        {
            case 1: return flipMode.none;
            case 2: return flipMode.horizontal;
            case 3: return flipMode.vertical;
            case 4: return flipMode.both;
            default: return flipMode.none;
        }
    }
}

class Prompt
{
    public static Options? GetOptions(Dictionary<string, int> videoTracks)
    {
        Form prompt = new Form()
        {
            Width = 480,
            Height = 250,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            MaximizeBox = false,
            MinimizeBox = false,
            ShowIcon = false,
            Text = "vhoda's automatic flipping with patterns",
            AutoScaleMode = AutoScaleMode.Dpi // Escalado automático para DPI
        };

        // Obtener DPI actual
        using (Graphics g = prompt.CreateGraphics())
        {
            float dpiX = g.DpiX;
            float dpiFactor = dpiX / 96.0f; // 96 es el DPI estándar en Windows

            // Ajustar tamaño del formulario
            prompt.Width = (int)(prompt.Width * dpiFactor);
            prompt.Height = (int)(prompt.Height * dpiFactor);
        }

        // Crear un TableLayoutPanel para organizar los controles
        TableLayoutPanel layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            AutoSize = true
        };
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // 70% del ancho para la primera columna
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // 30% del ancho para la segunda columna

        Label instructions = new Label
        {
            Text = "Where do you want the flipping to happen?",
            Dock = DockStyle.Fill,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft
        };

        ComboBox inputValue = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        inputValue.BeginUpdate();
        inputValue.DataSource = new BindingSource(videoTracks, null);
        inputValue.DisplayMember = "Key";
        inputValue.ValueMember = "Value";
        inputValue.EndUpdate();

        Label patternLabel = new Label
        {
            Text = "Enter the flip pattern (e.g. 1 2 3 4): \n 1: Getting to \n 2: Horizontal flip \n 3: Vertical flip \n 4: Both flips(Arps)",
            Dock = DockStyle.Fill,
            AutoSize = true
        };

        TextBox patternInput = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = "1 2"
        };

        CheckBox resample = new CheckBox
        {
            Text = "Disable resample for all clips",
            Checked = true,
            Dock = DockStyle.Fill,
            AutoSize = true
        };

        Button confirmation = new Button
        {
            Text = "Flip the clips",
            Dock = DockStyle.Fill,
            DialogResult = DialogResult.OK
        };

        // Añadir controles al layout
        layoutPanel.Controls.Add(instructions, 0, 0);
        layoutPanel.SetColumnSpan(instructions, 2); // Que las instrucciones ocupen ambas columnas
        layoutPanel.Controls.Add(inputValue, 0, 1);
        layoutPanel.SetColumnSpan(inputValue, 2); // Que el ComboBox ocupe ambas columnas
        layoutPanel.Controls.Add(patternLabel, 0, 2);
        layoutPanel.Controls.Add(patternInput, 1, 2);
        layoutPanel.Controls.Add(resample, 0, 3);
        layoutPanel.SetColumnSpan(resample, 2); // Que el CheckBox ocupe ambas columnas
        layoutPanel.Controls.Add(confirmation, 1, 4);

        // Agregar el layout al formulario
        prompt.Controls.Add(layoutPanel);

        if (prompt.ShowDialog() == DialogResult.OK)
        {
            List<int> flipclips = new List<int>();
            foreach (var val in patternInput.Text.Split(' '))
            {
                int patternVal;
                if (int.TryParse(val, out patternVal) && patternVal >= 1 && patternVal <= 4)
                {
                    flipclips.Add(patternVal);
                }
            }

            return new Options
            {
                targetTrack = videoTracks[inputValue.Text],
                flipPattern = flipclips,
                disableResample = resample.Checked
            };
        }
        else
        {
            return null;
        }
    }
}
