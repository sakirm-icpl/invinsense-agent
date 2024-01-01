using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;


namespace Common.Net
{
    public class FragmentedFileDownloader
    {
        private readonly HttpClient _client;

        public FragmentedFileDownloader()
        {
            _client = new HttpClient(new RetryPolicyHandler("Downloader"));
        }

        public async Task<bool> ServerSupportsRangeAsync(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Head, url))
            {
                var response = await _client.SendAsync(request);
                return response.Headers.AcceptRanges.Contains("bytes");
            }
        }

        public async Task DownloadFileAsync(string fileUrl, string destinationPath)
        {
            if (!await ServerSupportsRangeAsync(fileUrl))
            {
                throw new InvalidOperationException("Server does not support range requests.");
            }

            long? totalSize = await GetFileSizeAsync(fileUrl);

            using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                long currentOffset = 0;
                int chunkSize = 1024 * 1024; // 1 MB chunks
                while (currentOffset < totalSize)
                {
                    var endOffset = Math.Min(currentOffset + chunkSize, totalSize.Value) - 1;
                    byte[] fileContent = await DownloadChunkAsync(fileUrl, currentOffset, endOffset);
                    await fileStream.WriteAsync(fileContent, 0, fileContent.Length);
                    currentOffset += chunkSize;
                }
            }
        }

        private async Task<byte[]> DownloadChunkAsync(string url, long start, long end)
        {
            using (var request = new HttpRequestMessage())
            {
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Get;
                request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(start, end);

                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        private async Task<long?> GetFileSizeAsync(string url)
        {
            using (var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                if (response.Content.Headers.ContentLength.HasValue)
                {
                    return response.Content.Headers.ContentLength.Value;
                }
                return null;
            }
        }
    }
}
