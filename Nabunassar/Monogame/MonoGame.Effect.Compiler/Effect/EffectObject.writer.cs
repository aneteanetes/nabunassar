// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Utilities;

namespace MonoGame.Effect
{
	internal partial class EffectObject
	{

        private const string Header = "MGFX";
        internal const int Version = 10;
        
        static int ComputeHash(Stream stream)
        {
            System.Diagnostics.Debug.Assert(stream.CanSeek);

            unchecked
            {
                const int p = 16777619;
                var hash = (int)2166136261;

                var prevPosition = stream.Position;
                stream.Position = 0;

                var data = new byte[1024];
                int length;
                while((length = stream.Read(data, 0, data.Length)) != 0)
                {
                    for (var i = 0; i < length; i++)
                        hash = (hash ^ data[i]) * p;
                }

                // Restore stream position.
                stream.Position = prevPosition;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        /// <summary>
        /// Writes the effect for loading later.
        /// </summary>
        public void Write(BinaryWriter writer, MonogameEffectCompilerOptions options)
        {
            // Write a very simple header for identification and versioning.
            writer.Write(Header.ToCharArray());
            writer.Write((byte)Version);

            // Write an simple identifier for DX11 vs GLSL
            // so we can easily detect the correct shader type.
            var profile = (byte)options.Profile.FormatId;
            writer.Write(profile);

            // Write the rest to a memory stream.
            using (MemoryStream memStream = new MemoryStream())
            using (BinaryWriter memWriter = new BinaryWriter(memStream))
            {
                // Write all the constant buffers.
                memWriter.Write(ConstantBuffers.Count);
                foreach (var cbuffer in ConstantBuffers)
                    cbuffer.Write(memWriter, options);

                // Write all the shaders.
                memWriter.Write(Shaders.Count);
                foreach (var shader in Shaders)
                    shader.Write(memWriter, options);

                // Write the parameters.
                WriteParameters(memWriter, Parameters, Parameters.Length);

                // Write the techniques.
                memWriter.Write(Techniques.Length);
                foreach (var technique in Techniques)
                {
                    memWriter.Write(technique.name);
                    WriteAnnotations(memWriter, technique.annotation_handles);

                    // Write the passes.
                    memWriter.Write((int)technique.pass_count);
                    for (var p = 0; p < technique.pass_count; p++)
                    {
                        var pass = technique.pass_handles[p];

                        memWriter.Write(pass.name);
                        WriteAnnotations(memWriter, pass.annotation_handles);

                        // Write the index for the vertex and pixel shaders.
                        var vertexShader = GetShaderIndex(STATE_CLASS.VERTEXSHADER, pass.states);
                        var pixelShader = GetShaderIndex(STATE_CLASS.PIXELSHADER, pass.states);
                        memWriter.Write(vertexShader);
                        memWriter.Write(pixelShader);

                        // Write the state objects too!
                        if (pass.blendState != null)
                        {
                            memWriter.Write(true);
                            memWriter.Write((byte)pass.blendState.AlphaBlendFunction);
                            memWriter.Write((byte)pass.blendState.AlphaDestinationBlend);
                            memWriter.Write((byte)pass.blendState.AlphaSourceBlend);
                            memWriter.Write(pass.blendState.BlendFactor.R);
                            memWriter.Write(pass.blendState.BlendFactor.G);
                            memWriter.Write(pass.blendState.BlendFactor.B);
                            memWriter.Write(pass.blendState.BlendFactor.A);
                            memWriter.Write((byte)pass.blendState.ColorBlendFunction);
                            memWriter.Write((byte)pass.blendState.ColorDestinationBlend);
                            memWriter.Write((byte)pass.blendState.ColorSourceBlend);
                            memWriter.Write((byte)pass.blendState.ColorWriteChannels);
                            memWriter.Write((byte)pass.blendState.ColorWriteChannels1);
                            memWriter.Write((byte)pass.blendState.ColorWriteChannels2);
                            memWriter.Write((byte)pass.blendState.ColorWriteChannels3);
                            memWriter.Write(pass.blendState.MultiSampleMask);
                        }
                        else
                            memWriter.Write(false);

                        if (pass.depthStencilState != null)
                        {
                            memWriter.Write(true);
                            memWriter.Write((byte)pass.depthStencilState.CounterClockwiseStencilDepthBufferFail);
                            memWriter.Write((byte)pass.depthStencilState.CounterClockwiseStencilFail);
                            memWriter.Write((byte)pass.depthStencilState.CounterClockwiseStencilFunction);
                            memWriter.Write((byte)pass.depthStencilState.CounterClockwiseStencilPass);
                            memWriter.Write(pass.depthStencilState.DepthBufferEnable);
                            memWriter.Write((byte)pass.depthStencilState.DepthBufferFunction);
                            memWriter.Write(pass.depthStencilState.DepthBufferWriteEnable);
                            memWriter.Write(pass.depthStencilState.ReferenceStencil);
                            memWriter.Write((byte)pass.depthStencilState.StencilDepthBufferFail);
                            memWriter.Write(pass.depthStencilState.StencilEnable);
                            memWriter.Write((byte)pass.depthStencilState.StencilFail);
                            memWriter.Write((byte)pass.depthStencilState.StencilFunction);
                            memWriter.Write(pass.depthStencilState.StencilMask);
                            memWriter.Write((byte)pass.depthStencilState.StencilPass);
                            memWriter.Write(pass.depthStencilState.StencilWriteMask);
                            memWriter.Write(pass.depthStencilState.TwoSidedStencilMode);
                        }
                        else
                            memWriter.Write(false);

                        if (pass.rasterizerState != null)
                        {
                            memWriter.Write(true);
                            memWriter.Write((byte)pass.rasterizerState.CullMode);
                            memWriter.Write(pass.rasterizerState.DepthBias);
                            memWriter.Write((byte)pass.rasterizerState.FillMode);
                            memWriter.Write(pass.rasterizerState.MultiSampleAntiAlias);
                            memWriter.Write(pass.rasterizerState.ScissorTestEnable);
                            memWriter.Write(pass.rasterizerState.SlopeScaleDepthBias);
                        }
                        else
                            memWriter.Write(false);
                    }
                }

                // Calculate a hash code from memory stream
                // and write it to the header.
                var effectKey = ComputeHash(memStream);
                writer.Write((Int32)effectKey);

                //write content from memory stream to final stream.
                memStream.WriteTo(writer.BaseStream);
            }

            // Write a tail to be used by the reader for validation.
            writer.Write(Header.ToCharArray());
        }

        private static void WriteParameters(BinaryWriter writer, d3dx_parameter[] parameters, int count)
        {
            writer.Write(count);
            for (var i = 0; i < count; i++)
                WriteParameter(writer, parameters[i]);
        }

        private static void WriteParameter(BinaryWriter writer, d3dx_parameter param)
        {
            var class_ = ToXNAParameterClass(param.class_);
            var type = ToXNAParameterType(param.type);
            writer.Write((byte)class_);
            writer.Write((byte)type);

            writer.Write(param.name);
            writer.Write(param.semantic);
            WriteAnnotations(writer, param.annotation_handles);

            writer.Write((byte)param.rows);
            writer.Write((byte)param.columns);

            // Write the elements or struct members.
            WriteParameters(writer, param.member_handles, (int)param.element_count);
            WriteParameters(writer, param.member_handles, (int)param.member_count);

            if (param.element_count == 0 && param.member_count == 0)
            {
                switch (type)
                {
                    case EffectParameterType.Bool:
                    case EffectParameterType.Int32:
                    case EffectParameterType.Single:
                        writer.Write((byte[])param.data);
                        break;
                }
            }
        }

        private static void WriteAnnotations(BinaryWriter writer, d3dx_parameter[] annotations)
        {
            var count = annotations == null ? 0 : annotations.Length;
            writer.Write(count);
            for (var i = 0; i < count; i++)
                WriteParameter(writer, annotations[i]);
        }
	}
}

