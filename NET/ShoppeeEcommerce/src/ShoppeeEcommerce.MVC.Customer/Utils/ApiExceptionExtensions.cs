using Microsoft.AspNetCore.Mvc.ModelBinding;
using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using System.Net;

namespace ShoppeeEcommerce.MVC.Customer.Utils
{
    public static class ApiExceptionExtensions
    {
        public static async Task ToModelState(this ApiException ex, ModelStateDictionary modelState)
        {
            try
            {
                var validationErrors = await ex.GetContentAsAsync<Dictionary<string, string[]>>();

                if (validationErrors != null)
                {
                    foreach (var kvp in validationErrors)
                    {
                        foreach (var error in kvp.Value)
                        {
                            modelState.AddModelError(kvp.Key, error);
                        }
                    }

                    return;
                }
            }
            catch
            {
                // ignored — not this shape
            }
            // Try domain ErrorOr shape
            try
            {
                var domainErrors = await ex.GetContentAsAsync<List<ErrorResponse>>();

                if (domainErrors != null)
                {
                    foreach (var err in domainErrors)
                    {
                        modelState.AddModelError(string.Empty, err.Description);
                    }
                }
            }
            catch
            {
                // ignored — no valid known shape
            }
        }

        public static bool IsNotFound(this ApiException ex) => ex.StatusCode == HttpStatusCode.NotFound;
    }
}
