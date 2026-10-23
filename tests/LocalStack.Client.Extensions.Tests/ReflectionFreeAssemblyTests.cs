#if NET10_0_OR_GREATER
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace LocalStack.Client.Extensions.Tests;

public class ReflectionFreeAssemblyTests
{
    [Fact]
    public void Libraries_Should_Call_Runtime_Reflection_Only_From_RequiresUnreferencedCode_Methods()
    {
        var violations = new List<string>();
        var allowedCoreCalls = new List<string>();
        Scan(typeof(global::LocalStack.Client.Session).Assembly.Location, violations, allowedCoreCalls);
        Scan(typeof(global::LocalStack.Client.Extensions.ServiceCollectionExtensions).Assembly.Location, violations, new List<string>());

        Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        // The JIT-only fallback exists, so a scanner that finds nothing cannot pass.
        Assert.NotEmpty(allowedCoreCalls);
    }

    private static void Scan(string path, List<string> violations, List<string> allowed)
    {
        using var stream = File.OpenRead(path);
        using var pe = new PEReader(stream);
        MetadataReader metadata = pe.GetMetadataReader();
        foreach (MethodDefinitionHandle handle in metadata.MethodDefinitions)
        {
            MethodDefinition method = metadata.GetMethodDefinition(handle);
            if (method.RelativeVirtualAddress == 0)
            {
                continue;
            }
            BlobReader il = pe.GetMethodBody(method.RelativeVirtualAddress).GetILReader();
            while (il.RemainingBytes > 0)
            {
                byte first = il.ReadByte();
                var opcode = (ILOpCode)(first == 0xfe ? 0xfe00 | il.ReadByte() : first);
                if (opcode is ILOpCode.Call or ILOpCode.Callvirt or ILOpCode.Newobj or ILOpCode.Ldftn or ILOpCode.Ldvirtftn)
                {
                    var target = Member(metadata, MetadataTokens.EntityHandle(il.ReadInt32()));
                    if (IsBanned(metadata, target.Type, target.Name, target.Signature))
                    {
                        string call = $"{TypeName(metadata, method.GetDeclaringType())}.{metadata.GetString(method.Name)} -> {target.Type}.{target.Name}";
                        (HasRuc(metadata, handle) ? allowed : violations).Add(call);
                    }
                }
                else
                {
                    int size = OperandSize(opcode, ref il);
                    il.Offset += size;
                }
            }
        }
    }

    private static int OperandSize(ILOpCode opcode, ref BlobReader il) => opcode switch
    {
        ILOpCode.Switch => 4 * il.ReadInt32(),
        ILOpCode.Ldc_i8 or ILOpCode.Ldc_r8 => 8,
        ILOpCode.Ldarg or ILOpCode.Ldarga or ILOpCode.Starg or ILOpCode.Ldloc or ILOpCode.Ldloca or ILOpCode.Stloc => 2,
        ILOpCode.Ldarg_s or ILOpCode.Ldarga_s or ILOpCode.Starg_s or ILOpCode.Ldloc_s or ILOpCode.Ldloca_s or ILOpCode.Stloc_s or
        ILOpCode.Ldc_i4_s or ILOpCode.Unaligned or
        ILOpCode.Br_s or ILOpCode.Brfalse_s or ILOpCode.Brtrue_s or ILOpCode.Beq_s or ILOpCode.Bge_s or ILOpCode.Bgt_s or
        ILOpCode.Ble_s or ILOpCode.Blt_s or ILOpCode.Bne_un_s or ILOpCode.Bge_un_s or ILOpCode.Bgt_un_s or
        ILOpCode.Ble_un_s or ILOpCode.Blt_un_s or ILOpCode.Leave_s => 1,
        ILOpCode.Ldc_i4 or ILOpCode.Ldc_r4 or ILOpCode.Jmp or ILOpCode.Calli or
        ILOpCode.Br or ILOpCode.Brfalse or ILOpCode.Brtrue or ILOpCode.Beq or ILOpCode.Bge or ILOpCode.Bgt or
        ILOpCode.Ble or ILOpCode.Blt or ILOpCode.Bne_un or ILOpCode.Bge_un or ILOpCode.Bgt_un or
        ILOpCode.Ble_un or ILOpCode.Blt_un or ILOpCode.Leave or
        ILOpCode.Cpobj or ILOpCode.Ldobj or ILOpCode.Ldstr or ILOpCode.Castclass or ILOpCode.Isinst or ILOpCode.Unbox or
        ILOpCode.Ldfld or ILOpCode.Ldflda or ILOpCode.Stfld or ILOpCode.Ldsfld or ILOpCode.Ldsflda or ILOpCode.Stsfld or
        ILOpCode.Stobj or ILOpCode.Box or ILOpCode.Newarr or ILOpCode.Ldelema or ILOpCode.Ldelem or ILOpCode.Stelem or
        ILOpCode.Unbox_any or ILOpCode.Refanyval or ILOpCode.Mkrefany or ILOpCode.Ldtoken or ILOpCode.Initobj or
        ILOpCode.Constrained or ILOpCode.Sizeof => 4,
        _ => 0,
    };

    private static (string Type, string Name, BlobHandle Signature) Member(MetadataReader metadata, EntityHandle handle)
    {
        if (handle.Kind == HandleKind.MethodSpecification)
        {
            return Member(metadata, metadata.GetMethodSpecification((MethodSpecificationHandle)handle).Method);
        }
        if (handle.Kind == HandleKind.MethodDefinition)
        {
            MethodDefinition method = metadata.GetMethodDefinition((MethodDefinitionHandle)handle);
            return (TypeName(metadata, method.GetDeclaringType()), metadata.GetString(method.Name), method.Signature);
        }
        MemberReference member = metadata.GetMemberReference((MemberReferenceHandle)handle);
        return (TypeName(metadata, member.Parent), metadata.GetString(member.Name), member.Signature);
    }

    private static string TypeName(MetadataReader metadata, EntityHandle handle)
    {
        if (handle.Kind == HandleKind.TypeSpecification)
        {
            BlobReader signature = metadata.GetBlobReader(metadata.GetTypeSpecification((TypeSpecificationHandle)handle).Signature);
            SignatureTypeCode code = signature.ReadSignatureTypeCode();
            if (code == SignatureTypeCode.GenericTypeInstance)
            {
                code = signature.ReadSignatureTypeCode();
            }
            return code == SignatureTypeCode.TypeHandle ? TypeName(metadata, signature.ReadTypeHandle()) : string.Empty;
        }
        if (handle.Kind == HandleKind.TypeDefinition)
        {
            TypeDefinition type = metadata.GetTypeDefinition((TypeDefinitionHandle)handle);
            return type.IsNested ? $"{TypeName(metadata, type.GetDeclaringType())}+{metadata.GetString(type.Name)}" :
                $"{metadata.GetString(type.Namespace)}.{metadata.GetString(type.Name)}";
        }
        if (handle.Kind == HandleKind.TypeReference)
        {
            TypeReference type = metadata.GetTypeReference((TypeReferenceHandle)handle);
            return type.ResolutionScope.Kind == HandleKind.TypeReference ? $"{TypeName(metadata, type.ResolutionScope)}+{metadata.GetString(type.Name)}" :
                $"{metadata.GetString(type.Namespace)}.{metadata.GetString(type.Name)}";
        }
        return string.Empty;
    }

    private static bool IsBanned(MetadataReader metadata, string type, string name, BlobHandle signature)
    {
        return type switch
        {
            "System.Type" or "System.Reflection.TypeInfo" => name is
                "GetField" or "GetFields" or "GetProperty" or "GetProperties" or "GetConstructor" or "GetConstructors" or
                "GetMethod" or "GetMethods" or "GetMember" or "GetMembers" or "InvokeMember" ||
                (name == "GetType" && HasStringNameParameter(metadata, signature)),
            "System.Reflection.Assembly" => name is "GetType" or "GetTypes" or "GetExportedTypes" or "CreateInstance",
            "System.Reflection.FieldInfo" or "System.Reflection.PropertyInfo" => name is "GetValue" or "SetValue",
            "System.Reflection.MethodBase" or "System.Reflection.ConstructorInfo" => name == "Invoke",
            "System.Activator" => name == "CreateInstance",
            _ => type.StartsWith("System.Reflection.Emit.", StringComparison.Ordinal) ||
                (type.StartsWith("System.Linq.Expressions.Expression", StringComparison.Ordinal) && name == "Compile"),
        };
    }

    private static bool HasStringNameParameter(MetadataReader metadata, BlobHandle signature)
    {
        BlobReader reader = metadata.GetBlobReader(signature);
        if (reader.ReadSignatureHeader().IsInstance || reader.ReadCompressedInteger() == 0)
        {
            return false;
        }
        // These GetType overloads return System.Type, encoded as a class type handle.
        return reader.ReadSignatureTypeCode() == SignatureTypeCode.TypeHandle && !reader.ReadTypeHandle().IsNil &&
            reader.ReadSignatureTypeCode() == SignatureTypeCode.String;
    }

    private static bool HasRuc(MetadataReader metadata, MethodDefinitionHandle handle)
    {
        MethodDefinition method = metadata.GetMethodDefinition(handle);
        if (method.GetCustomAttributes().Any(attribute =>
            Member(metadata, metadata.GetCustomAttribute(attribute).Constructor).Type ==
            "System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute"))
        {
            return true;
        }
        string name = metadata.GetString(method.Name).TrimStart('<');
        int end = name.IndexOf('>', StringComparison.Ordinal);
        if (end < 0)
        {
            return false;
        }
        // Generated lambdas/local functions encode the outer user method in <...>.
        TypeDefinitionHandle owner = method.GetDeclaringType();
        while (!owner.IsNil)
        {
            TypeDefinition type = metadata.GetTypeDefinition(owner);
            MethodDefinitionHandle[] matches = type.GetMethods().Where(candidate =>
                metadata.GetString(metadata.GetMethodDefinition(candidate).Name) == name.Substring(0, end)).ToArray();
            if (matches.Length == 1)
            {
                return HasRuc(metadata, matches[0]);
            }
            owner = type.GetDeclaringType();
        }
        return false;
    }
}
#endif
