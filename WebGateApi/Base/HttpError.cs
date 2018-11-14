using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;

namespace WebGateApi.Base
{


    // 12,04,18 Пока горбуха, должен переродится в нормальный хелпер-тулз когда прибавиться опыта....


    /*  что то в этом роде :
     *  200: OK. Все сработало именно так, как и ожидалось.
     *  201: Ресурс был успешно создан в ответ на POST-запрос. Заголовок Location содержит URL, указывающий на только что созданный ресурс.
     *  204: Запрос обработан успешно, и в ответе нет содержимого (для запроса DELETE, например).
     *  304: Ресурс не изменялся. Можно использовать закэшированную версию.
     *  400: Неверный запрос. Может быть связано с разнообразными проблемами на стороне пользователя, такими как неверные JSON-данные в теле запроса, неправильные параметры действия, и т.д.
     *  401: Аутентификация завершилась неудачно.
     *  403: Аутентифицированному пользователю не разрешен доступ к указанной точке входа API.
     *  404: Запрошенный ресурс не существует.
     *  405: Метод не поддерживается. Сверьтесь со списком поддерживаемых HTTP-методов в заголовке Allow.
     *  415: Не поддерживаемый тип данных. Запрашивается неправильный тип данных или номер версии.
     *  422: Проверка данных завершилась неудачно (в ответе на POST-запрос, например). Подробные сообщения об ошибках смотрите в теле ответа.
     *  429: Слишком много запросов. Запрос отклонен из-за превышения ограничения частоты запросов.
     *  500: Внутренняя ошибка сервера. Возможная причина — ошибки в самой программе.
     */


    # region interfaces
    public interface ICommonError
    {
        string Name { get; set; }
        string Message { get; set; }
        int Code { get; set; }
        int StatusCode { get; set; }
        object Data { get; set; }
    }

    public interface IErrorBuilder<TTarget>
    {
        ICommonError Info(string message);                                      // 100    
        ICommonError Success(string message);                                   // 200
        ICommonError SuccessPost(string location);                              // 201
        ICommonError SuccessPost(string message, HeaderDictionary addHeaders);  // 201
        ICommonError Redirect(string message);                                  // 300
        ICommonError ClientError(string message);                               // 400
        ICommonError ServerError(string message);                               // 500

        void EerectTo(ICommonError error, TTarget targetObj);
        string ToJsonString(ICommonError error);
    }

    public interface IHttpErrorBuilder : IErrorBuilder<HttpResponse>
    {
        //bool IsPutErrorToBody { get; set; }
    }
    #endregion    interfaces 

    #region CommonError
    class CommonError : ICommonError
    {
        const string OK_POST = @"The resource was successfully created in response to a POST request. The Location header contains a URL pointing to the resource you just created.";

        public string Name { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }

        //public CommonError() : this("OK", "", 0, 200) { }

        public CommonError(string name, string message, int code, int statusCode, object data = null)
        {
            Name = name;
            Message = message;
            Code = code;
            StatusCode = statusCode;
            Data = data;
        }

        public static ICommonError Info(string message)
        {
            return new CommonError("Information", message, 0, 100);
        }

        public static ICommonError Success(string message)
        {
            return new CommonError("OK", message, 0, 200);
        }

        public static ICommonError SuccessPost(string location = null)
        {
            return new CommonError("OK", OK_POST, 0, 201, location != null ? (new HeaderDictionary { { "Location", location } }) : null );
        }

        public static ICommonError SuccessPost(string message, HeaderDictionary addHeaders )
        {
            return new CommonError("OK", message, 0, 201, addHeaders);
        }

        public static ICommonError Redirect(string message)
        {
            return new CommonError("Redirected", message, 0, 300);
        }
        public static ICommonError ClientError(string message)
        {
            return new CommonError("Client error", message, 0, 400);
        }
        public static ICommonError ServerError(string message)
        {
            return new CommonError("Internalserver error", message, 0, 500);
        }
    }
    #endregion CommonError

    public class ErrorBuilder<TTarget> : IErrorBuilder<TTarget>
    {
        private Action<ICommonError, TTarget> ErectFunc;

        public ErrorBuilder(Action<ICommonError, TTarget> erectFunc) {
            ErectFunc = erectFunc;
        }

        public ICommonError Info(string message) => CommonError.ClientError(message);
        public ICommonError Success(string message) => CommonError.Success(message);
        public ICommonError SuccessPost(string location) => CommonError.SuccessPost(location);                                            // 201
        public ICommonError SuccessPost(string message, HeaderDictionary addHeaders) => CommonError.SuccessPost(message,  addHeaders);    // 201
        public ICommonError Redirect(string message) => CommonError.Redirect(message);
        public ICommonError ClientError(string message) => CommonError.ClientError(message);
        public ICommonError ServerError(string message) => CommonError.ServerError(message);

        public void EerectTo(ICommonError error, TTarget targetObj)
        {
            ErectFunc(error, targetObj);
        }

        public string ToJsonString(ICommonError err) => ToJson(err);


        protected static string ToJson(ICommonError err)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("\"name\":\"" + err.Name + "\",");
            sb.AppendLine("\"message\":\"" + err.Message + "\",");
            sb.AppendLine("\"code\":" + err.Code.ToString() + ",");
            sb.AppendLine("\"statusCode\":" + err.StatusCode.ToString() + "");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }



    public class HttpErrorBuilder : ErrorBuilder<HttpResponse>, IHttpErrorBuilder
    {
        
        private static Action<ICommonError, HttpResponse> erFunc = (err, resp) => erFuncEx(err, resp, ToJson);

        private static Action<ICommonError, HttpResponse, Func<ICommonError, string>> erFuncEx = (err, resp, f ) =>
        {
            resp.StatusCode = err.StatusCode;
            resp.Headers.Add("Status", ToHttpHeader( err.Name) );
            resp.Headers.Add("StatusDescription", ToHttpHeader(err.Message)); 
            resp.Headers.Add("StatusSubCode", ToHttpHeader(err.Code.ToString()) );

            (err.Data as HeaderDictionary)?.ToList().ForEach(x => resp.Headers.Add(x.Key, ToHttpHeader(x.Value)));

        };

        private static string ToUtf8(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Encoding.UTF8.GetString(bytes);
        }

        private static string ToAscii(string str)
        {

            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            byte[] unicodeBytes = unicode.GetBytes(str);

            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            return asciiString;
        }

        private static string ToHttpHeader(string str)
        {
            var encoder = HtmlEncoder.Default;
            var ret = encoder.Encode(str);
            return ret;
        }
        
        public HttpErrorBuilder() : this(erFunc)
        {
        }

        public HttpErrorBuilder(Action<ICommonError, HttpResponse> erectFunc) : base(erectFunc)
        {
            //IsPutErrorToBody = false;
        }

        
    }

}
