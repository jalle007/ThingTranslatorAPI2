using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Apis.Translate.v2.Data;
using Google.Apis.Vision.v1.Data;
using GoogleApi;
using GoogleApi.Entities.Translate.Translate.Request;
using TranslationsResource = Google.Apis.Translate.v2.Data.TranslationsResource;

namespace ThingTranslatorAPI2.Controllers {
  [RoutePrefix("api")]
  public class TranslatorController : ApiController {
    String apiKey = "AIzaSyCUD75r6fNhZE5Xa8TNJaAeAXrSWzg-BiM";

    public class MyMultipartFormDataStreamProvider : MultipartFormDataStreamProvider {
      public MyMultipartFormDataStreamProvider(string path)
          : base(path) {
      }
    }

    [Route("upload")]
    [HttpPost]
    public async Task<JsonResult<List<object>>> Upload() {
      if (!Request.Content.IsMimeMultipartContent())
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

      IList<AnnotateImageResponse> result;
      var provider = new MultipartMemoryStreamProvider();
      await Request.Content.ReadAsMultipartAsync(provider);

      var res = new List<object>();
      var file = provider.Contents[0];
    
      var buffer = await file.ReadAsByteArrayAsync();
      //Do whatever you want with filename and its binaray data.
     
   
      String bestGuess, translated;
       
      result = LabelDetectior.GetLabels(buffer);
      
      res.Add(new { GetLabels = result  });
      return Json(res);


      res.Add(new { Result = result });
      try
      {
       bestGuess = result[0].LabelAnnotations.FirstOrDefault()?.Description;
        res.Add(new { BestGuess = bestGuess });
        translated = TranslateText(bestGuess, "en", "hr");
        res.Add(new { Translated = translated });

      } catch (Exception ex)
      {
        Trace.TraceError(ex.Message);

        res.Add(new { Translation = ex.Message.ToString() });
        return Json(res);
        throw;
      }
      res.Add(  new { Translation = translated } );
       
      return Json(res);
    }

    [Route("translate")]
    public IHttpActionResult GetTranslation() {
      return Ok(TranslateText("Hello", "en", "hr"));
    }

    private String TranslateText(String text, String source, String target) {
      var _request = new TranslateRequest {
        Source = source,
        Target = target,
        Qs = new[] { text },
        Key = apiKey
      };

      try{
      var _result = GoogleTranslate.Translate.Query(_request);
      return _result.Data.Translations.First().TranslatedText;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    private StreamContent StreamConversion() {
      Stream reqStream = Request.Content.ReadAsStreamAsync().Result;
      var tempStream = new MemoryStream();
      reqStream.CopyTo(tempStream);

      tempStream.Seek(0, SeekOrigin.End);
      var writer = new StreamWriter(tempStream);
      writer.WriteLine();
      writer.Flush();
      tempStream.Position = 0;

      var streamContent = new StreamContent(tempStream);
      foreach (var header in Request.Content.Headers) {
        streamContent.Headers.Add(header.Key, header.Value);
      }
      return streamContent;
    }
  }
}
