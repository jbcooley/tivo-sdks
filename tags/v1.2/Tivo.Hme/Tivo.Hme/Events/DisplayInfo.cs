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
using System.Collections.Generic;
using System.Text;
using Tivo.Hme.Host;

namespace Tivo.Hme.Events
{
    class DisplayInfo : EventInfo
    {
        public const long Type = 8;
        private const long _rootStreamId = 1;
        private long _metricsPerResolutionInfo;
        private ResolutionInfo _currentResolution;
        private List<ResolutionInfo> _supportedResolutions = new List<ResolutionInfo>();

        public ResolutionInfo CurrentResolution
        {
            get { return _currentResolution; }
        }

        public List<ResolutionInfo> SupportedResolutions
        {
            get { return _supportedResolutions; }
        }

        public override void Read(HmeReader reader)
        {
            long streamId = reader.ReadInt64();
            System.Diagnostics.Debug.Assert(_rootStreamId == streamId);
            _metricsPerResolutionInfo = reader.ReadInt64();
            _currentResolution = new ResolutionInfo(reader.ReadInt64(),
                reader.ReadInt64(), reader.ReadInt64(), reader.ReadInt64());
            SkipExtraResolutionMetrics(reader);
            long count = reader.ReadInt64();
            for (int i = 0; i < count; ++i)
            {
                _supportedResolutions.Add(new ResolutionInfo(
                    reader.ReadInt64(), reader.ReadInt64(), reader.ReadInt64(), reader.ReadInt64()));
                SkipExtraResolutionMetrics(reader);
            }
            reader.ReadTerminator();
        }

        private void SkipExtraResolutionMetrics(HmeReader reader)
        {
            for (int i = ResolutionInfo.FieldCount; i < _metricsPerResolutionInfo; ++i)
            {
                reader.ReadInt64();
            }
        }

        public override void RaiseEvent(Application application)
        {
            application.OnDisplayInfoReceived(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().Name);
            builder.AppendFormat(": (MetricsPerResolutionInfo,{0})(CurrentResolution,{1}) ", _metricsPerResolutionInfo, CurrentResolution);
            int index = 0;
            foreach (ResolutionInfo entry in _supportedResolutions)
            {
                builder.AppendFormat("(SupportedResolution[{0}],{1})", index++, entry);
            }
            return builder.ToString();
        }
    }
}
