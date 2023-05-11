﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Uno.Resizetizer;

public class WindowIconGeneratorTask_V0 : Task
{
	private const string FileName = "Uno.Resizetizer.WindowIconExtensions.g.cs";

    public ITaskItem[] UnoIcons { get; set; }

	[Required]
	public string IntermediateOutputDirectory { get; set; }

	private List<ITaskItem> _generatedClass = new List<ITaskItem>();
	[Output]
	public ITaskItem[] GeneratedClass => _generatedClass.ToArray();

	public override bool Execute()
	{
		if (UnoIcons is null || UnoIcons.Length == 0)
			return true;

		if(string.IsNullOrEmpty(IntermediateOutputDirectory))
		{
			Log.LogError("The IntermediateOutputDirectory (typically the obj directory) is a required parameter but was null or empty.");
			return false;
		}

		var iconPath = UnoIcons[0].ItemSpec;
		var iconName = Path.GetFileNameWithoutExtension(iconPath);

		var code = @$"//------------------------------------------------------------------------------
// <auto-generated>
//  This code was auto-generated.
//
//  Changes to this file may cause incorrect behavior and will be lost if
//  the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Uno.Resizetizer
{{
	public static class WindowExtensions
	{{
		/// <summary>
		/// This will set the Window Icon for the given <see cref=""global::Microsoft.UI.Xaml.Window"" /> using
		/// the provided UnoIcon.
		/// </summary>
		public static void SetWindowIcon(this global::Microsoft.UI.Xaml.Window window)
		{{
#if WINDOWS
			var hWnd =
			global::WinRT.Interop.WindowNative.GetWindowHandle(window);

			// Retrieve the WindowId that corresponds to hWnd.
			global::Microsoft.UI.WindowId windowId =
			global::Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

			// Lastly, retrieve the AppWindow for the current (XAML) WinUI 3 window.
			global::Microsoft.UI.Windowing.AppWindow appWindow =
				global::Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
			appWindow.SetIcon(""{iconName}.ico"");
#endif
		}}
	}}
}}";

		if(!Directory.Exists(IntermediateOutputDirectory))
		{
			Directory.CreateDirectory(IntermediateOutputDirectory);
		}

		var item = new TaskItem(Path.Combine(IntermediateOutputDirectory, FileName));
		File.WriteAllText(item.ItemSpec, code);
		_generatedClass.Add(item);
		return true;
	}
}
