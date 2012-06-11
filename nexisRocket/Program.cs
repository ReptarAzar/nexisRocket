using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;
using MFToolkit.Net.Web;
using astra.http;

public class Program
{
    HttpImplementation webServer;

    public static void Main()
    {
        new Program();
    }

    public Program()
    {
        HttpWiflyImpl wireless = new HttpWiflyImpl(processResponse, 80, HttpWiflyImpl.DeviceType.crystal_14_MHz, SPI.SPI_module.SPI1, SecretLabs.NETMF.Hardware.NetduinoPlus.Pins.GPIO_PIN_D10);
        wireless.Open("tgpublic", "");
        wireless.Listen();
    }

    /*
     * The web server business logic
     */
    protected void processResponse(HttpContext context)
    {
        String tz = "GMT-5";
        String content = Resources.homePage;
        String target = context.Request.Path;

        if (target == "/favicon.ico")
        {
            context.Response.ContentType = "image/png";
            context.Response.LastModified = "Tue, 8 Feb 2011 06:45:19 GMT";
            context.Response.ContentLength = Resources.favIcon.Length;
            context.Response.BinaryWrite(Resources.favIcon);
            context.Response.Add("Connection", "close");
            context.Response.Close();
            return;
        }
        Debug.Print("\n\nRequest received for " + context.Request.RawUrl);

        context.Response.ContentType = "text/html";
        context.Response.ContentLength = content.Length;
        context.Response.Add("server", "Tinker/0.0.1");
        context.Response.Add("Cache-Control", "no-cache");
        context.Response.Add("Pragma", "no-cache");
        context.Response.Add("Expires", "0");
        context.Response.Date = getDate(DateTime.Now, tz);
        context.Response.Write(content);
        context.Response.Close();
    }

    static String getDate(DateTime dt, String tz)
    {
        return day[(int)dt.DayOfWeek] + ", " + dt.Day + ' ' + month[dt.Month - 1] + ' ' + dt.Year + ' ' +
            digits(dt.Hour) + ':' + digits(dt.Minute) + ':' + digits(dt.Second) + ' ' + tz;
    }

    static String digits(int t)
    {
        return t < 10 ? "0" + t : "" + t;
    }

    static String[] day = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
    static String[] month = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
}
