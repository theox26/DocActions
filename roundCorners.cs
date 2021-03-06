using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Pageflex.Scripting;
public class DocActions
{
    static public void ConvertToGrey(string rbColor, string imageName) 
    {
        if (rbColor == "BW")//the trigger field rbColor if = BW then convert
        {
            try
            {
                Document doc = Application.CurrentDocument;
                Image img = (Image)doc.FindElement(imageName); 
                string strImgPath = img.SourceResolved;

                // no image in box
                if (strImgPath.Length == 0) { return; }

                string strFileExt = Path.GetExtension(strImgPath);
                string strNewPath = strImgPath.Replace(strFileExt, "_round.pdf");
                int radius = 15;
                int totalCrop = radius + 1;				
				
                bool NoVector = img.IsRaster;
                if (NoVector == false)
                {
                    // VECTOR IMAGE (PDF/EPS)

                    //need the proper method for ghost script
					//string fileArgs = @" -q -sDEVICE=pdfwrite -sColorConversionStrategy=Gray  -dProcessColorModel=/DeviceGray -dCompatibilityLevel=1.4 -dPDFSETTINGS=/prepress -dNOPAUSE -dBATCH " + " -sOutputFile=\u0022" + strNewPath + "\u0022" + " \u0022" + strImgPath + "\u0022";
                   	
                    string fileArgs = ;
                    string result = CallGhostScript(fileArgs);
                    if (result == "True")
                    {
                        // Application.Log("image name       : " + strImgName);
                        // Application.Log("image name       : " + strImgNewName);
                        // Application.Log("old image source : " + strImgPath);
                        Application.Log("File Arguments   : " + fileArgs);
                        // Application.Log("new image source : " + strNewPath);
                        img.Source = strNewPath;
                    }
                    else
                    {
                        Application.Log("GhostScript Error!");
                    }
                }

                if (NoVector == true)
                {
                    // BITMAP IMAGE
                    //string fileArgs = @" " + "\u0022" + strImgPath + "\u0022" + " -colorspace Gray -normalize " + "\u0022" + strNewPath + "\u0022";
                    //convert original_picture.png \  \( +clone  -alpha extract \    -draw 'fill black polygon 0,0 0,15 15,0 fill white circle 15,15 15,0' \    \( +clone -flip \) -compose Multiply -composite \    \( +clone -flop \) -compose Multiply -composite \  \) -alpha off -compose CopyOpacity -composite picture_with_rounded_corners.png
                    
					string fileArgs = @" " + "\u0022" + strImgPath + "\u0022" + " ( +clone -crop " + totalCrop + "x" + totalCrop + "+0+0  -fill white -colorize 100% -draw " + "\u0022" + "fill black circle " + radius + "," + radius + " " + radius + ",0" + "\u0022" +  " -background white  -alpha shape ( +clone -flip ) ( +clone -flop ) ( +clone -flip ) ) -flatten "  + "\u0022" + strNewPath + "\u0022";

                    string result = CallImageMagick(fileArgs);
                    if (result == "True")
                    {
                        // Application.Log("image name       : " + strImgName);
                        // Application.Log("image name       : " + strImgNewName);
                        // Application.Log("old image source : " + strImgPath);
                        // Application.Log("File Arguments   : " + fileArgs);
                        // Application.Log("new image source : " + strNewPath);
                        img.Source = strNewPath;
                    }
                    else
                    {
                        Application.Log("ImageMagic Error!");
                    }
                }


            }
            catch (Pageflex.Scripting.Exceptions.ElementNotVisibleException Image)
            {
                Application.Log("Image Not Visible");
            }


            catch (Pageflex.Scripting.Exceptions.ElementNotFoundException Image)
            {
                Application.Log("Image Not found");
            }
        }
    }


 // routine to call ImageMagic
 private static string CallImageMagick(string fileArgs)
    {
        Application.Log("CallImageMagick");
		//string pathImageMagick = @"C:\ImageMagick-6.6.9-Q16\convert.exe";
		//string pathImageMagick = @"C:\Program Files\ImageMagick-6.9.0-Q16\convert.exe";
		string pathImageMagick = Application.GetVariable("_IMPath").Value;
        ProcessStartInfo startInfo = new ProcessStartInfo(pathImageMagick);
        startInfo.Arguments = fileArgs;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        Process exeProcess = Process.Start(startInfo);
        string IMResponse = exeProcess.StandardOutput.ReadToEnd();
        exeProcess.WaitForExit();
        exeProcess.Close();
        if (!string.IsNullOrEmpty(IMResponse))
        {         
        return IMResponse;
        }
        else
        {
            return "True";
        }
     }

    // routine to call GhostScript
    private static string CallGhostScript(string fileArgs)
    {
        Application.Log("CallGhostScript");
		//string pathGhostScript = @"C:\gs\gs9.02\bin\gswin32.exe";
        string pathGhostScript = Application.GetVariable("_GSPath").Value;
        ProcessStartInfo startInfo = new ProcessStartInfo(pathGhostScript);
        startInfo.Arguments = fileArgs;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        Process exeProcess = Process.Start(startInfo);
        string GSResponse = exeProcess.StandardOutput.ReadToEnd();
        exeProcess.WaitForExit();
        exeProcess.Close();
        if (!string.IsNullOrEmpty(GSResponse))
        {
            return GSResponse;
        }
        else
        {
            return "True";
        }
    }
}