using System;
using System.Collections.Generic;
using System.Text;

namespace SBR;

// this class represents data in myApps.xml
internal class MyApps
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ScreenshotName { get; set; } = string.Empty;
}
