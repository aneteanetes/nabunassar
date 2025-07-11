﻿// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Effect
{
    public class MonogameEffectCompilerOptions
    {
        [CommandLineParser.Required]
        public string SourceFile;

        [CommandLineParser.Required]
        [CommandLineParser.Name("OutputFile", "\t - The output file path.  Use a .h extension to generate a C header file.")]
        public string OutputFile = string.Empty;

        [CommandLineParser.ProfileName]
        public ShaderProfile Profile = ShaderProfile.OpenGL;

        [CommandLineParser.Name("Debug", "\t\t - Include extra debug information in the compiled effect.")]
        public bool Debug;

        [CommandLineParser.Name("Defines", "\t - Semicolon-delimited define assignments")]
        public string Defines;
    }
}
