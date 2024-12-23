using System.Net;
using Microsoft.AspNetCore.Http;
//Xay dung phuong thuc mo rong su dung trong Program
namespace App.ExtendMethods
{
    public static class AppExtends
    {
        public static void AddStatusCodePages(this WebApplication? app)
        {
            app.UseStatusCodePages(appErr =>
            {
                appErr.Run(async context =>
                {
                    var reponse = context.Response;
                    var code = reponse.StatusCode;
                    var content = @$"
                                    <html>
                                        <head>
                                            <title> Loi {code}</title>
                                        </head>
                                        <body>
                                            <h2> Co loi xay ra: {code} - {(HttpStatusCode)code} </h2>
                                        </body>
                                    </html>
                                ";
                    await reponse.WriteAsync(content);
                });
            });
        }
    }
}