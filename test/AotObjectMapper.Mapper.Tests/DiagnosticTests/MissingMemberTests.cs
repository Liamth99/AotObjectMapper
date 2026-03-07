using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace AotObjectMapper.Mapper.Tests.DiagnosticTests;

public class MissingMemberTests : AOMVerifierBase
{
    DiagnosticResult ExpectedErrorDiagnostic(string member, string type, int location)
        => DiagnosticResult.CompilerError(AOMDiagnostics.RequiredMemberNotMappedId)
                           .WithMessageFormat(AOMDiagnostics.AOM302_RequiredMemberNotMapped.MessageFormat)
                           .WithArguments(member, type)
                           .WithLocation(location);

    DiagnosticResult ExpectedWarningDiagnostic(string member, string type, int location)
        => DiagnosticResult.CompilerWarning(AOMDiagnostics.UnmappedDestinationMemberId)
                           .WithMessageFormat(AOMDiagnostics.AOM400_UnmappedDestinationMember.MessageFormat)
                           .WithArguments(member, type)
                           .WithLocation(location);

    [Fact]
    public async Task MapAttribute_RequiredMemberNotMapped_AOM302()
    {
        const string code =
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;
            
            public class S;
            
            public class T
            {
                public required string Prop { get; set; } = "";
            }

            public partial class Top
            {
                [GenerateMapper]
                [{|#0:Map<S, T>|}]
                public partial class Mapper;
            }
            """;

        await VerifyGeneratorDiagnosticsAsync(code, [ExpectedErrorDiagnostic("Prop", "T", 0)], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MapAttribute_RequiredMemberNotMappedSuppressed_AOM400()
    {
        const string code =
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;
            
            public class S;
            
            public class T
            {
                public required string Prop { get; set; }
            }

            public partial class Top
            {
                [GenerateMapper(MappingOptions.SuppressNullWarnings)]
                [{|#0:Map<S, T>|}]
                public partial class Mapper;
            }
            """;

        await VerifyGeneratorDiagnosticsAsync(code, [ExpectedWarningDiagnostic("Prop", "T", 0)], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MapAttribute_MissingMember_AOM400()
    {
        const string code =
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;
            
            public class S;
            
            public class T
            {
                public string Prop { get; set; }
            }

            public partial class Top
            {
                [GenerateMapper(MappingOptions.SuppressNullWarnings)]
                [{|#0:Map<S, T>|}]
                public partial class Mapper;
            }
            """;

        await VerifyGeneratorDiagnosticsAsync(code, [ExpectedWarningDiagnostic("Prop", "T", 0)], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MapAttribute_MissingMemberIgnored_NoDiagnostic()
    {
        const string code =
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;
            
            public class S;
            
            public class T
            {
                public string Prop { get; set; }
            }

            public partial class Top
            {
                [GenerateMapper]
                [Map<S, T>("Prop")]
                public partial class Mapper;
            }
            """;

        await VerifyGeneratorDiagnosticsAsync(code, [], cancellationToken: TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MapAttribute_RequiredMemberIgnored_AOM302()
    {
        const string code =
            """
            using System;
            using AotObjectMapper.Abstractions.Attributes;
            using AotObjectMapper.Abstractions.Enums;
            using AotObjectMapper.Abstractions.Models;
            
            public class S;
            
            public class T
            {
                public required string Prop { get; set; }
            }

            public partial class Top
            {
                [GenerateMapper]
                [{|#0:Map<S, T>("Prop")|}]
                public partial class Mapper;
            }
            """;

        await VerifyGeneratorDiagnosticsAsync(code, [ExpectedErrorDiagnostic("Prop", "T", 0)], cancellationToken: TestContext.Current.CancellationToken);
    }
}