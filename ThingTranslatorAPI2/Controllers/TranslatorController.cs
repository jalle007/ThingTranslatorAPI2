using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Apis.Vision.v1.Data;
using TranslationsResource = Google.Apis.Translate.v2.Data.TranslationsResource;

namespace ThingTranslatorAPI2.Controllers
{
  [RoutePrefix("api")]
  public class TranslatorController : ApiController
    {
    String apiKey = "AIzaSyCUD75r6fNhZE5Xa8TNJaAeAXrSWzg-BiM";
    static String translated;

    public class MyMultipartFormDataStreamProvider : MultipartFormDataStreamProvider {
      public MyMultipartFormDataStreamProvider(string path)
          : base(path) {
      }
    }

    [Route("files")]
    [HttpPost]
    public async Task<JsonResult<List<object>>> Upload() {
      if (!Request.Content.IsMimeMultipartContent())
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

      IList<AnnotateImageResponse> result;
      var provider = new MultipartMemoryStreamProvider();
      await Request.Content.ReadAsMultipartAsync(provider);
      foreach (var file in provider.Contents) {
        if (file.Headers.ContentLength != 0) {
          var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
        }
        var buffer = await file.ReadAsByteArrayAsync();

        //Do whatever you want with filename and its binaray data.
        result = LabelDetectior.GetLabels(buffer);
        var bestGuess = result[0].LabelAnnotations.FirstOrDefault().Description;

        new TranslatorController().Translate(bestGuess).Wait();

        /*
        var labels = new List<object>();
        foreach (var lbl in result[0].LabelAnnotations)
        {
          labels.Add(new { Label = lbl.Description, Score = lbl.Score });
        }
        
        var ret = Json(labels);
        return ret;
        */
      }

      var res = new List<object>();
      res.Add(new { Translation = translated });
      var x = Json(res);
      return x;
      //return Ok();
    }

    public async Task Translate(String srcText) {
      var service = new TranslateService(new BaseClientService.Initializer() {
        ApiKey = "AIzaSyCUD75r6fNhZE5Xa8TNJaAeAXrSWzg-BiM",
        ApplicationName = "Translate API Sample"
      });

      // Execute the first translation request.
      var response = await service.Translations.List(srcText, "bs").ExecuteAsync();
      var translations = new List<string>();

      foreach (TranslationsResource translation in response.Translations) {
        translations.Add(translation.TranslatedText);
        Console.WriteLine("translating " + srcText + ": " + translation.TranslatedText);
      }
      translated = translations[0];
    }


    [Route("translate")]
    public IHttpActionResult GetTranslation() {

      return Ok();
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
