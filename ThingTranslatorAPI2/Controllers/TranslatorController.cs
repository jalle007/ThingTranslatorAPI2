using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Google.Apis.Vision.v1.Data;
using GoogleApi;
using GoogleApi.Entities.Translate.Translate.Request;
using TranslationsResource = Google.Apis.Translate.v2.Data.TranslationsResource;

namespace ThingTranslatorAPI2.Controllers {
   
  [RoutePrefix("api")]
  public class TranslatorController : ApiController
  {

    
     // "AIzaSyCUD75r6fNhZE5Xa8TNJaAeAXrSWzg-BiM";

    //public class MyMultipartFormDataStreamProvider : MultipartFormDataStreamProvider {
    //  public MyMultipartFormDataStreamProvider(string path)
    //      : base(path) {
    //  }
    //}

    [Route("upload")]
    [HttpPost]
    public async Task<JsonResult<Response>> Upload() {
      if (!Request.Content.IsMimeMultipartContent())
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

      String bestGuess, translated, langCode = string.Empty;
      IList<AnnotateImageResponse> labels;
      var response = new Response();
      byte[] buffer = null;
     
       

      var provider = new MultipartMemoryStreamProvider();
      await Request.Content.ReadAsMultipartAsync(provider);

      // var file = provider.Contents[0];

      foreach (var content in provider.Contents)
      {
        if (content.Headers.ContentLength>10)
        {
           buffer = await content.ReadAsByteArrayAsync();
          //Do whatever you want with filename and its binaray data.
          //var path = System.Web.Hosting.HostingEnvironment.MapPath( "~/" ) + "123.jpg";
          //File.WriteAllBytes(path, buffer);
        }
        else
        {
          langCode = await content.ReadAsStringAsync();

        }
      }

      labels = LabelDetectior.GetLabels(buffer);
      
      try {
        //Take the first result that has the highest score
        bestGuess = labels[0].LabelAnnotations.FirstOrDefault()?.Description;

        //translate text
        translated = TranslateText(bestGuess, "en", langCode);

        response.Original = bestGuess;
        response.Translation = translated;

      } catch (Exception ex) {
        Trace.TraceError(ex.Message);

        response.Error = ex.Message;
        return Json(response);
      }

      return Json(response);
    }

    //dummy method
    [Route("translate")]
    public IHttpActionResult GetTranslation() {
      return Ok(TranslateText("Hello", "en", "bs"));
    }

    //Translate text from source to target language
    private String TranslateText(String text, String source, String target) {
    

      var _request = new TranslateRequest {
        Source = source,
        Target = target,
        Qs = new[] { text },
        Key = getApiKey()
      };

      try {
        var _result = GoogleTranslate.Translate.Query(_request);
        return _result.Data.Translations.First().TranslatedText;
      } catch (Exception ex) {
        return ex.Message;
      }
    }

    private static String getApiKey() {
      return (Environment.GetEnvironmentVariable("apiKey"));  
    }
  }
}
