namespace EasyPlot.Contracts.Services;

public interface ISharedConfigService
{
    string TempDirectory { get; }

    string ResultGpPath { get; }

    string ResultPngPath { get; }

    // gnuplot では \ をパスとして認識しないので / のものを用意しておく
    /// <summary>
    /// パス区切り文字を / で返します
    /// </summary>
    string ResultPngPathNotBackSlash { get; }
}
