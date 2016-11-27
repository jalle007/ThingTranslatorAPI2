﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
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

  public class Response {
    public string Original { get; set; }
    public string Translation { get; set; }
    public string Error { get; set; }

  }


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
    public async Task<JsonResult<Response>> Upload() {
      if (!Request.Content.IsMimeMultipartContent())
        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

      createEnvVar();
      String bestGuess, translated, langCode = string.Empty;
      IList<AnnotateImageResponse> result;
      var res = new Response();
      byte[] buffer = null;
      IEnumerable<string> headerValues;
      if (Request.Headers.TryGetValues("langCode", out headerValues)) {
        langCode = headerValues.FirstOrDefault();

      }
       

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

      result = LabelDetectior.GetLabels(buffer);
      

      try {
        bestGuess = result[0].LabelAnnotations.FirstOrDefault()?.Description;
        res.Original = bestGuess;

        translated = TranslateText(bestGuess, "en", langCode);
        res.Translation = translated;

      } catch (Exception ex) {
        Trace.TraceError(ex.Message);

        res.Error = ex.Message;
        return Json(res);
      }


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

      try {
        var _result = GoogleTranslate.Translate.Query(_request);
        return _result.Data.Translations.First().TranslatedText;
      } catch (Exception ex) {
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

    private static void createEnvVar() {
      var path = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "VisionAPI-0a3feb1f1da5.json";
      var exist = System.IO.File.Exists(path);
      if (exist) {
        if (Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") == null) {
          Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
          Trace.TraceError("createEnvVar: created " + path);
        }
      } else {
        Trace.TraceError("createEnvVar: missing " + path);
      }
    }
  }
}
