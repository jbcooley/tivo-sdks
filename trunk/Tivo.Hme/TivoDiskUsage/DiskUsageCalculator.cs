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
using System.Linq;
using Tivo.Hmo;

namespace TivoDiskUsage
{
    class DiskUsageCalculator
    {
        Dictionary<string, ulong> _categoryTotals = new Dictionary<string, ulong>();

        public const string Current = "Current";
        public const string Free = "Estimated Free";
        public const string InProgress = "In Progress";
        public const string SaveUntilIDelete = "Save Until I Delete";
        public const string ExpiresSoon = "Expires Soon";
        public const string Expired = "Expired";
        public const string Suggestion = "Suggestion";

        private DiskUsageCalculator()
        {
        }

        public static DiskUsageCalculator Calculate(TivoContainer container, string tivoName)
        {
            return Calculate(new TivoContainer[] { container }, tivoName);
        }

        public static DiskUsageCalculator Calculate(IEnumerable<TivoContainer> containers, string tivoName)
        {
            DiskUsageCalculator calculator = new DiskUsageCalculator();
            calculator.CurrentSpaceUsed = calculator.CalculateCategoryTotals(containers);
            calculator.MaxSpaceUsed = calculator.GetMaxSpaceUsedForTivo(calculator.CurrentSpaceUsed, tivoName);

            // decrement this value with each category found
            ulong currentRecordings = calculator.CurrentSpaceUsed;
            foreach (KeyValuePair<string, ulong> entry in calculator._categoryTotals)
            {
                currentRecordings -= entry.Value;
            }
            calculator._categoryTotals.Add(Current, currentRecordings);

            ulong estimatedFree = calculator.MaxSpaceUsed - calculator.CurrentSpaceUsed;
            calculator._categoryTotals.Add(Free, estimatedFree);
            return calculator;
        }

        public ulong CurrentSpaceUsed { get; private set; }
        public ulong MaxSpaceUsed { get; private set; }
        public ulong this[string categoryName] { get { return _categoryTotals[categoryName]; } }
        public int CategoryCount { get { return _categoryTotals.Count; } }

        /// <summary>
        /// Categories sorted in order of most likely to keep to least likely to keep
        /// </summary>
        public IEnumerable<string> Categories
        {
            get
            {
                List<string> categories = new List<string>(_categoryTotals.Keys);
                categories.Sort(delegate(string left, string right)
                {
                    int leftNum = RankCategory(left);
                    int rightNum = RankCategory(right);
                    if (leftNum == -1 && rightNum == -1)
                        return StringComparer.CurrentCulture.Compare(left, right);
                    else if (leftNum == -1)
                        return 1;
                    else if (rightNum == -1)
                        return -1;
                    else
                        return leftNum - rightNum;
                });
                return categories;
            }
        }

        private static int RankCategory(string name)
        {
            switch (name)
            {
                case InProgress:
                    return 1;
                case SaveUntilIDelete:
                    return 2;
                case Current:
                    return 3;
                case ExpiresSoon:
                    return 4;
                case Expired:
                    return 5;
                case Suggestion:
                    return 6;
                case Free:
                    return 7;
                default:
                    return -1;
            }
        }

        private ulong CalculateCategoryTotals(IEnumerable<TivoContainer> containers)
        {
            ulong total = 0;
            foreach (TivoVideo video in containers.SelectMany(cs => cs.TivoItems).Where(item => item is TivoVideo))
            {
                total += (ulong)video.Length;
                if (video.CustomIcon != null)
                {
                    string categoryName = GetPrettyName(video.CustomIcon.Uri.AbsoluteUri);
                    if (_categoryTotals.ContainsKey(categoryName))
                        _categoryTotals[categoryName] += (ulong)video.Length;
                    else
                        _categoryTotals.Add(categoryName, (ulong)video.Length);
                }
            }
            return total;
        }

        private ulong GetMaxSpaceUsedForTivo(ulong total, string tivoName)
        {
            if (Properties.Settings.Default.TivoNames == null)
                Properties.Settings.Default.TivoNames = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.MaximumSpaceUsed == null)
                Properties.Settings.Default.MaximumSpaceUsed = new System.Collections.Specialized.StringCollection();
            // get estimated free space
            ulong maxSpaceUsed = total;
            int tivoNameIndex = Properties.Settings.Default.TivoNames.IndexOf(tivoName);
            if (tivoNameIndex != -1)
            {
                maxSpaceUsed = Math.Max(Convert.ToUInt64(Properties.Settings.Default.MaximumSpaceUsed[tivoNameIndex]), maxSpaceUsed);
                Properties.Settings.Default.MaximumSpaceUsed[tivoNameIndex] = maxSpaceUsed.ToString();
            }
            else
            {
                Properties.Settings.Default.TivoNames.Add(tivoName);
                Properties.Settings.Default.MaximumSpaceUsed.Add(maxSpaceUsed.ToString());
            }
            Properties.Settings.Default.Save();
            return maxSpaceUsed;
        }

        private static readonly int prefixLength = "urn:tivo:image:".Length;
        private static readonly int suffixLength = "-recording".Length;

        private static string GetPrettyName(string categoryName)
        {
            StringBuilder labelBuilder = new StringBuilder();
            bool capitalize = true;
            foreach (char letter in categoryName.Substring(prefixLength, categoryName.Length - prefixLength - suffixLength).Replace('-', ' '))
            {
                if (!char.IsWhiteSpace(letter) && capitalize)
                {
                    labelBuilder.Append(char.ToUpper(letter));
                    capitalize = false;
                }
                else
                {
                    capitalize = char.IsWhiteSpace(letter);
                    labelBuilder.Append(letter);
                }
            }
            return labelBuilder.ToString();
        }
    }
}
