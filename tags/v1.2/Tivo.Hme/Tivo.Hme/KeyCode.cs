// Copyright (c) 2008 Josh Cooley

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace Tivo.Hme
{
    /// <summary>
    /// Represents the key pressed on a remote.
    /// </summary>
    public enum KeyCode : long
    {
        /// <summary>
        /// key not known
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// TiVo® or equivalent (reserved)
        /// </summary>
        Tivo = 1,
        /// <summary>
        /// arrow up
        /// </summary>
        Up = 2,
        /// <summary>
        /// arrow down
        /// </summary>
        Down = 3,
        /// <summary>
        /// arrow left
        /// </summary>
        Left = 4,
        /// <summary>
        /// arrow right
        /// </summary>
        Right = 5,
        /// <summary>
        /// select
        /// </summary>
        Select = 6,
        /// <summary>
        /// play
        /// </summary>
        Play = 7,
        /// <summary>
        /// pause
        /// </summary>
        Pause = 8,
        /// <summary>
        /// play slowly
        /// </summary>
        Slow = 9,
        /// <summary>
        /// reverse
        /// </summary>
        Reverse = 10,
        /// <summary>
        /// fast forward
        /// </summary>
        Forward = 11,
        /// <summary>
        /// instant replay
        /// </summary>
        Replay = 12,
        /// <summary>
        /// advance to the next marker
        /// </summary>
        Advance = 13,
        /// <summary>
        /// thumbs up
        /// </summary>
        ThumbsUp = 14,
        /// <summary>
        /// thumbs down
        /// </summary>
        ThumbsDown = 15,
        /// <summary>
        /// volume up
        /// </summary>
        VolumeUp = 16,
        /// <summary>
        /// volume down
        /// </summary>
        VolumeDown = 17,
        /// <summary>
        /// channel up
        /// </summary>
        ChannelUp = 18,
        /// <summary>
        /// channel down
        /// </summary>
        ChannelDown = 19,
        /// <summary>
        /// mute
        /// </summary>
        Mute = 20,
        /// <summary>
        /// record
        /// </summary>
        Record = 21,
        /// <summary>
        /// back to live TV
        /// </summary>
        LiveTv = 23,
        /// <summary>
        /// info
        /// </summary>
        Info = 25,
        /// <summary>
        /// display, same as info
        /// </summary>
        Display = Info,
        /// <summary>
        /// clear
        /// </summary>
        Clear = 28,
        /// <summary>
        /// enter
        /// </summary>
        Enter = 29,
        /// <summary>
        /// 0
        /// </summary>
        Num0 = 40,
        /// <summary>
        /// 1
        /// </summary>
        Num1 = 41,
        /// <summary>
        /// 2
        /// </summary>
        Num2 = 42,
        /// <summary>
        /// 3
        /// </summary>
        Num3 = 43,
        /// <summary>
        /// 4
        /// </summary>
        Num4 = 44,
        /// <summary>
        /// 5
        /// </summary>
        Num5 = 45,
        /// <summary>
        /// 6
        /// </summary>
        Num6 = 46,
        /// <summary>
        /// 7
        /// </summary>
        Num7 = 47,
        /// <summary>
        /// 8
        /// </summary>
        Num8 = 48,
        /// <summary>
        /// 9
        /// </summary>
        Num9 = 49,

        // Optional key support below
        /// <summary>
        /// window
        /// </summary>
        Window = 22,
        /// <summary>
        /// picture in picture, same as window
        /// </summary>
        Pip = Window,
        /// <summary>
        /// aspect, same as window
        /// </summary>
        Aspect = Window,
        /// <summary>
        /// exit (reserved)
        /// </summary>
        Exit = 24,
        /// <summary>
        /// list now playing (reserved)
        /// </summary>
        List = 26,
        /// <summary>
        /// guide (reserved)
        /// </summary>
        Guide = 27,
        /// <summary>
        /// stop
        /// </summary>
        Stop = 51,
        /// <summary>
        /// dvd menu
        /// </summary>
        Menu = 52,
        /// <summary>
        /// dvd top menu
        /// </summary>
        TopMenu = 53,
        /// <summary>
        /// angle
        /// </summary>
        Angle = 54,
        /// <summary>
        /// dvd (reserved)
        /// </summary>
        Dvd = 55
    }
}
