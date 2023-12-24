using Common.ConfigProvider;

namespace CommonTests;

public class CommonUtilTests
{
    [Fact]
    public void ShouldMapArtifactsFolder()
    {
        Assert.Equal("C:\\ProgramData\\Infopercept\\artifacts", CommonUtils.ArtifactsFolder);
    }

    [Fact]
    public void ShouldMapConfigFolder()
    {
        Assert.Equal("C:\\ProgramData\\Infopercept\\configs", CommonUtils.ConfigFolder);
    }

    [Fact]
    public void ShouldMapLogsFolder()
    {
        Assert.Equal("C:\\ProgramData\\Infopercept\\logs", CommonUtils.LogsFolder);
    }
}