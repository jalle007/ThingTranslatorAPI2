using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Google.Apis.Discovery;

namespace ThingTranslatorAPI2 {

  public class LabelDetectior {
    

    /// <returns>an authorized Cloud Vision client.</returns>
    public static VisionService CreateAuthorizedClient()
    {

      createEnvVar();
      try {
        GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;
        // Inject the Cloud Vision scopes
        if (credential.IsCreateScopedRequired) {
          credential = credential.CreateScoped(new[]
          {
                    VisionService.Scope.CloudPlatform
                });
        }
        return new VisionService(new BaseClientService.Initializer {
          HttpClientInitializer = credential,
          GZipEnabled = false
        });
      } catch (Exception ex) {
        Trace.TraceError("CreateAuthorizedClient: " + ex.StackTrace);
      }
      return null;
    }

    private static void createEnvVar()
    {
      var path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "VisionAPI-0a3feb1f1da5.json";
      var exist = System.IO.File.Exists(  path);
      if (exist)
      {Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
        Trace.TraceError("createEnvVar: created " + path);
      }
      else
      {
        Trace.TraceError("createEnvVar: missing " + path  );
      }
    }

    public static IList<AnnotateImageResponse> GetLabels(  string imagePath) {
      try
      {
        VisionService vision = CreateAuthorizedClient();

        // Convert image to Base64 encoded for JSON ASCII text based request   
        byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
        string imageContent = Convert.ToBase64String(imageArray);
        // Post label detection request to the Vision API
        var responses = vision.Images.Annotate(
            new BatchAnnotateImagesRequest() {
              Requests = new[] {
                    new AnnotateImageRequest() {
                        Features = new [] { new Feature() { Type = "LABEL_DETECTION"}},
                        Image = new Image() { Content = imageContent }
                    }
           }
            }).Execute();
        return responses.Responses;
      }
      catch (Exception ex)
      {
       
        Trace.TraceError(ex.StackTrace);
      }
      return null;

    }

    public static IList<AnnotateImageResponse> GetLabels(byte[] imageArray) {
      try
      {
        VisionService vision = CreateAuthorizedClient();

        // Convert image to Base64 encoded for JSON ASCII text based request   
        string imageContent = Convert.ToBase64String(imageArray);
        // Post label detection request to the Vision API
        var responses = vision.Images.Annotate(
            new BatchAnnotateImagesRequest() {
              Requests = new[] {
                    new AnnotateImageRequest() {
                        Features = new [] { new Feature() { Type = "LABEL_DETECTION"}},
                        Image = new Image() { Content = imageContent }
                    }
           }
            }).Execute();
        return responses.Responses;
      }
      catch (Exception ex)
      {
        Trace.TraceError(ex.StackTrace);
      }
      return null;

    }
  }
}