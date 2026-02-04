using BenchmarkDotNet.Running;
using BenchmarkDotNetVisualizer;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).RunAll();

        await summaries.ConcatReportsAndSaveAsImageAsync(
            DirectoryHelper.GetPathRelativeToProjectDirectory("BenchmarkSummary.png"),
            new ConcatReportImageOptions()
            {
                Title           = "Object Mappers Benchmark",
                SortByColumns   = ["Mean", "Allocated",],
                SpectrumColumns = ["Mean", "Allocated",],
            });
    }
}