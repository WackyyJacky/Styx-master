using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

internal class FileDownloader
{
    private readonly string _fullPathWhereToSave;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
    private readonly string _url;
    private bool _result;

    public FileDownloader(string url, string fullPathWhereToSave)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
        if (string.IsNullOrEmpty(fullPathWhereToSave)) throw new ArgumentNullException("fullPathWhereToSave");

        _url = url;
        _fullPathWhereToSave = fullPathWhereToSave;
    }

    public bool StartDownload(int timeout)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPathWhereToSave));

            if (File.Exists(_fullPathWhereToSave)) File.Delete(_fullPathWhereToSave);

            using (var client = new WebClient())
            {
                var ur = new Uri(_url);
                // client.Credentials = new NetworkCredential("username", "password");
                client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                client.DownloadFileCompleted += WebClientDownloadCompleted;
                Console.WriteLine(@"Downloading: " + _url);
                Console.WriteLine(@"Saving To: " + _fullPathWhereToSave);
                client.DownloadFileAsync(ur, _fullPathWhereToSave);
                _semaphore.Wait(timeout);
                return _result && File.Exists(_fullPathWhereToSave);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Was not able to download file!");
            Console.Write(e);
            return false;
        }
        finally
        {
            _semaphore.Dispose();
        }
    }

    private void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        Console.Write("\r     -->    {0}%.", e.ProgressPercentage);
    }

    private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
    {
        _result = !args.Cancelled;
        if (!_result) Console.Write(args.Error.ToString());
        Console.WriteLine(Environment.NewLine + "Download Finished!");
        _semaphore.Release();
    }

    public static bool DownloadFile(string url, string fullPathWhereToSave, int timeoutInMilliSec)
    {
        return new FileDownloader(url, fullPathWhereToSave).StartDownload(timeoutInMilliSec);
    }
}