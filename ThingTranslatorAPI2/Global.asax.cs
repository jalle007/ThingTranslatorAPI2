﻿using System;
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

      /*  If GAC is not alreadz stored at zour szstem we create it on the file
       *  First we get value od VisionaPIKez which is actuallz JSON filoe stored as EnvVariable
       *  Then we save that string to file
       *  Then set GOOGLE_APPLICATION_CREDENTIALS to point to that file
       *  in the end we should have this Enviromental variable stored at your host
       *  GOOGLE_APPLICATION_CREDENTIALS=\part\to\your\visionapiikey.json
       */
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
