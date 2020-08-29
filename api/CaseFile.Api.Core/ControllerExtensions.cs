using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CaseFile.Api.Core
{
    public static class ControllerExtensions
    {
        public static readonly int LOWER_OBS_VALUE = 1;
        public static readonly int UPPER_OBS_VALUE = 300;
        public static readonly string RESET_ERROR_MESSAGE = "Internal server error, please verify that provided id is correct ";
        public static readonly string DEVICE_RESET = "device";
        public static readonly string PASSWORD_RESET = "reset-password";

        public static int GetIdOngOrDefault(this Controller controller, int defaultIdOng)
        {
            return int.TryParse(controller.User.Claims.FirstOrDefault(a => a.Type == ClaimsHelper.IdNgo)?.Value, out var result)
                ? result
                : defaultIdOng;
        }

        public static int GetCurrentUserId(this Controller controller)
        {
            return int.Parse(controller.User.Claims.First(c => c.Type == ClaimsHelper.UserIdProperty).Value);
        }

        public static IAsyncResult ResultAsync(this Controller controller, HttpStatusCode statusCode, ModelStateDictionary modelState = null)
        {
            controller.Response.StatusCode = (int)statusCode;

            if (modelState == null)
            {
                return Task.FromResult(new StatusCodeResult((int)statusCode));
            }

            return Task.FromResult(controller.BadRequest(modelState));
        }
    }
}
