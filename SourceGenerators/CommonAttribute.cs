using System;

namespace Godot
{
    public enum OutputType
    {
        ROSLYN,
        REAL,
    }
    public class CommonAttribute(OutputType outputType = OutputType.ROSLYN, string outputDir = "") : Attribute
    {
        public OutputType OutputType { get; } = outputType;
        public string OutputDir { get; } = outputDir;
    }
}
