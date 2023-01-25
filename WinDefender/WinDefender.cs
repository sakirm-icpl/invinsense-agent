using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jitbit.Utils
{
	public class WinDefender
	{
		private static bool _isDefenderAvailable;
		private static string _defenderPath;
		private static SemaphoreSlim _lock = new SemaphoreSlim(5); //limit to 5 concurrent checks at a time

		//static ctor
		static WinDefender()
		{
			if (OperatingSystem.IsWindows())
			{
				_defenderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows Defender", "MpCmdRun.exe");
				_isDefenderAvailable = File.Exists(_defenderPath);
			}
			else
				_isDefenderAvailable = false;
		}

		public static async Task<bool> IsVirus(CancellationToken cancellationToken = default)
		{
			if (!_isDefenderAvailable) return false;

			//string path = Path.GetTempFileName();
			//await File.WriteAllBytesAsync(path, file, cancellationToken); //save temp file

			if (cancellationToken.IsCancellationRequested) return false;

			try
			{
                //return await IsVirus(path, cancellationToken);
                await _lock.WaitAsync(cancellationToken);
				using (var process = Process.Start(_defenderPath, $"-Scan -ScanType 1"))
				{
                    if (process == null)
                    {
                        _isDefenderAvailable = false; //disable future attempts
                        throw new InvalidOperationException("Failed to start MpCmdRun.exe");
                    }

                    try
                    {
                        //await process.WaitForExitAsync().WaitAsync(TimeSpan.FromMilliseconds(5000), cancellationToken);
						while(!process.HasExited)
						{
                            if (cancellationToken.IsCancellationRequested)
                            {
                                process.Kill();
                                break;
                            }
							await Task.Delay(10000);
                        }
                    }
                    catch (TimeoutException ex) //timeout
                    {
                        throw new TimeoutException("Timeout waiting for MpCmdRun.exe to return", ex);
                    }
                    finally
                    {
                        process.Kill(); //always kill the process, it's fine if it's already exited, but if we were timed out or cancelled via token - let's kill it
                    }

                    return process.ExitCode == 2;
                }
                
            }
			finally
			{
                _lock.Release();
            }
		}
	}
}
