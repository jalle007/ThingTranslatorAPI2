using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ThingTranslatorAPI2 {
  public class WebApiApplication : System.Web.HttpApplication {
    protected void Application_Start() {
      GlobalConfiguration.Configure(WebApiConfig.Register);
      createEnvVar();
    }

    /*Use your own VisionAPI key here
   * To create new key go to : https://cloud.google.com/vision/docs/quickstart
   */
    private static void createEnvVar() {
      var GAC = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
      if (GAC == null) {
        var VisionApiKey = Environment.GetEnvironmentVariable("VisionApiKey");
        if (VisionApiKey != null) {
          var path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "VisionAPI-0a3feb1f1da5.json";

          Trace.TraceError("path: " + path);

          File.WriteAllText(VisionApiKey, path);
          Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        }
      }

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
