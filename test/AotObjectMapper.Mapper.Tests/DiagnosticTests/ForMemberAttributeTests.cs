using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace AotObjectMapper.Mapper.Tests.DiagnosticTests;

public class ForMemberAttributeTests : AOMVerifierBase
{
    [Fact]
    public async Task ForMemberAttribute_NoDiagnostics()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1
        {
            public int Id { get; set; }
        }
        
        public class T2
        {
            public int Id { get; set; }
        }
        
        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>("Id")]
            private static int GetId(T1 src) => 0;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ForMemberAttribute_AllIssues_AOM100_AOM101_AOM104()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1
        {
            public int Id { get; set; }
        }
        
        public class T2
        {
            public int Id { get; set; }
        }
        
        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>("Id")]
            private double {|#0:GetId|}(T2 {|#1:src|}, T1 {|#2:ctx|}) => 0;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(
          code,
          [
              DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureNotStaticId).WithLocation(0).WithArguments("GetId"),
              DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureReturnTypeId).WithLocation(0).WithArguments("GetId", "Int32"),
              DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureParameterTypeId).WithLocation(1).WithArguments("First" ,"GetId", "T1"),
              DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureParameterTypeId).WithLocation(2).WithArguments("Second" ,"GetId", "MapperContextBase"),
          ],
          cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ForMemberAttribute_NotStatic_AOM104()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1
        {
            public int Id { get; set; }
        }
        
        public class T2
        {
            public int Id { get; set; }
        }
        
        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>("Id")]
            private int {|#0:GetId|}(T1 src) => 0;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureNotStaticId).WithLocation(0).WithArguments("GetId")], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ForMemberAttribute_IncorrectReturnType_AOM100()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1
        {
            public int Id { get; set; }
        }
        
        public class T2
        {
            public int Id { get; set; }
        }
        
        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>("Id")]
            private static string {|#0:GetId|}(T1 src) => "";
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureReturnTypeId).WithLocation(0).WithArguments("GetId", "Int32")], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ForMemberAttribute_IncorrectSourceType_AOM101()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1
        {
            public int Id { get; set; }
        }
        
        public class T2
        {
            public int Id { get; set; }
        }
        
        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>("Id")]
            private static int GetId(T2 {|#0:src|}) => 0;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureParameterTypeId).WithLocation(0).WithArguments("First" ,"GetId", "T1")], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ForMemberAttribute_IncorrectContextType_AOM101()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1
        {
            public int Id { get; set; }
        }
        
        public class T2
        {
            public int Id { get; set; }
        }
        
        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>("Id")]
            private static int GetId(T1 src, MapperContext {|#0:ctx|}) => 0;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [DiagnosticResult.CompilerError(AOMDiagnostics.MethodHasIncorrectSignatureParameterTypeId).WithLocation(0).WithArguments("Second" ,"GetId", "MapperContextBase")], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ForMemberAttribute_InvalidMemberName_AOM201()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1
        {
            public int Id { get; set; }
        }
        
        public class T2
        {
            public int Id { get; set; }
        }
        
        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>({|#0:"Ids"|})]
            private static int GetId(T1 src) => 0;
        }
        """;

        await VerifyGeneratorDiagnosticsAsync(code, [DiagnosticResult.CompilerError(AOMDiagnostics.MemberNamesShouldBeValidId).WithLocation(0).WithArguments("Ids" , "T2")], cancellationToken: TestContext.Current.CancellationToken);
    }
}