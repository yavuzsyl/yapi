using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YAPI.Contracts.Requests;
using YAPI.Contracts.Responses;
using YAPI.Contracts.V1;
using YAPI.Domain;
using YAPI.Services;

namespace YAPI.Controllers.V1
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody]RegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthenticationResult() { ErrorMessage = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)) });

            var registrationResponse = await identityService.RegisterAsync(request.Email, request.Password);
            if (!registrationResponse.Success)
                return BadRequest(registrationResponse);

            return Ok(registrationResponse);
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthenticationResult() { ErrorMessage = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)) });

            var loginResponse = await identityService.LoginAsync(request.Email, request.Password);
            if (!loginResponse.Success)
                return BadRequest(loginResponse);

            return Ok(loginResponse);
        }
    }
}