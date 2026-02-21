using Microsoft.Xna.Framework;
using ioi.Monogame.Settings;
using ioi.Monogame.Viewports;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ioi
{
    internal partial class GameHost
	{
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117
        }

#pragma warning disable CA1416 // Validate platform compatibility
        static double GetWindowsScreenScalingFactor(bool percentage = true)
        {
            //Create Graphics object from the current windows handle
            Graphics GraphicsObject = Graphics.FromHwnd(IntPtr.Zero);
                              //Get Handle to the device context associated with this Graphics object
            IntPtr DeviceContextHandle = GraphicsObject.GetHdc();
            //Call GetDeviceCaps with the Handle to retrieve the Screen Height
            int LogicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
            //Divide the Screen Heights to get the scaling factor and round it to two decimals
            double ScreenScalingFactor = Math.Round((double)PhysicalScreenHeight / (double)LogicalScreenHeight, 2);
            //If requested as percentage - convert it
            if (percentage)
            {
                ScreenScalingFactor *= 100.0;
            }
            //Release the Handle and Dispose of the GraphicsObject object
            GraphicsObject.ReleaseHdc(DeviceContextHandle);
            GraphicsObject.Dispose();
            //Return the Scaling Factor
            return ScreenScalingFactor;
        }
#pragma warning restore CA1416 // Validate platform compatibility

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr LoadLibraryW(string lpszLib);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetNumVideoDisplays();

		public delegate void GetDisplayBounds(int index, out SDL_Rect rect);

		public struct SDL_Rect
		{
			public int x { get; set; }

			public int y { get; set; }

			public int w { get; set; }

			public int h { get; set; }
		}

		private static List<SDL_Rect> MonitorBounds = new List<SDL_Rect>();
		private static bool SDLLoaded = false;

		private void SDL_InitMonitors()
		{
			if (SDLLoaded)
			{
				// that means counter already +1 on SDL library (monogame import internal same dll)
				// So, we not need dispose it, but it will be very good not increment counter
				// acutally, if GameClient will be reinited, but application is not - it can be potential increment
				return;
			}

			var entrydll = Assembly.GetExecutingAssembly().Location;
			var root = Path.GetDirectoryName(entrydll);

			var sdlPath = Path.Combine(root, $@"runtimes\{RuntimeInformation.RuntimeIdentifier}\native\SDL2.dll");

			if (!File.Exists(sdlPath))
				sdlPath = Path.Combine(root, "SDL2.dll");

			if (!File.Exists(sdlPath))
			{
				Console.WriteLine("Cant find SDL2.dll, monitor choosing is not available!");
				return;
			}

			var SDL = LoadLibraryW(sdlPath);
			SDLLoaded = true;

			var SDL_GetNumVideoDisplays = GetProcAddress(SDL, "SDL_GetNumVideoDisplays");
			var SDL_GetNumVideoDisplaysFunc = Marshal.GetDelegateForFunctionPointer<GetNumVideoDisplays>(SDL_GetNumVideoDisplays);

			var dCount = SDL_GetNumVideoDisplaysFunc();

			var SDL_GetDisplayBounds = GetProcAddress(SDL, "SDL_GetDisplayBounds");
			var SDL_GetDisplayBoundsFunc = Marshal.GetDelegateForFunctionPointer<GetDisplayBounds>(SDL_GetDisplayBounds);

			for (int i = 0; i < dCount; i++)
			{
				SDL_Rect r = new SDL_Rect();
				SDL_GetDisplayBoundsFunc(i, out r);
				MonitorBounds.Add(r);
			}
		}

		private SDL_Rect SelectedMonitorBounds;

		private bool SetMonitor(int index)
		{
			Point windowPosition = default;

			var bounds = MonitorBounds.ElementAtOrDefault(index);
						
			if (MonitorBounds.Count == 1)
				bounds = MonitorBounds[0];

            SelectedMonitorBounds = bounds;

            if (bounds.x != 0)
			{
				var isfullscreen = Settings.WindowMode == WindowMode.FullScreenHardware || Settings.WindowMode == WindowMode.FullScreenSoftware;
				windowPosition = new Point(bounds.x, isfullscreen ? 0 : 50);
			}

			var resolution = Game.MainViewport.Bounds;

			if (resolution.Width != bounds.w || resolution.Height != bounds.h)
			{
				if (bounds.w > resolution.Width || bounds.h > resolution.Height)
				{
                    windowPosition = new Point
					{
						X = bounds.w / 2 - resolution.Width / 2,
						Y = bounds.h / 2 - resolution.Height / 2
					};
				}
			}

			Window.Position = windowPosition;

			return false;
		}
	}
}