using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web.Http;

namespace ThingTranslatorAPI2 {
  public class Global : System.Web.HttpApplication {

    public static String apiKey;


    protected void Application_Start() {
      GlobalConfiguration.Configure(WebApiConfig.Register);
        apiKey = ConfigurationManager.AppSettings["apiKey"];
     
      Application["apiKey"] = apiKey;
      Trace.TraceError("apiKey: " + apiKey);

      createEnvVar();

    }

    /*Use your own VisionAPI key here
   * To create new key go to : https://cloud.google.com/vision/docs/quickstart
   */

    /*  If GAC is not alreadz stored at zour szstem we create it on the file
     *  First we get value od VisionaPIKez which is actuallz JSON filoe stored as EnvVariable
     *  Then we save that string to file
     *  Then set GOOGLE_APPLICATION_CREDENTIALS to point to that file
     *  in the end we should have this Enviromental variable stored at your host
     *  GOOGLE_APPLICATION_CREDENTIALS=\part\to\your\visionapiikey.json
     */

    private static void createEnvVar() {
      var GAC = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
      Trace.TraceError("GAC: " + GAC);

     // if (Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") == null) {
     //   var path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "VisionAPI-0a3feb1f1da5.json";
     //   Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
     //   Trace.TraceError("createEnvVar: created " + path);
     // }
     //else {
           
     //   }

     // return;

        if (GAC == null) {
        var VisionApiKey = ConfigurationManager.AppSettings["VisionApiKey"]; //Environment.GetEnvironmentVariable("VisionApiKey");
        if (VisionApiKey != null) {
          var path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "VisionAPI-0a3feb1f1da5.json";

          Trace.TraceError("path: " + path);

          File.WriteAllText(VisionApiKey, path);
          Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        }
      }

    }
  }
}
