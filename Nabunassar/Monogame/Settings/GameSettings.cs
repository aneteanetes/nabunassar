﻿using Microsoft.Xna.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Nabunassar.Monogame.Settings
{
    public class GameSettings
    {
        /// <summary>
        /// Логировать работу
        /// </summary>
        public bool IsLogging { get; set; }

        public string LanguageCode { get; set; } = "ru-RU";

        public string GameTitle { get; set; } = "Nabunassar";

        public TimeSpan DropFpsOnUnfocus { get; set; } = TimeSpan.Zero;

        public WindowMode WindowMode { get; set; } = WindowMode.Windowed;

        [Display(Name = "Ширина в px")]
        public int WidthPixel
        {
            get
            {
                if (widthPixel == 0)
                {
                    WidthHeightAutomated = true;
                    return OriginWidthPixel;
                }
                return widthPixel;
            }
            set => widthPixel = value;
        }
        private int widthPixel = 0;

        public bool WidthHeightAutomated { get; private set; }

        [Display(Name = "Высота в px")]
        public int HeightPixel
        {
            get
            {
                if (heightPixel == 0)
                {
                    WidthHeightAutomated = true;
                    return OriginHeightPixel;
                }
                return heightPixel;
            }
            set => heightPixel = value;
        }
        private int heightPixel = 0;

        public int OriginWidthPixel { get; set; }

        public int OriginHeightPixel { get; set; }

        [Display(Name = "V-sync")]
        public bool VerticalSync { get; set; } = true;

        [Display(Name = "2D свет")]
        public bool Add2DLighting { get; set; } = true;

        public int MonitorIndex { get; set; } = 0;

        [Display(Name = "2D свет - цвет")]
        public Color AmbientColor2DLight { get; set; }

        public bool Borderless { get; set; } = false;

        public bool IsDebug { get; set; }

        public bool NeedCalculateCamera { get; set; } = true;

        /// <summary>
        /// Растягивать или ужимать изображения если нет подходящего под разрешение
        /// </summary>
        public bool ResouceStretching { get; set; } = true;

        public bool IsGamePadConnected { get; set; } = false;

        /// <summary>
        /// Путь до проекта
        /// </summary>
        public string PathProject { get; set; }

        /// <summary>
        /// Пусть к бинарникам
        /// </summary>
        public string PathBin { get; set; }

        /// <summary>
        /// Путь к папке данных в <see cref="PathBin"/>
        /// </summary>
        public string PathData { get; set; }

        /// <summary>
        /// Путь к репозиторию
        /// </summary>
        public string PathRepository { get; set; }

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            PathBin = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            PathRepository = Directory.GetParent(PathProject).ToString();
            PathData = Path.Combine(PathBin, "Data");

            IsInitialized = true;
        }
    }
}
