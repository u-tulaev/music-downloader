﻿using MusicDownloader.Downloader;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace MusicDownloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LinkFinder lf;
        private volatile bool _completed;

        public MainWindow()
        {
            InitializeComponent();

            lf = new LinkFinder();
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            Download(tBSearch.Text);
        }

        public void Download(string songName)
        {
            lf = new LinkFinder();

            if (Directory.Exists(lf.GetPath))
            {
                _completed = false;

                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);

                        wc.DownloadFileAsync(
                            new Uri(lf.GetDownloadString(songName)),
                            Path.Combine(lf.GetPath, $"{songName + Guid.NewGuid()}.mp3")
                        );

                        //string[] lines = lf.GetSongTitle.Split(new char[] { '-', '\n' });
                        //tBlocksongTitle.Text += String.Format(" {0}", lines[1].Trim());
                        //tBlockAuthor.Text += String.Format(" {0}", lines[2].Trim());
                    }
                }
                catch
                {
                    MessageBox.Show("Oops! Seems something went wrong. Check your Internet connection", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    tBSearch.Text = String.Empty;
                }
            }
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            string progress = String.Format("{0} downloaded {1:0.0} of {2:0.0} MB. {3} % complete...",
                (string)e.UserState,
                BytesToMB(e.BytesReceived),
                BytesToMB(e.TotalBytesToReceive),
                e.ProgressPercentage);

            tBlockProgressInfo.Text = progress;
            pBProcess.Value = e.ProgressPercentage;
        }

        private double BytesToMB(long bytes) => ((bytes / 1000000d) < 1.0 ? bytes / 100000d : bytes / 1000000d); 

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
                tBlockProgressInfo.Text = "Download has been canceled.";
            else
                tBlockProgressInfo.Text = "Download completed!";

            _completed = true;
            Process.Start(lf.GetPath);
        }

    }
}