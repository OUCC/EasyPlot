using EasyPlot.Contracts.Services;

namespace EasyPlot.Services;

public class SharedConfigService : ISharedConfigService
{
    public string TempDirectory { get; }

    public string ResultGpPath { get; }

    public string ResultPngPath { get; }

    public string ResultPngPathNotBackSlash { get; }

    public SharedConfigService()
    {
        // TODO: GUID に変更するか検討
        TempDirectory = Path.Combine(Path.GetTempPath(), "EasyPlot");

        ResultGpPath = Path.Combine(TempDirectory, "result.gp");
        ResultPngPath = Path.Combine(TempDirectory, "result.png");
        ResultPngPathNotBackSlash = ResultPngPath.Replace('\\', '/');

        if (!Directory.Exists(TempDirectory))
            Directory.CreateDirectory(TempDirectory);
    }
}
