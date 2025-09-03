using System;
using System.IO;
using System.Windows;
using CTecUtil;
using CTecUtil.Config;
using Newtonsoft.Json;

namespace Xfp.Config
{
    public class XfpApplicationConfig : ApplicationConfig
    {
        static XfpApplicationConfig() => Settings = new XfpApplicationConfig();
        public XfpApplicationConfig() => Data = new XfpApplicationConfigData();


        public static XfpApplicationConfig Settings { get; protected set; }


        protected override ApplicationConfigData newConfigDataData() => new XfpApplicationConfigData();

        protected override void readSettings()
        {
            if (!_initialised)
                notInitialisedError();

            try
            {
                using FileStream stream = new(_configFilePath, FileMode.Open, FileAccess.Read);
                if (stream is not null)
                {
                    using StreamReader reader = new(stream);
                    if (reader is not null)
                    {
                        Data = JsonConvert.DeserializeObject<XfpApplicationConfigData>(reader.ReadToEnd());
                        reader.Close();
                    }
                }
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException ex) { Debug.WriteLine(ex.Message); }
            catch (UnauthorizedAccessException ex) { Debug.WriteLine(ex.Message); }
            catch (IOException ex) { Debug.WriteLine(ex.Message); }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }


        /// <summary>
        /// Monitor window's size and position.
        /// </summary>
        public WindowSizeParams MonitorWindow { get => ((Xfp.Config.XfpApplicationConfigData)Data).MonitorWindow; }


        /// <summary>
        /// Save the Monitor window's size and position.
        /// </summary>
        public void UpdateMonitorWindowParams(Window window, double scale, bool saveSettings = false)
        {
            ((Xfp.Config.XfpApplicationConfigData)Data).MonitorWindow = new();
            updateWindowParams(window, scale, ((Xfp.Config.XfpApplicationConfigData)Data).ValidationWindow, saveSettings);
        }


        public double ValidationWindowZoomLevel
        {
            get => ((Xfp.Config.XfpApplicationConfigData)Data).ValidationWindow.Scale;
            set => ((Xfp.Config.XfpApplicationConfigData)Data).ValidationWindow.Scale = value;
        }

        public double SerialMonitorZoomLevel
        {
            get => ((Xfp.Config.XfpApplicationConfigData)Data).MonitorWindow.Scale;
            set => ((Xfp.Config.XfpApplicationConfigData)Data).MonitorWindow.Scale = value;
        }
    }
}
