using System.Threading.Tasks;
using AotObjectMapper.Mapper.Analyzers;
using AotObjectMapper.Mapper.CodeFixes;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace AotObjectMapper.Mapper.Tests.DiagnosticTests;

public class NameOfAnalyzerTests : AOMVerifierBase
{
    DiagnosticResult ExpectedDiagnostic(string type, string member, int location)
        => DiagnosticResult.CompilerWarning(AOMDiagnostics.PreferNameOfId)
                           .WithMessageFormat(AOMDiagnostics.AOM202_PreferNameOf.MessageFormat)
                           .WithArguments(type, member)
                           .WithLocation(location);

    [Fact]
    public async Task PreferNameOf_StringArg_AOM202()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1 { public int Id { get; set; } }
        public class T2 { public int Id { get; set; } }

        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>({|#0:"Id"|})]
            private static int GetId(T1 src) => 0;
        }
        """;

        await VerifyAnalyzerAsync<PreferNameOfAnalyzer>(code, [ExpectedDiagnostic("T2", "Id", 0)], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task PreferNameOf_StringArgTypeAlias_AOM202()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        namespace Space1
        {
            public class T1 { public int Id { get; set; } }
            public class T2 { public int Id { get; set; } }
        }
        
        namespace Space2
        {
            using S = Space1.T1;
            using T = Space1.T2;
        
            [GenerateMapper]
            [Map<S, T>]
            public partial class TMapper
            {
                [ForMember<S, T>({|#0:"Id"|})]
                private static int GetId(S src) => 0;
            }
        }
        """;

        await VerifyAnalyzerAsync<PreferNameOfAnalyzer>(code, [ExpectedDiagnostic("T", "Id", 0)], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task PreferNameOf_ConstArg_NoDiagnostic()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1 { public int Id { get; set; } }
        public class T2 { public int Id { get; set; } }
        

        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            public const string Arg = "Id";
        
            [ForMember<T1, T2>(Arg)]
            private static int GetId(T1 src) => 0;
        }
        """;

        await VerifyAnalyzerAsync<PreferNameOfAnalyzer>(code, [], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task PreferNameOf_NameOf_NoDiagnostic()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1 { public int Id { get; set; } }
        public class T2 { public int Id { get; set; } }

        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>(nameof(T2.Id))]
            private static int GetId(T1 src) => 0;
        }
        """;

        await VerifyAnalyzerAsync<PreferNameOfAnalyzer>(code, [], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task PreferNameOf_CodeFix_ReplacesString()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1 { public int Id { get; set; } }
        public class T2 { public int Id { get; set; } }

        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>({|#0:"Id"|})]
            private static int GetId(T1 src) => 0;
        }
        """;

        const string codeFix =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        public class T1 { public int Id { get; set; } }
        public class T2 { public int Id { get; set; } }

        [GenerateMapper]
        [Map<T1, T2>]
        public partial class TMapper
        {
            [ForMember<T1, T2>(nameof(T2.Id))]
            private static int GetId(T1 src) => 0;
        }
        """;

        await VerifyCodeFixAsync<PreferNameOfAnalyzer, Aom202Fix>(code, codeFix, [ExpectedDiagnostic("T2", "Id", 0)], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task PreferNameOf_StringArgTypeAlias_ReplacesStrings()
    {
        const string code =
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;

            namespace Space1
            {
                public class T1 { public int Id { get; set; } }
                public class T2 { public int Id { get; set; } }
            }

            namespace Space2
            {
                using S = Space1.T1;
                using T = Space1.T2;

                [GenerateMapper]
                [Map<S, T>]
                public partial class TMapper
                {
                    [ForMember<S, T>({|#0:"Id"|})]
                    private static int GetId(S src) => 0;
                }
            }
            """;

        const string codeFix =
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;

            namespace Space1
            {
                public class T1 { public int Id { get; set; } }
                public class T2 { public int Id { get; set; } }
            }

            namespace Space2
            {
                using S = Space1.T1;
                using T = Space1.T2;

                [GenerateMapper]
                [Map<S, T>]
                public partial class TMapper
                {
                    [ForMember<S, T>(nameof(T.Id))]
                    private static int GetId(S src) => 0;
                }
            }
            """;

        await VerifyCodeFixAsync<PreferNameOfAnalyzer, Aom202Fix>(code, codeFix, [ExpectedDiagnostic("T", "Id", 0)], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task PreferNameOf_IEnumerableArguments_ReplacesString()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1 { public int Id { get; set; } public string Name { get; set; } }
        public class T2 { public int Id { get; set; } public string Name { get; set; } }

        [GenerateMapper]
        [Map<T1, T2>({|#0:"Id"|}, {|#1:"Name"|})]
        public partial class TMapper;
        """;

        const string codeFix =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;
        
        public class T1 { public int Id { get; set; } public string Name { get; set; } }
        public class T2 { public int Id { get; set; } public string Name { get; set; } }

        [GenerateMapper]
        [Map<T1, T2>(nameof(T2.Id), nameof(T2.Name))]
        public partial class TMapper;
        """;

        await VerifyCodeFixAsync<PreferNameOfAnalyzer, Aom202Fix>(code, codeFix, [ExpectedDiagnostic("T2", "Id", 0), ExpectedDiagnostic("T2", "Name", 1)], TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task PreferNameOf_IEnumerableArguments_AOM202()
    {
        const string code =
        """
        using System;
        using AotObjectMapper.Abstractions.Attributes;
        using AotObjectMapper.Abstractions.Enums;
        using AotObjectMapper.Abstractions.Models;

        public class T1 { public int Id { get; set; } public string Name { get; set; } }
        public class T2 { public int Id { get; set; } public string Name { get; set; } }

        [GenerateMapper]
        [Map<T1, T2>({|#0:"Id"|}, {|#1:"Name"|})]
        public partial class TMapper;
        """;

        await VerifyAnalyzerAsync<PreferNameOfAnalyzer>(code, [ExpectedDiagnostic("T2", "Id", 0), ExpectedDiagnostic("T2", "Name", 1)], TestContext.Current.CancellationToken);
    }
}