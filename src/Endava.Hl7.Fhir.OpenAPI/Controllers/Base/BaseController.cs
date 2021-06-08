using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Endava.Hl7.Fhir.OpenAPI.Controllers.Base
{
    // Inject common services in a BaseController
    public abstract class BaseController<T> : ControllerBase where T: BaseController<T>
	{
		private ILogger<T> _logger;

		protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();

        /// <summary>
        /// Return ModelState errors
        /// </summary>
        /// <returns>List of errors</returns>
        protected List<string> ModelStateErrors()
        {
            var modelErrors = ModelState.Values.Aggregate(
                   new List<string>(),
                   (errors, mse) =>
                   {
                       errors.AddRange(mse.Errors.Select(err => err.ErrorMessage));
                       return errors;
                   },
                   errors => errors
                );
            return modelErrors;
        }
    }
}
