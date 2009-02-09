using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Tivo.Hme.Samples
{
    [Tivo.Hme.Host.Services.UsesHostHttpServices]
    class Music : HmeApplicationHandler
    {
        static List<string> _fileNames = new List<string>();

        // list of tracks
        MusicList _musicList;
        Track _track;
        Application _application;
        int _playingIndex = -1;
        DateTime _lastSelectionChange;

        static Music()
        {
            string musicPath = Properties.Settings.Default.MusicPath;
            foreach (string musicFileName in System.IO.Directory.GetFiles(musicPath, "*.mp3", System.IO.SearchOption.AllDirectories))
            {
                _fileNames.Add(musicFileName);
            }
        }

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            _musicList = new MusicList(27);
            _musicList.Bounds = new System.Drawing.Rectangle(64, 40, 640 - 64 * 2, 480 - 150);
            _musicList.SelectionChanged += new EventHandler<EventArgs>(_musicList_SelectionChanged);
            e.Application.Root.Children.Add(_musicList);
            _track = new Track();
            _track.Bounds = new System.Drawing.Rectangle(64, _musicList.Bounds.Y + _musicList.Bounds.Height, 640 - 64 * 2, 60);
            e.Application.Root.Children.Add(_track);

            _musicList.AddRange(_fileNames);
            _musicList.Focus();

            _application = e.Application;
            _application.KeyPress += new EventHandler<KeyEventArgs>(application_KeyPress);
            _application.ResourceStateChanged += new EventHandler<ResourceStateChangedArgs>(application_ResourceStateChanged);
        }

        public override void OnApplicationEnd()
        {
        }

        private string MapUrl(string path)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            return info.Name;
            //return path.Replace(_localPath, string.Empty).Replace('\\', '/');
        }

        void _musicList_SelectionChanged(object sender, EventArgs e)
        {
            _lastSelectionChange = DateTime.UtcNow;
        }

        void application_KeyPress(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case KeyCode.Select:
                case KeyCode.Play:
                    _application.GetSound("select").Play();
                    Play(_playingIndex);
                    break;
                case KeyCode.Pause:
                    _track.PauseTrack();
                    break;
                case KeyCode.Slow:
                    _track.SlowTrack();
                    break;
                case KeyCode.Forward:
                    _track.FastForward();
                    break;
                case KeyCode.Reverse:
                    _track.Rewind();
                    break;
            }
        }

        void application_ResourceStateChanged(object sender, ResourceStateChangedArgs e)
        {
            if (e.Status >= ResourceStatus.Closed)
            {
                // the track finished - what next?
                if (_fileNames.Count == 1)
                {
                    _playingIndex = -1;
                }
                else
                {
                    // advance
                    int index = _playingIndex + 1;
                    if (index == _fileNames.Count)
                        index = 0;

                    // if the user hasn't touched the list recently, move the
                    // selector to reflect the new track.
                    if (DateTime.UtcNow - _lastSelectionChange > TimeSpan.FromSeconds(5))
                    {
                        _musicList.SelectedIndex = index;
                    }
                    else
                    {
                        index = _musicList.SelectedIndex;
                    }

                    // now play the new track
                    Play(index);
                }
            }
        }

        private void Play(int index)
        {
            if (index != _playingIndex)
            {
                _playingIndex = index;

                Uri uri = new Uri(BaseUri, "music/" + MapUrl(_fileNames[_playingIndex]));
                _track.PlayTrack(uri);
            }
        }

        private class MusicList : BaseList<string>
        {
            public MusicList(int rowHeight)
                : base(rowHeight)
            {
            }

            protected override View CreateSelector(int width, int height)
            {
                ColorView selector = new ColorView(System.Drawing.Color.Blue);
                selector.Bounds = new System.Drawing.Rectangle(0, 0, width, height);
                this.Children.Add(selector);
                return selector;
            }

            protected override View CreateRow(int index)
            {
                string path = this[index];
                string rowText = System.IO.Path.GetFileName(path);
                if (rowText.Length > 50)
                    rowText = "..." + rowText.Substring(rowText.Length - 50);
                TextView row = new TextView(rowText,
                    new TextStyle("default", System.Drawing.FontStyle.Regular, 24),
                    System.Drawing.Color.White, TextLayout.HorizontalAlignLeft);
                row.Bounds = new System.Drawing.Rectangle(0, index * RowHeight, Bounds.Width, RowHeight);
                this.Children.Add(row);
                return row;
            }
        }

        private class Track : TextView
        {
            private bool _slowed;
            StreamedMusic _currentTrack;

            public Track()
                : base(string.Empty,
                new TextStyle("default", System.Drawing.FontStyle.Bold, 18),
                System.Drawing.Color.Yellow,
                TextLayout.HorizontalAlignCenter | TextLayout.TextWrap)
            {
            }

            protected override void OnNewApplication()
            {
                base.OnNewApplication();
                Application.ResourceStateChanged += new EventHandler<ResourceStateChangedArgs>(Application_ResourceStateChanged);
            }

            public void PlayTrack(Uri uri)
            {
                if (_currentTrack != null)
                    _currentTrack.Close();
                _currentTrack = Application.GetStreamedMusic(uri, "audio/mp3", MusicStart.AutoPlay);
            }

            public void PauseTrack()
            {
                if (_currentTrack != null)
                    _currentTrack.Pause();
            }

            public void SlowTrack()
            {
                float speed = 1;
                if (_currentTrack != null)
                {
                    if (!_slowed)
                        speed = 0.5F;
                    _currentTrack.Forward(speed);
                    _slowed = !_slowed;
                }
            }

            public bool Slowed
            {
                get { return _slowed; }
            }

            // keeps track of current FF/REW speed
            int _speedIndex = 3;
            float[] _speeds = { -60.0f, -15.0f, -3.0f, 1.0f, 3.0f, 15.0f, 60.0f };

            /// <summary>
            /// Fast forward the current stream
            /// </summary>
            public void FastForward()
            {
                if (_currentTrack != null)
                {
                    ++_speedIndex;
                    switch (_speedIndex)
                    {
                        case 4:
                            Application.GetSound("speedup1").Play();
                            break;
                        case 5:
                            Application.GetSound("speedup2").Play();
                            break;
                        case 6:
                            Application.GetSound("speedup3").Play();
                            break;
                        case 7:
                            // currently going as fast as we can, drop back to play, to mimic video FF behavior
                            _speedIndex = 3;
                            Application.GetSound("slowdown1").Play();
                            break;
                        default:
                            Application.GetSound("slowdown1").Play();
                            break;
                    }
                    _currentTrack.Forward(_speeds[_speedIndex]);
                }
            }

            /// <summary>
            /// rewind the current stream.
            /// </summary>
            public void Rewind()
            {
                if (_currentTrack != null)
                {
                    --_speedIndex;
                    switch (_speedIndex)
                    {
                        case -1:
                            // currently going as fast as we can, drop back to play, to mimic video REW behavior
                            _speedIndex = 3;
                            Application.GetSound("slowdown1").Play();
                            break;
                        case 0:
                            Application.GetSound("speedup3").Play();
                            break;
                        case 1:
                            Application.GetSound("speedup2").Play();
                            break;
                        case 2:
                            Application.GetSound("speedup1").Play();
                            break;
                        default:
                            Application.GetSound("slowdown1").Play();
                            break;
                    }
                    _currentTrack.Reverse(-_speeds[_speedIndex]);
                }
            }

            void Application_ResourceStateChanged(object sender, ResourceStateChangedArgs e)
            {
                // TODO: set the label text to the event contents.
                if (e.Resource != null && e.Resource.Equals(_currentTrack))
                {
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();
                    builder.Append(e.Resource.Name);
                    builder.Append(" .ResourceStateChanged( ");
                    builder.Append(e.Status.ToString());
                    builder.Append(", {");
                    bool first = true;
                    foreach (KeyValuePair<string, string> pair in e.ResourceInfo)
                    {
                        if (!first)
                            builder.Append(", ");
                        builder.Append(pair.Key);
                        builder.Append("=");
                        builder.Append(pair.Value);
                        first = false;
                    }
                    builder.Append("} )");
                    Text = builder.ToString();
                }
            }
        }
    }
}
