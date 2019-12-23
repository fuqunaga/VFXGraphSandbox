using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.VFX
{
    [VFXInfo]
    class VFXVatOutput : VFXShaderGraphParticleOutput
    {
        public override string name { get { return "Output Particle VAT"; } }
        public override string codeGeneratorTemplate { get { return "Assets/VATExtension/Templates/VFXParticleMeshes"; } }
        public override VFXTaskType taskType { get { return VFXTaskType.ParticleMeshOutput; } }
        public override bool supportsUV { get { return shaderGraph == null; } }
        public override bool implementsMotionVector { get { return true; } }
        public override CullMode defaultCullMode { get { return CullMode.Back;  } }

        public override IEnumerable<VFXAttributeInfo> attributes
        {
            get
            {
                yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Color, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Alpha, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Alive, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.AxisX, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.AxisY, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.AxisZ, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.AngleX, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.AngleY, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.AngleZ, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.PivotX, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.PivotY, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.PivotZ, VFXAttributeMode.Read);

                yield return new VFXAttributeInfo(VFXAttribute.Size, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.ScaleX, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.ScaleY, VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.ScaleZ, VFXAttributeMode.Read);

                if (usesFlipbook)
                    yield return new VFXAttributeInfo(VFXAttribute.TexIndex, VFXAttributeMode.Read);
            }
        }

        protected override IEnumerable<VFXNamedExpression> CollectGPUExpressions(IEnumerable<VFXNamedExpression> slotExpressions)
        {
            foreach (var exp in base.CollectGPUExpressions(slotExpressions))
                yield return exp;
            if (shaderGraph == null)
            {
                yield return slotExpressions.First(o => o.name == "mainTexture");
                yield return slotExpressions.First(o => o.name == "vertexAnimTexture");
                yield return slotExpressions.First(o => o.name == "animTime");
            }
        }

        protected override IEnumerable<VFXPropertyWithValue> inputProperties
        {
            get
            {
                if( shaderGraph == null)
                    foreach (var property in PropertiesFromType("OptionalInputProperties"))
                        yield return property;
                foreach (var property in base.inputProperties)
                    yield return property;
            }
        }

        public class OptionalInputProperties
        {
            [Tooltip("Specifies the base color (RGB) and opacity (A) of the particle.")]
            public Texture2D mainTexture = VFXResources.defaultResources.particleTexture;

            public Texture2D vertexAnimTexture = VFXResources.defaultResources.particleTexture;
            public float animTime = 0f;
        }
        public class InputProperties
        {
            [Tooltip("Specifies the mesh used to render the particle.")]
            public Mesh mesh = VFXResources.defaultResources.mesh;
            [Tooltip("Defines a bitmask to control which submeshes are rendered."), BitField]
            public uint subMeshMask = 0xffffffff;

           // public Texture vertexAnimTexture;
        }

        public override VFXExpressionMapper GetExpressionMapper(VFXDeviceTarget target)
        {
            var mapper = base.GetExpressionMapper(target);

            switch (target)
            {
                case VFXDeviceTarget.CPU:
                {
                    mapper.AddExpression(inputSlots.First(s => s.name == "mesh").GetExpression(), "mesh", -1);
                    mapper.AddExpression(inputSlots.First(s => s.name == "subMeshMask").GetExpression(), "subMeshMask", -1);
                    break;
                }
                default:
                {
                    break;
                }
            }

            return mapper;
        }
    }
}