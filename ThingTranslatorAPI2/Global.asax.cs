using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ThingTranslatorAPI2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            createEnvVar();
    }

    private static void createEnvVar() {
      /*Use your own VisionAPI key here
       * To create new key go to : https://cloud.google.com/vision/docs/quickstart
       */
      var VisionApiKey = (Environment.GetEnvironmentVariable("VisionApiKey"));
      var path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "VisionAPI-0a3feb1f1da5.json";
      var exist = System.IO.File.Exists(path);
      if (!exist) {
        File.WriteAllText(VisionApiKey, path);
      }
      Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

      //if (exist) {
      //  if (Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") == null) {
      //    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
      //    Trace.TraceError("createEnvVar: created " + path);
      //  }
      //} else {
      //  Trace.TraceError("createEnvVar: missing " + path);
      //}
    }
  }
}
