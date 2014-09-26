namespace Jwc.CIBuildTasks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a MSBuild task to extract release notes from AssemblyInfo.
    /// </summary>
    public class ReleaseNoteExtractor : Task
    {
        private string assemblyInfo;
        private string releaseNotes;
        private string xmlEscapedReleaseNotes;

        /// <summary>
        /// Gets or sets the file path of the assembly information.
        /// </summary>
        [Required]
        public string AssemblyInfo
        {
            get
            {
                return this.assemblyInfo;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.assemblyInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the release notes.
        /// </summary>
        [Output]
        public string ReleaseNotes
        {
            get
            {
                return this.releaseNotes;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.releaseNotes = value;
            }
        }

        /// <summary>
        /// Gets or sets the XML escaped release notes.
        /// </summary>
        [Output]
        public string XmlEscapedReleaseNotes
        {
            get
            {
                return this.xmlEscapedReleaseNotes;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.xmlEscapedReleaseNotes = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            this.SetReleaseNotes();
            this.SetXmlEscapedReleaseNotes();

            return true;
        }

        private static IEnumerable<string> GetReleaseNoteLines(string content)
        {
            return content.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries)
                .SkipWhile(l => l.TrimStart(' ', '*').Length == 0)
                .Reverse().SkipWhile(l => l.TrimStart(' ', '*').Length == 0).Reverse();
        }

        private static int GetIndentation(IEnumerable<string> lines)
        {
            var firstLine = lines.First();
            var indentation = 0;
            foreach (var ch in firstLine)
            {
                if (ch == '*')
                {
                    indentation++;
                }
                else if (ch == ' ')
                {
                    indentation++;
                }
                else
                {
                    break;
                }
            }

            return indentation;
        }

        private void SetReleaseNotes()
        {
            var content = File.ReadAllText(this.AssemblyInfo, Encoding.UTF8);
            var match = Regex.Match(content, @"/\*([\S\s]*?)\*/");
            if (!match.Success)
            {
                this.ReleaseNotes = string.Empty;
                return;
            }

            var targetContent = match.Result("$1");
            var lines = GetReleaseNoteLines(targetContent);
            if (!lines.Any())
            {
                this.ReleaseNotes = string.Empty;
                return;
            }

            this.SetReleaseNotes(lines);
        }

        private void SetReleaseNotes(IEnumerable<string> lines)
        {
            var indentation = GetIndentation(lines);
            var builder = new StringBuilder();
            foreach (var line in lines)
                builder.AppendLine(new string(line.Skip(indentation).ToArray()).TrimEnd());

            builder.Remove(
                builder.Length - Environment.NewLine.Length,
                Environment.NewLine.Length);
            this.ReleaseNotes = builder.ToString();
        }

        private void SetXmlEscapedReleaseNotes()
        {
            var result = new StringBuilder();
            using (var stringWriter = new StringWriter(result, CultureInfo.CurrentCulture))
            {
                var xmlWriter = new XmlTextWriter(stringWriter);
                xmlWriter.WriteString(this.ReleaseNotes);
                this.XmlEscapedReleaseNotes = result.ToString();
            }
        }
    }
}