using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace _Emulator
{

    public static class VersionExtension
    {
        public static int CompareVersion(this string version, string otherVersion)
        {
            return version.ParseVersion().CompareTo(otherVersion.ParseVersion());
        }
        public static Version ParseVersion(this string value)
        {
            if (value.StartsWith("v"))
            {
                value = value.Substring(1);
            }
            int[] components = new int[]
            {
                -1,
                -1,
                -1,
                -1
            };
            StringBuilder builder = new StringBuilder();
            char ch;
            int component = 0;
            bool minusPreviously = false;
            for (int i = 0; i < value.Length; i++)
            {
                ch = value[i];
                if (char.IsDigit(ch))
                {
                    builder.Append(ch);
                    minusPreviously = false;
                    continue;
                }
                if (ch != '.' && ch != '-')
                {
                    if (minusPreviously && ch == 'R')
                    {
                        minusPreviously = false;
                        continue;
                    }
                    throw new ArgumentException($"Unsupported character '{ch}' found at index {i}");
                }
                if (component == 4)
                {
                    throw new ArgumentException("Too many components");
                }
                if (!int.TryParse(builder.ToString(), out int componentValue))
                {
                    throw new ArgumentException($"Failed to parse number component at index {i}: '{builder.ToString()}'");
                }
                components[component] = componentValue;
                builder.Length = 0;
                builder.Capacity = 0;
                if (ch == '.')
                {
                    component++;
                }
                else if (ch == '-')
                {
                    component = 3;
                    minusPreviously = true;
                }
            }
            if (builder.Length > 0)
            {
                if (!int.TryParse(builder.ToString(), out int componentValue))
                {
                    throw new ArgumentException($"Failed to parse number component at index {value.Length - 1}: '{builder.ToString()}'");
                }
                components[component] = componentValue;
            }
            return new Version(components[0], components[1], components[2], components[3]);
        }

        public static Version ToVersion(this int[] components)
        {
            if (components.Length > 4)
            {
                throw new ArgumentException("Too many components");
            }
            int major = -1, minor = -1, patch = -1, revision = -1;
            if (components.Length > 0)
            {
                major = components[0];
            }
            if (components.Length > 1)
            {
                minor = components[1];
            }
            if (components.Length > 2)
            {
                patch = components[2];
            }
            if (components.Length > 3)
            {
                revision = components[3];
            }
            return new Version(major, minor, patch, revision);
        }
    }

    public class Version : IComparable<Version>
    {

        public static readonly Version UNKNOWN = new Version(-1, -1, -1, -1);

        public static Version Parse(string value) 
        {
            return value.ParseVersion();
        }

        public static Version From(int[] components)
        {
            return components.ToVersion();
        }

        public static int Compare(string version, string otherVersion)
        {
            return version.CompareVersion(otherVersion);
        }

        public int Major => major;
        public int Minor => minor;
        public int Patch => patch;
        public int Revision => revision;

        private readonly int major, minor, patch, revision;

        public Version(int major, int minor, int patch) : this(major, minor, patch, -1) { }

        public Version(int major, int minor, int patch, int revision)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
            this.revision = revision;
        }

        public Version Add(int major, int minor, int patch)
        {
            return Add(major, minor, patch, 0);
        }

        public Version Add(int major, int minor, int patch, int revision)
        {
            return new Version(this.major + major, this.minor + minor, this.patch + patch, this.revision + revision);
        }

        public bool IsHigher(Version version)
        {
            return CompareTo(version) > 0;
        }

        public bool IsSame(Version version)
        {
            return CompareTo(version) == 0;
        }

        public bool IsLower(Version version)
        {
            return CompareTo(version) < 0;
        }

        public int CompareTo(Version other)
        {
            int tmp = major.CompareTo(other.major);
            if (tmp != 0)
            {
                return tmp;
            }
            tmp = minor.CompareTo(other.minor);
            if (tmp != 0)
            {
                return tmp;
            }
            tmp = patch.CompareTo(other.patch);
            if (tmp != 0)
            {
                return tmp;
            }
            return revision.CompareTo(other.revision);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(major).Append('.').Append(minor);
            if (patch > -1)
            {
                builder.Append('.').Append(patch);
            }
            if (revision > -1)
            {
                builder.Append("-R").Append(revision);
            }
            return builder.ToString();
        }
    }
}
